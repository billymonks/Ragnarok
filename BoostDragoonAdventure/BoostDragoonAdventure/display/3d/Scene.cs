using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.display._3d.vertex;
using wickedcrush.map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.manager.gameplay;

namespace wickedcrush.display._3d
{
    public class Scene
    {
        const int ART_GRID_SIZE = 10;
        public List<WCVertex>[,] gridVertices;
        public List<WCVertex> solidGeomVertices;

        public Vector3 cameraPosition;
        public Vector3 cameraDirection;

        private DynamicVertexBuffer buffer;

        Effect normalMappingEffect;

        Matrix viewMatrix;

        private Game game;

        Vector2 sceneDimensions;

        Texture2D normalTexture;

        public Scene(Game game)
        {
            this.game = game;
            solidGeomVertices = new List<WCVertex>();
            normalMappingEffect = game.Content.Load<Effect>(@"fx/NormalMappingMultiLights");
            normalTexture = game.Content.Load<Texture2D>(@"debugcontent/img/terribad_normal");
            //normalTexture = game.Content.Load<Texture2D>(@"debugcontent/img/terribad_fwd_normal");
            
        }

        public void BuildScene(Map map)
        {
            gridVertices = new List<WCVertex>[map.width / ART_GRID_SIZE, map.height / ART_GRID_SIZE];
            
            for(int i = 0; i < gridVertices.GetLength(0); i++)
            {
                for (int j = 0; j < gridVertices.GetLength(1); j++)
                {
                    gridVertices[i, j] = new List<WCVertex>();
                }
            }

            AddGeometry(map);

            SetEffectParameters();
            
        }

        public void DrawScene(Game game, GameplayManager gameplay)
        {
            //cameraPosition = new Vector3(gameplay.camera.cameraPosition.X + 320, -100f, -gameplay.camera.cameraPosition.Y - 240); 
            cameraPosition = new Vector3(gameplay.camera.cameraPosition.X + 320, 100f, gameplay.camera.cameraPosition.Y + 380); 
            PrepareVertices(gameplay);

            if (solidGeomVertices.Count <= 0)
                return;

            viewMatrix = Matrix.CreateLookAt(cameraPosition, new Vector3(cameraPosition.X, 0f, cameraPosition.Z - 100), new Vector3(0f, 0.5f, -0.5f));
            
            buffer = new DynamicVertexBuffer(game.GraphicsDevice, typeof(WCVertex), solidGeomVertices.Count, BufferUsage.WriteOnly);
            buffer.SetData(solidGeomVertices.ToArray());

            game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            

            normalMappingEffect.Parameters["View"].SetValue(viewMatrix);
            normalMappingEffect.Parameters["EyePosition"].SetValue(cameraPosition);

            normalMappingEffect.Parameters["ColorMap"].SetValue(game.whiteTexture);
            normalMappingEffect.Parameters["NormalMap"].SetValue(normalTexture);

            normalMappingEffect.Parameters["SpecularIntensity"].SetValue(1f);
            normalMappingEffect.Parameters["baseColor"].SetValue(new Vector4(0f,0f,0f,1f));

            normalMappingEffect.CurrentTechnique = normalMappingEffect.Techniques["MultiPassLight"]; //geom has normal map (some sprites do not)


            game.GraphicsDevice.SetVertexBuffer(buffer);

            game.GraphicsDevice.BlendState = BlendState.Opaque;

            normalMappingEffect.CurrentTechnique.Passes["Ambient"].Apply();

            game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, solidGeomVertices.Count / 3);

            game.GraphicsDevice.BlendState = BlendState.Additive;

            normalMappingEffect.Parameters["DiffuseColor"].SetValue(new Vector4(0.7f, 0.75f, 0.9f, 1f));
            normalMappingEffect.Parameters["DiffuseIntensity"].SetValue(0.5f);
            normalMappingEffect.Parameters["SpecularColor"].SetValue(new Vector4(0.4f, 0.9f, 0.6f, 1f));
            normalMappingEffect.Parameters["PointLightPosition"].SetValue(new Vector3(cameraPosition.X, 100f, cameraPosition.Z - 100));
            normalMappingEffect.Parameters["PointLightRange"].SetValue(1000);

            normalMappingEffect.CurrentTechnique.Passes["Point"].Apply();
            game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, solidGeomVertices.Count / 3);

            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            
            
        }

        private void SetEffectParameters()
        {
            normalMappingEffect.Parameters["World"].SetValue(Matrix.Identity);

            sceneDimensions = new Vector2(480 * game.aspectRatio, 480);
            normalMappingEffect.Parameters["Projection"].SetValue(Matrix.CreateOrthographic(480 * game.aspectRatio, 480, -800, 800));
            
            normalMappingEffect.Parameters["AmbientColor"].SetValue(new Vector4(1f, 1f, 1f, 1f));
            normalMappingEffect.Parameters["AmbientIntensity"].SetValue(0.02f);



        }

        private void PrepareVertices(GameplayManager gameplay)
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
            if (endX >= gridVertices.GetLength(0))
            {
                endX = gridVertices.GetLength(0)-1;
            }
            if (endY >= gridVertices.GetLength(1))
            {
                endY = gridVertices.GetLength(1)-1;
            }

            if (endX > gridVertices.GetLength(0))
                endX = gridVertices.GetLength(0);
            if (endY > gridVertices.GetLength(1))
                endY = gridVertices.GetLength(1);

            for (int i = startX; i < endX; i++)
                for (int j = endY; j >= startY; j--)
                    foreach (WCVertex v in gridVertices[i, j])
                    {
                        solidGeomVertices.Add(v);
                    }

        }

        private void AddGeometry(Map map)
        {
            for (int i = 0; i < map.getLayer(LayerType.WALL).data.GetLength(0); i++)
            {
                for (int j = 0; j < map.getLayer(LayerType.WALL).data.GetLength(1); j++)
                {
                    if (!map.getLayer(LayerType.WALL).data[i, j] && !map.getLayer(LayerType.DEATHSOUP).data[i, j])
                    {
                        AddFloorVertices(map, i * 2, 0, j * 2);
                        AddFloorVertices(map, i * 2 + 1, 0, j * 2);
                        AddFloorVertices(map, i * 2, 0, j * 2 + 1);
                        AddFloorVertices(map, i * 2 + 1, 0, j * 2 + 1);

                        if ( j > 0 && map.getLayer(LayerType.WALL).data[i, j - 1])
                        {
                            AddWallVertices(map, i * 2, 0, j * 2);
                            AddWallVertices(map, i * 2 + 1, 0, j * 2);
                            AddWallVertices(map, i * 2, 1, j * 2);
                            AddWallVertices(map, i * 2 + 1, 1, j * 2);
                            AddWallVertices(map, i * 2, 2, j * 2);
                            AddWallVertices(map, i * 2 + 1, 2, j * 2);
                            AddWallVertices(map, i * 2, 3, j * 2);
                            AddWallVertices(map, i * 2 + 1, 3, j * 2);
                            AddWallVertices(map, i * 2, 4, j * 2);
                            AddWallVertices(map, i * 2 + 1, 4, j * 2);
                            AddWallVertices(map, i * 2, 5, j * 2);
                            AddWallVertices(map, i * 2 + 1, 5, j * 2);
                        }


                    }
                    else if (map.getLayer(LayerType.DEATHSOUP).data[i, j] && j > 0 && !map.getLayer(LayerType.DEATHSOUP).data[i, j - 1])
                    {
                        AddWallVertices(map, i * 2, -1, j * 2);
                        AddWallVertices(map, i * 2 + 1, -1, j * 2);
                    }
                    else if (map.getLayer(LayerType.WALL).data[i, j])
                    {
                        AddFloorVertices(map, i * 2, 6, j * 2);
                        AddFloorVertices(map, i * 2 + 1, 6, j * 2);
                        AddFloorVertices(map, i * 2, 6, j * 2 + 1);
                        AddFloorVertices(map, i * 2 + 1, 6, j * 2 + 1);
                    }
                    
                }
            }
        }

        private void AddWallVertices(Map map, int x, int y, int z)
        {
            Vector3 normal = Vector3.Backward;
            Vector3 tangent = Vector3.Up;
            Vector3 binormal = Vector3.Cross(tangent, normal);

            Vector2 topRight = new Vector2(1f, 0f);
            Vector2 bottomRight = new Vector2(1f, 1f);
            Vector2 topLeft = new Vector2(0f, 0f);
            Vector2 bottomLeft = new Vector2(0f, 1f);

            Vector4 backBottomLeft = new Vector4(x * ART_GRID_SIZE, y * ART_GRID_SIZE, (z) * ART_GRID_SIZE, 1);
            Vector4 backBottomRight = new Vector4((x + 1) * ART_GRID_SIZE, y * ART_GRID_SIZE, (z) * ART_GRID_SIZE, 1);
            Vector4 backTopLeft = new Vector4(x * ART_GRID_SIZE, (y + 1) * ART_GRID_SIZE, (z) * ART_GRID_SIZE, 1);
            Vector4 backTopRight = new Vector4((x + 1) * ART_GRID_SIZE, (y + 1) * ART_GRID_SIZE, (z) * ART_GRID_SIZE, 1);

            gridVertices[x, z].Add(new WCVertex(
                backBottomLeft,
                normal,
                Vector2.Zero,
                bottomLeft,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                backTopLeft,
                normal,
                Vector2.Zero,
                topLeft,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                backBottomRight,
                normal,
                Vector2.Zero,
                bottomRight,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                backBottomRight,
                normal,
                Vector2.Zero,
                bottomRight,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                backTopLeft,
                normal,
                Vector2.Zero,
                topLeft,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                backTopRight,
                normal,
                Vector2.Zero,
                topRight,
                tangent,
                binormal));



        }

        private void AddFloorVertices(Map map, int x, int y, int z)
        {
            Vector3 normal = Vector3.Up;
            Vector3 tangent = Vector3.Forward;
            Vector3 binormal = Vector3.Cross(tangent, normal);

            Vector2 topRight = new Vector2(0f, 1f);
            Vector2 bottomRight = new Vector2(0f, 0f);
            Vector2 topLeft = new Vector2(1f, 1f);
            Vector2 bottomLeft = new Vector2(1f, 0f);

            gridVertices[x, z].Add(new WCVertex(
                new Vector4(x * ART_GRID_SIZE, y * ART_GRID_SIZE, z * ART_GRID_SIZE, 1),
                normal,
                Vector2.Zero,
                bottomRight,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                new Vector4((x + 1) * ART_GRID_SIZE, y * ART_GRID_SIZE, z * ART_GRID_SIZE, 1),
                normal,
                Vector2.Zero,
                bottomLeft,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                new Vector4(x * ART_GRID_SIZE, y * ART_GRID_SIZE, (z + 1) * ART_GRID_SIZE, 1),
                normal,
                Vector2.Zero,
                topRight,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                new Vector4((x + 1) * ART_GRID_SIZE, y * ART_GRID_SIZE, z * ART_GRID_SIZE, 1),
                normal,
                Vector2.Zero,
                bottomLeft,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                new Vector4((x + 1) * ART_GRID_SIZE, y * ART_GRID_SIZE, (z + 1) * ART_GRID_SIZE, 1),
                normal,
                Vector2.Zero,
                topLeft,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                new Vector4(x * ART_GRID_SIZE, y * ART_GRID_SIZE, (z + 1) * ART_GRID_SIZE, 1),
                normal,
                Vector2.Zero,
                topRight,
                tangent,
                binormal));
        }

        private void clearGridVerts()
        {
            for (int i = 0; i < gridVertices.GetLength(0); i++)
            {
                for (int j = 0; j < gridVertices.GetLength(1); j++)
                {
                    gridVertices[i, j].Clear();
                }
            }
        }
    }
}
