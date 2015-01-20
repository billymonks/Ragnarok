using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.display._3d.vertex;
using wickedcrush.map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.manager.gameplay;
using wickedcrush.display._3d.texture;

namespace wickedcrush.display._3d
{
    public class Scene
    {
        const int ART_GRID_SIZE = 10;
        //public List<WCVertex> solidGeomVertices;

        public Vector3 cameraPosition;
        public Vector3 cameraDirection;

        //private DynamicVertexBuffer buffer;

        Effect normalMappingEffect;

        Matrix viewMatrix;

        private GameBase game;

        Vector2 sceneDimensions;

        SurfaceTileLayer floorOuterLayer, floorInnerLayer; // todo: group variable number of layers in a parent class
        SurfaceTileLayer wallSurfaceOuterLayer, wallSurfaceInnerLayer; // todo: group variable number of layers in a parent class
        WallTileLayer wallLayer, cliffLayer;

        

        public Scene(GameBase game)
        {
            this.game = game;
            
            normalMappingEffect = game.Content.Load<Effect>(@"fx/NormalMappingMultiLights");
            
        }

        public void BuildScene(GameBase game, Map map)
        {
            bool[,] data = GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.DEATHSOUP].data, true);
            //bool[,] data = map.layerList[LayerType.DEATHSOUP].data;
            data = ScaleLayer(data, 2);

            wallLayer = new WallTileLayer(game, ScaleLayer(map.layerList[LayerType.WALL].data, 2), 3, 0, "pink_outer");
            cliffLayer = new WallTileLayer(game, InvertLayer(ScaleLayer(map.layerList[LayerType.DEATHSOUP].data, 2)), 0, -1, "pink_outer");

            data = InvertLayer(data);
            floorOuterLayer = new SurfaceTileLayer(game, data, 0, "pink_outer", true);
            data = ShrinkLayer(data, 1);
            floorInnerLayer = new SurfaceTileLayer(game, data, 0, "pink_inner", false);

            data = ScaleLayer(map.layerList[LayerType.WALL].data, 2);
            wallSurfaceOuterLayer = new SurfaceTileLayer(game, data, 3, "pink_outer", true);
            wallSurfaceInnerLayer = new SurfaceTileLayer(game, ShrinkLayer(data, 1), 3, "pink_inner", false);
            

            SetEffectParameters();
            
        }

        public bool[,] ScaleLayer(bool[,] a, int scale) //must be power of 2, makes bigger
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

        public bool[,] GetCompositeLayer(bool[,] a, bool[,] b, bool or) //or = ||, and = &&
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

        public bool[,] ShrinkLayer(bool[,] a, int count)
        {
            bool[,] b = new bool[a.GetLength(0), a.GetLength(1)];

            if (count > 0)
            {
                count--;
                
                for (int i = 1; i < a.GetLength(0)-1; i++)
                {
                    for (int j = 1; j < a.GetLength(1)-1; j++)
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

        public bool[,] InvertLayer(bool[,] a)
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

        public void DrawLayer(GameBase game, GameplayManager gameplay, TileLayer layer)
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

            normalMappingEffect.CurrentTechnique.Passes["Point"].Apply();
            game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, layer.solidGeomVertices.Count / 3);
            //game.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 
        }

        public void DrawScene(GameBase game, GameplayManager gameplay)
        {
            cameraPosition = new Vector3(gameplay.camera.cameraPosition.X + 320, 100f, gameplay.camera.cameraPosition.Y + 380);
            viewMatrix = Matrix.CreateLookAt(cameraPosition, new Vector3(cameraPosition.X, 0f, cameraPosition.Z - 100), new Vector3(0f, 0.5f, -0.5f));

            normalMappingEffect.Parameters["View"].SetValue(viewMatrix);
            normalMappingEffect.Parameters["EyePosition"].SetValue(cameraPosition);
            normalMappingEffect.CurrentTechnique = normalMappingEffect.Techniques["MultiPassLight"]; //geom has normal map (some sprites do not)

            normalMappingEffect.Parameters["DiffuseColor"].SetValue(new Vector4(0.7f, 0.75f, 0.9f, 1f));
            normalMappingEffect.Parameters["DiffuseIntensity"].SetValue(0.9f);
            normalMappingEffect.Parameters["SpecularColor"].SetValue(new Vector4(0.7f, 0.75f, 0.9f, 1f));
            normalMappingEffect.Parameters["SpecularIntensity"].SetValue(1f);
            normalMappingEffect.Parameters["PointLightPosition"].SetValue(new Vector3(cameraPosition.X + 10, 30f + 100, cameraPosition.Z - 120 + 300));
            normalMappingEffect.Parameters["PointLightRange"].SetValue(1000);

            DrawLayer(game, gameplay, cliffLayer);
            DrawLayer(game, gameplay, wallLayer);
            DrawLayer(game, gameplay, floorOuterLayer);
            DrawLayer(game, gameplay, floorInnerLayer);
            DrawLayer(game, gameplay, wallSurfaceOuterLayer);
            DrawLayer(game, gameplay, wallSurfaceInnerLayer);

            
            

            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            
            
        }

        private void SetEffectParameters()
        {
            normalMappingEffect.Parameters["World"].SetValue(Matrix.Identity);

            sceneDimensions = new Vector2(480 * game.aspectRatio, 480);
            normalMappingEffect.Parameters["Projection"].SetValue(Matrix.CreateOrthographic(480 * game.aspectRatio, 480, -200, 400));
            //normalMappingEffect.Parameters["Projection"].SetValue(Matrix.CreatePerspective(32 * game.aspectRatio, 16, 10, 1600));
            
            normalMappingEffect.Parameters["AmbientColor"].SetValue(new Vector4(1f, 1f, 1f, 1f));
            normalMappingEffect.Parameters["AmbientIntensity"].SetValue(0.115f);

            normalMappingEffect.Parameters["baseColor"].SetValue(new Vector4(0f, 0f, 0f, 1f));

        }

        
    }
}
