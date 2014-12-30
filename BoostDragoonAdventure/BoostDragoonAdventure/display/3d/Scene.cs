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

        SurfaceTileLayer surfaceTileLayer;

        

        public Scene(Game game)
        {
            this.game = game;
            solidGeomVertices = new List<WCVertex>();
            normalMappingEffect = game.Content.Load<Effect>(@"fx/NormalMappingMultiLights");

            
            
            //normalTexture = game.whiteTexture;
            //normalTexture = game.Content.Load<Texture2D>(@"debugcontent/img/terribad_fwd_normal");
            
        }

        public void BuildScene(Game game, Map map)
        {
            surfaceTileLayer = new SurfaceTileLayer(game, 
                ScaleLayer(
                    InvertLayer(GetCompositeLayer(
                        map.layerList[LayerType.WALL].data, 
                        map.layerList[LayerType.DEATHSOUP].data, 
                        true)), 
                     2),
                 0);

            /*gridVertices = new List<WCVertex>[map.width / ART_GRID_SIZE, map.height / ART_GRID_SIZE];
            
            for(int i = 0; i < gridVertices.GetLength(0); i++)
            {
                for (int j = 0; j < gridVertices.GetLength(1); j++)
                {
                    gridVertices[i, j] = new List<WCVertex>();
                }
            }*/

            //AddGeometry(map, 3);

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
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (or)
                        a[i, j] = a[i, j] || b[i, j];
                    else
                        a[i, j] = a[i, j] && b[i, j];
                }
            }

            return a;
        }

        public bool[,] InvertLayer(bool[,] a)
        {
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    a[i, j] = !a[i, j];
                }
            }

            return a;
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

            normalMappingEffect.Parameters["ColorMap"].SetValue(surfaceTileLayer.colorTexture);
            normalMappingEffect.Parameters["NormalMap"].SetValue(surfaceTileLayer.normalTexture);

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
            normalMappingEffect.Parameters["PointLightPosition"].SetValue(new Vector3(cameraPosition.X, 30f, cameraPosition.Z - 100));
            normalMappingEffect.Parameters["PointLightRange"].SetValue(500);

            normalMappingEffect.CurrentTechnique.Passes["Point"].Apply();
            game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, solidGeomVertices.Count / 3);

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
            if (endX >= surfaceTileLayer.gridVertices.GetLength(0))
            {
                endX = surfaceTileLayer.gridVertices.GetLength(0) - 1;
            }
            if (endY >= surfaceTileLayer.gridVertices.GetLength(1))
            {
                endY = surfaceTileLayer.gridVertices.GetLength(1) - 1;
            }

            if (endX > surfaceTileLayer.gridVertices.GetLength(0))
                endX = surfaceTileLayer.gridVertices.GetLength(0);
            if (endY > surfaceTileLayer.gridVertices.GetLength(1))
                endY = surfaceTileLayer.gridVertices.GetLength(1);

            for (int i = startX; i < endX; i++)
                for (int j = endY; j >= startY; j--)
                    foreach (WCVertex v in surfaceTileLayer.gridVertices[i, j])
                    {
                        solidGeomVertices.Add(v);
                    }

        }

        /*private void AddGeometry(Map map, int wallHeight)
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
                            for (int k = 0; k < wallHeight; k++)
                            {
                                //AddWallVertices(map, i * 2, k, j * 2);
                                //AddWallVertices(map, i * 2 + 1, k, j * 2);
                            }
                        }


                    }
                    else if (map.getLayer(LayerType.DEATHSOUP).data[i, j] && j > 0 && !map.getLayer(LayerType.DEATHSOUP).data[i, j - 1])
                    {
                        //AddWallVertices(map, i * 2, -1, j * 2);
                        //AddWallVertices(map, i * 2 + 1, -1, j * 2);

                        if (map.getLayer(LayerType.WALL).data[i, j - 1])
                        {
                            for (int k = 0; k < wallHeight; k++)
                            {
                                //AddWallVertices(map, i * 2, k, j * 2);
                                //AddWallVertices(map, i * 2 + 1, k, j * 2);
                            }

                        }
                    }
                    else if (map.getLayer(LayerType.WALL).data[i, j])
                    {
                        //AddFloorVertices(map, i * 2, wallHeight, j * 2);
                        //AddFloorVertices(map, i * 2 + 1, wallHeight, j * 2);
                        //AddFloorVertices(map, i * 2, wallHeight, j * 2 + 1);
                        //AddFloorVertices(map, i * 2 + 1, wallHeight, j * 2 + 1);
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

            int quarter = x % 2 + 2 * (z % 2);

            Vector2 topRight = GetFloorCoordinate(map, x, z, new Vector2(0f, 1f), quarter);
            Vector2 bottomRight = GetFloorCoordinate(map, x, z, new Vector2(0f, 0f), quarter);
            Vector2 topLeft = GetFloorCoordinate(map, x, z, new Vector2(1f, 1f), quarter);
            Vector2 bottomLeft = GetFloorCoordinate(map, x, z, new Vector2(1f, 0f), quarter);

            gridVertices[x, z].Add(new WCVertex(
                new Vector4(x * ART_GRID_SIZE, y * ART_GRID_SIZE, z * ART_GRID_SIZE, 1),
                normal,
                bottomRight,
                bottomRight,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                new Vector4((x + 1) * ART_GRID_SIZE, y * ART_GRID_SIZE, z * ART_GRID_SIZE, 1),
                normal,
                bottomLeft,
                bottomLeft,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                new Vector4(x * ART_GRID_SIZE, y * ART_GRID_SIZE, (z + 1) * ART_GRID_SIZE, 1),
                normal,
                topRight,
                topRight,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                new Vector4((x + 1) * ART_GRID_SIZE, y * ART_GRID_SIZE, z * ART_GRID_SIZE, 1),
                normal,
                bottomLeft,
                bottomLeft,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                new Vector4((x + 1) * ART_GRID_SIZE, y * ART_GRID_SIZE, (z + 1) * ART_GRID_SIZE, 1),
                normal,
                topLeft,
                topLeft,
                tangent,
                binormal));

            gridVertices[x, z].Add(new WCVertex(
                new Vector4(x * ART_GRID_SIZE, y * ART_GRID_SIZE, (z + 1) * ART_GRID_SIZE, 1),
                normal,
                topRight,
                topRight,
                tangent,
                binormal));
        }

        private Vector2 GetFloorCoordinate(Map map, int x, int y, Vector2 coordinate, int quarter) //quarter = 0, 1, 2, 3
        {
            //int z = (int)(coordinate.X / 1 + 2 * coordinate.Y / 1);
            int floorTexInt = GetFloorTextureInt(map, x, y);
            int artFragment = GetArtFragment(floorTexInt, quarter);
            Vector2 coordFromFragment = ConvertFragmentToCoordinate(artFragment);
            Vector2 result = coordFromFragment + coordinate * new Vector2(1f / 3f, 1f / 3f);
            return result;
        }

        private Vector2 ConvertFragmentToCoordinate(int num)
        {
            int i = (num % 3);
            int j = (num / 3);
            Vector2 result = new Vector2((1f/3f)*i, (1f/3f)*j);
            return result;
        }

        private int GetArtFragment(int input, int quarter)
        {
            byte[,] fragmentSet = {{0, 2, 6, 8},
                                  {0, 1, 6, 7},
                                  {0, 2, 3, 5},
                                  {0, 1, 3, 4},
                                  {1, 2, 7, 8},
                                  {1, 1, 7, 7},
                                  {1, 2, 4, 5},
                                  {1, 1, 4, 4},
                                  {3, 5, 6, 8},
                                  {3, 4, 6, 7},
                                  {3, 5, 3, 5},
                                  {3, 4, 3, 4},
                                  {4, 5, 7, 8},
                                  {4, 4, 7, 7},
                                  {4, 5, 4, 5},
                                  {4, 4, 4, 4}};

            return fragmentSet[input, quarter];
        }

        private int GetFloorTextureInt(Map map, int x, int y)
        {
            int result = 0;

            bool[,] data = new bool[map.getLayer(LayerType.DEATHSOUP).data.GetLength(0), map.getLayer(LayerType.DEATHSOUP).data.GetLength(1)];

            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    if (map.getLayer(LayerType.DEATHSOUP).data[i, j] || map.getLayer(LayerType.WALL).data[i, j])
                        data[i, j] = true;
                    else data[i, j] = false;

            x /= 2;
            y /= 2;

            if (data[x, y])
            {
                return 4;
            }

            if (x > 0 && !data[x - 1, y])
            {
                result += 4;
            }
            if (y > 0 && !data[x, y - 1])
            {
                result += 8;
            }
            if (x < data.GetLength(0)-1 && !data[x + 1, y])
            {
                result ++;
            }
            if (y < data.GetLength(1)-1 && !data[x, y + 1])
            {
                result += 2;
            }

            return result;
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
        }*/
    }
}
