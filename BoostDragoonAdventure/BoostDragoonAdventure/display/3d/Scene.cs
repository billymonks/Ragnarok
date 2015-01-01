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
        //public List<WCVertex>[,] gridVertices;
        public List<WCVertex> solidGeomVertices;

        public Vector3 cameraPosition;
        public Vector3 cameraDirection;

        private DynamicVertexBuffer buffer;

        Effect normalMappingEffect;

        Matrix viewMatrix;

        private Game game;

        Vector2 sceneDimensions;

        SurfaceTileLayer floorOuterLayer, floorInnerLayer; // todo: group variable number of layers in a parent class
        SurfaceTileLayer wallOuterLayer, wallInnerLayer; // todo: group variable number of layers in a parent class

        

        public Scene(Game game)
        {
            this.game = game;
            solidGeomVertices = new List<WCVertex>();
            normalMappingEffect = game.Content.Load<Effect>(@"fx/NormalMappingMultiLights");
            
        }

        public void BuildScene(Game game, Map map)
        {
            bool[,] data = GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.DEATHSOUP].data, true);
            //bool[,] data = map.layerList[LayerType.DEATHSOUP].data;
            data = ScaleLayer(data, 2);
            data = InvertLayer(data);
            floorOuterLayer = new SurfaceTileLayer(game, data, 0, "pink_outer", true);
            data = ShrinkLayer(data, 1);
            floorInnerLayer = new SurfaceTileLayer(game, data, 0, "pink_inner", false);

            data = ScaleLayer(map.layerList[LayerType.WALL].data, 2);
            wallOuterLayer = new SurfaceTileLayer(game, data, 2, "pink_outer", true);
            wallInnerLayer = new SurfaceTileLayer(game, ShrinkLayer(data, 1), 2, "pink_inner", false);

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

        public void DrawLayer(Game game, GameplayManager gameplay, SurfaceTileLayer layer)
        {
            PrepareVertices(gameplay, layer);

            if (solidGeomVertices.Count <= 0)
                return;

            buffer = new DynamicVertexBuffer(game.GraphicsDevice, typeof(WCVertex), solidGeomVertices.Count, BufferUsage.WriteOnly);
            buffer.SetData(solidGeomVertices.ToArray());

            game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            normalMappingEffect.Parameters["ColorMap"].SetValue(layer.colorTexture);
            normalMappingEffect.Parameters["NormalMap"].SetValue(layer.normalTexture);

            game.GraphicsDevice.SetVertexBuffer(buffer);

            game.GraphicsDevice.BlendState = BlendState.Opaque;

            normalMappingEffect.CurrentTechnique.Passes["Ambient"].Apply();

            game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, solidGeomVertices.Count / 3);

            game.GraphicsDevice.BlendState = BlendState.Additive;

            normalMappingEffect.CurrentTechnique.Passes["Point"].Apply();
            game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, solidGeomVertices.Count / 3);

        }

        public void DrawScene(Game game, GameplayManager gameplay)
        {
            cameraPosition = new Vector3(gameplay.camera.cameraPosition.X + 320, 100f, gameplay.camera.cameraPosition.Y + 380);
            viewMatrix = Matrix.CreateLookAt(cameraPosition, new Vector3(cameraPosition.X, 0f, cameraPosition.Z - 100), new Vector3(0f, 0.5f, -0.5f));

            normalMappingEffect.Parameters["View"].SetValue(viewMatrix);
            normalMappingEffect.Parameters["EyePosition"].SetValue(cameraPosition);
            normalMappingEffect.CurrentTechnique = normalMappingEffect.Techniques["MultiPassLight"]; //geom has normal map (some sprites do not)

            normalMappingEffect.Parameters["DiffuseColor"].SetValue(new Vector4(0.7f, 0.75f, 0.9f, 1f));
            normalMappingEffect.Parameters["DiffuseIntensity"].SetValue(0.9f);
            normalMappingEffect.Parameters["SpecularColor"].SetValue(new Vector4(0.7f, 0.75f, 0.9f, 1f));
            normalMappingEffect.Parameters["SpecularIntensity"].SetValue(0f);
            normalMappingEffect.Parameters["PointLightPosition"].SetValue(new Vector3(cameraPosition.X + 10, 30f, cameraPosition.Z - 120));
            normalMappingEffect.Parameters["PointLightRange"].SetValue(500);

            DrawLayer(game, gameplay, floorOuterLayer);
            DrawLayer(game, gameplay, floorInnerLayer);
            DrawLayer(game, gameplay, wallOuterLayer);
            DrawLayer(game, gameplay, wallInnerLayer);

            //game.GraphicsDevice.BlendState = BlendState.Additive;

            //PrepareVertices(gameplay, outerLayer);

            //if (solidGeomVertices.Count <= 0)
                //return;

            
            
            //buffer = new DynamicVertexBuffer(game.GraphicsDevice, typeof(WCVertex), solidGeomVertices.Count, BufferUsage.WriteOnly);
            //buffer.SetData(solidGeomVertices.ToArray());

            //game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            //game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            

            

            


            

            

            

            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            
            
        }

        private void SetEffectParameters()
        {
            normalMappingEffect.Parameters["World"].SetValue(Matrix.Identity);

            sceneDimensions = new Vector2(480 * game.aspectRatio, 480);
            normalMappingEffect.Parameters["Projection"].SetValue(Matrix.CreateOrthographic(480 * game.aspectRatio, 480, -200, 400));
            //normalMappingEffect.Parameters["Projection"].SetValue(Matrix.CreatePerspective(1280, 720, 10, 800));
            
            normalMappingEffect.Parameters["AmbientColor"].SetValue(new Vector4(1f, 1f, 1f, 1f));
            normalMappingEffect.Parameters["AmbientIntensity"].SetValue(0.115f);

            normalMappingEffect.Parameters["baseColor"].SetValue(new Vector4(0f, 0f, 0f, 1f));

        }

        private void PrepareVertices(GameplayManager gameplay, SurfaceTileLayer layer)
        {
            int startX, startY, endX, endY;
            solidGeomVertices.Clear();

            /*startX = 0;
            startY = 0;
            endX = gridVertices.GetLength(0);
            endY = gridVertices.GetLength(1);*/

            startX = (int)cameraPosition.X / 10 - (int)(sceneDimensions.X / 20) - 1;
            startY = (int)cameraPosition.Z / 10 - (int)(sceneDimensions.Y / 20) - 20;
            endX = startX + (int)(sceneDimensions.X/10) + 2; // gridVertices.GetLength(0);
            endY = startY + (int)(sceneDimensions.Y/10) + 27; // gridVertices.GetLength(1);

            if (startX < 0)
                startX = 0;
            if (startY < 0)
                startY = 0;
            if (endX >= layer.gridVertices.GetLength(0))
            {
                endX = layer.gridVertices.GetLength(0) - 1;
            }
            if (endY >= layer.gridVertices.GetLength(1))
            {
                endY = layer.gridVertices.GetLength(1) - 1;
            }

            if (endX > layer.gridVertices.GetLength(0))
                endX = layer.gridVertices.GetLength(0);
            if (endY > layer.gridVertices.GetLength(1))
                endY = layer.gridVertices.GetLength(1);

            for (int i = startX; i < endX; i++)
                for (int j = endY; j >= startY; j--)
                    foreach (WCVertex v in layer.gridVertices[i, j])
                    {
                        solidGeomVertices.Add(v);
                    }

        }

        
    }
}
