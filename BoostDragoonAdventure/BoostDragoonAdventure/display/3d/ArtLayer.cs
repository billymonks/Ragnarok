using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.map;
using wickedcrush.manager.gameplay;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using System.IO;

namespace wickedcrush.display._3d
{
    public class ArtLayer
    {
        List<TileLayer> tileLayers;
        //List<SurfaceTileLayer> surfaceTileLayers;

        public ArtLayer(GameBase game, Map map, int baseHeight, int height)
        {
            LoadCaveTileLayers(game, map, baseHeight, height);
        }

        public ArtLayer(GameBase game, bool[,] data, int baseHeight, int height, string path)
        {
            
            //surfaceTileLayers = new List<SurfaceTileLayer>();
            LoadTileLayers(game, data, baseHeight, height, "Content/img/layer/"+path+".xml");
        }

        private void LoadTileLayers(GameBase game, bool[,] data, int baseHeight, int height, string path)
        {
            tileLayers = new List<TileLayer>();

            XDocument doc;
            XElement rootElement;

            if (File.Exists(path))
            {
                doc = XDocument.Load(path);
                rootElement = new XElement(doc.Element("artlayer"));

                foreach (XElement e in rootElement.Element("tilelayers").Elements("tileset"))
                {
                    AddTileLayer(game, data, baseHeight, height, e);
                }

            }
        }

        private void AddTileLayer(GameBase game, bool[,] data, int baseHeight, int height, XElement tileElement)
        {
            foreach (XElement e in tileElement.Element("transformations").Elements("transformation"))
            {
                if (e.Attribute("mode").Value.Equals("scale"))
                {
                    data = LayerTransformations.ScaleLayer(data, int.Parse(e.Value));
                }
                else if (e.Attribute("mode").Value.Equals("shrink"))
                {
                    data = LayerTransformations.ShrinkLayer(data, int.Parse(e.Value));
                }
                else if (e.Attribute("mode").Value.Equals("invert"))
                {
                    data = LayerTransformations.InvertLayer(data);
                }
            }

            if (Boolean.Parse(tileElement.Attribute("surface").Value))
                tileLayers.Add(new SurfaceTileLayer(game, data, height, tileElement.Value, bool.Parse(tileElement.Attribute("edgeonly").Value)));
            else
                tileLayers.Add(new WallTileLayer(game, data, height, baseHeight, tileElement.Value));
        }

        private void LoadCaveTileLayers(GameBase game, Map map, int baseHeight, int height)
        {
            tileLayers = new List<TileLayer>();

            bool[,] data = LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.DEATHSOUP].data, true);

            //data = ScaleLayer(data, 2);



            data = LayerTransformations.InvertLayer(LayerTransformations.ScaleLayer(map.layerList[LayerType.DEATHSOUP].data, 2));
            tileLayers.Add(new WallTileLayer(game, data, baseHeight, baseHeight - 1, "cliff16"));
            tileLayers.Add(new SurfaceTileLayer(game, data, baseHeight, "dungeon_floor_4x4", false));

            data = LayerTransformations.ScaleLayer(map.layerList[LayerType.WALL].data, 2);
            tileLayers.Add(new WallTileLayer(game, data, height, baseHeight, "wall16"));
            tileLayers.Add(new SurfaceTileLayer(game, data, height, "dungeon_floor_4x4", false));
        }

        

        public void DrawLayer(GameBase game, GameplayManager gameplay, Effect normalMappingEffect, Dictionary <string, PointLightStruct> lightList)
        {
            foreach (TileLayer t in tileLayers)
            {
                DrawLayer(game, gameplay, normalMappingEffect, lightList, t);
            }
        }

        private void DrawLayer(GameBase game, GameplayManager gameplay, Effect normalMappingEffect, Dictionary <string, PointLightStruct> lightList, TileLayer layer)
        {
            //PrepareVertices(gameplay, layer);

            if (layer.solidGeomVertices.Count <= 0)
                return;


            //buffer.SetData(solidGeomVertices.ToArray());

            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            normalMappingEffect.Parameters["ColorMap"].SetValue(layer.colorTexture);
            normalMappingEffect.Parameters["NormalMap"].SetValue(layer.normalTexture);

            game.GraphicsDevice.SetVertexBuffer(layer.buffer);

            game.GraphicsDevice.BlendState = BlendState.Opaque;

            normalMappingEffect.CurrentTechnique.Passes["Ambient"].Apply();

            game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, layer.solidGeomVertices.Count / 3);

            game.GraphicsDevice.BlendState = BlendState.Additive;

            foreach (KeyValuePair<string, PointLightStruct> s in lightList)
            {
                normalMappingEffect.Parameters["DiffuseColor"].SetValue(s.Value.DiffuseColor);
                normalMappingEffect.Parameters["DiffuseIntensity"].SetValue(s.Value.DiffuseIntensity);
                normalMappingEffect.Parameters["SpecularColor"].SetValue(s.Value.SpecularColor);
                normalMappingEffect.Parameters["SpecularIntensity"].SetValue(s.Value.SpecularIntensity);
                normalMappingEffect.Parameters["PointLightPosition"].SetValue(s.Value.PointLightPosition);
                normalMappingEffect.Parameters["PointLightRange"].SetValue(s.Value.PointLightRange);

                normalMappingEffect.CurrentTechnique.Passes["Point"].Apply();
                game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, layer.solidGeomVertices.Count / 3);
            }
            //game.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 
        }
    }

    public static class LayerTransformations
    {
        public static bool[,] ScaleLayer(bool[,] a, int scale) //must be power of 2, makes bigger
        {
            bool[,] b = new bool[(a.GetLength(0) * scale), (a.GetLength(1) * scale)];

            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    for (int k = 0; k < scale; k++)
                    {
                        b[i * scale, j * scale] = a[i, j];
                        b[i * scale + k, j * scale] = a[i, j];
                        b[i * scale, j * scale + k] = a[i, j];
                        b[i * scale + k, j * scale + k] = a[i, j];
                    }
                }
            }

            return b;
        }

        public static bool[,] GetCompositeLayer(bool[,] a, bool[,] b, bool or) //or = ||, and = &&
        {
            bool[,] result = new bool[a.GetLength(0), a.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (or)
                        result[i, j] = a[i, j] || b[i, j];
                    else
                        result[i, j] = a[i, j] && b[i, j];
                }
            }

            return result;
        }

        public static bool[,] ShrinkLayer(bool[,] a, int count)
        {
            bool[,] b = new bool[a.GetLength(0), a.GetLength(1)];

            if (count > 0)
            {
                count--;

                for (int i = 1; i < a.GetLength(0) - 1; i++)
                {
                    for (int j = 1; j < a.GetLength(1) - 1; j++)
                    {
                        b[i, j] = (a[i, j] && a[i, j - 1]);
                        b[i, j] = (b[i, j] && a[i, j + 1]);
                        b[i, j] = (b[i, j] && a[i - 1, j]);
                        b[i, j] = (b[i, j] && a[i + 1, j]);
                    }
                }
            }

            if (count == 0)
                return b;
            else
                return ShrinkLayer(b, count);
        }

        public static bool[,] InvertLayer(bool[,] a)
        {
            bool[,] result = new bool[a.GetLength(0), a.GetLength(1)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    result[i, j] = !a[i, j];
                }
            }

            return result;
        }
    }
}
