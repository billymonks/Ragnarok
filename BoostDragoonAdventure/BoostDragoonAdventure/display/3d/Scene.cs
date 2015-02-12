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
    public class PointLightStruct
    {
        public Vector4 DiffuseColor;
        public float DiffuseIntensity;
        public Vector4 SpecularColor;
        public float SpecularIntensity;
        public Vector3 PointLightPosition;
        public float PointLightRange;

        public PointLightStruct(Vector4 DiffuseColor, float DiffuseIntensity, Vector4 SpecularColor, float SpecularIntensity, Vector3 PointLightPosition, float PointLightRange)
        {
            this.DiffuseColor = DiffuseColor;
            this.DiffuseIntensity = DiffuseIntensity;
            this.SpecularColor = SpecularColor;
            this.SpecularIntensity = SpecularIntensity;
            this.PointLightPosition = PointLightPosition;
            this.PointLightRange = PointLightRange;
        }
    }
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

        public Dictionary<string, PointLightStruct> lightList = new Dictionary<string,PointLightStruct>();

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

            wallLayer = new WallTileLayer(game, ScaleLayer(map.layerList[LayerType.WALL].data, 2), 2, 0, "wall16");
            cliffLayer = new WallTileLayer(game, InvertLayer(ScaleLayer(map.layerList[LayerType.DEATHSOUP].data, 2)), 0, -1, "cliff16");

            data = InvertLayer(ScaleLayer(map.layerList[LayerType.DEATHSOUP].data, 2));
            floorOuterLayer = new SurfaceTileLayer(game, data, 0, "dungeon_floor_4x4", false);
            //data = ShrinkLayer(data, 1);
            //floorInnerLayer = new SurfaceTileLayer(game, data, 0, "dungeon_floor_4x4", false);

            data = ScaleLayer(map.layerList[LayerType.WALL].data, 2);
            wallSurfaceOuterLayer = new SurfaceTileLayer(game, data, 2, "dungeon_floor_4x4", false);
            //wallSurfaceInnerLayer = new SurfaceTileLayer(game, ShrinkLayer(data, 1), 3, "rock_blue", false);
            

            SetEffectParameters();

            
            lightList.Add("camera", new PointLightStruct(new Vector4(0.7f, 0.75f, 0.9f, 1f), 0.6f, new Vector4(0.7f, 0.75f, 0.9f, 1f), 0f, new Vector3(cameraPosition.X + 10, 30f + 100, cameraPosition.Z - 120 + 300), 1000f));
            lightList.Add("character", new PointLightStruct(new Vector4(1f, 0.65f, 0.5f, 1f), 0.9f, new Vector4(1f, 0.65f, 0.5f, 1f), 1f, new Vector3(cameraPosition.X + 10, 30f, cameraPosition.Z - 120), 100f));
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

        public void DrawScene(GameBase game, GameplayManager gameplay)
        {
            cameraPosition = new Vector3(gameplay.camera.cameraPosition.X + 320, 100f, gameplay.camera.cameraPosition.Y + 380);
            viewMatrix = Matrix.CreateLookAt(cameraPosition, new Vector3(cameraPosition.X, 0f, cameraPosition.Z - 100), new Vector3(0f, 0.5f, -0.5f));

            normalMappingEffect.Parameters["View"].SetValue(viewMatrix);
            normalMappingEffect.Parameters["EyePosition"].SetValue(cameraPosition);
            normalMappingEffect.CurrentTechnique = normalMappingEffect.Techniques["MultiPassLight"]; //geom has normal map (some sprites do not)

            
            lightList["camera"].PointLightPosition = new Vector3(cameraPosition.X + 10, 30f + 100, cameraPosition.Z - 120 + 300);
            lightList["character"].PointLightPosition = new Vector3(cameraPosition.X + 10, 30f, cameraPosition.Z - 120);

            DrawLayer(game, gameplay, cliffLayer);
            DrawLayer(game, gameplay, wallLayer);
            DrawLayer(game, gameplay, floorOuterLayer);
            //DrawLayer(game, gameplay, floorInnerLayer);
            DrawLayer(game, gameplay, wallSurfaceOuterLayer);
            //DrawLayer(game, gameplay, wallSurfaceInnerLayer);

            

            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            
            
        }

        private void SetEffectParameters()
        {
            normalMappingEffect.Parameters["World"].SetValue(Matrix.Identity);

            sceneDimensions = new Vector2(480 * game.aspectRatio, 480);
            normalMappingEffect.Parameters["Projection"].SetValue(Matrix.CreateOrthographic(480 * game.aspectRatio, 480, -200, 400));
            //normalMappingEffect.Parameters["Projection"].SetValue(Matrix.CreatePerspective(32, 16, 10, 1600));
            
            normalMappingEffect.Parameters["AmbientColor"].SetValue(new Vector4(1f, 1f, 1f, 1f));
            normalMappingEffect.Parameters["AmbientIntensity"].SetValue(0.115f);

            normalMappingEffect.Parameters["baseColor"].SetValue(new Vector4(0.02f, 0.02f, 0.05f, 1f));

        }

        
    }
}
