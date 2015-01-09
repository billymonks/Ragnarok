using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.display._3d.texture;
using wickedcrush.display._3d.vertex;
using Microsoft.Xna.Framework;

namespace wickedcrush.display._3d
{
    public class SurfaceTileLayer : TileLayer
    {

        bool edgeTilesOnly;

        public SurfaceTileLayer(Game game, bool[,] data, int height, String tilesetPath, bool edgeOnly) : base(game, data, height, tilesetPath)
        {
            edgeTilesOnly = edgeOnly;

            BuildScene(game);
        }

        protected override void AddGeometry()
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (data[i, j])
                    {
                        AddFloorVertices(i, height, j);
                    }
                }
            }
        }

        private void AddFloorVertices(int x, int y, int z)
        {
            Vector3 normal = Vector3.Up;
            Vector3 tangent = Vector3.Forward;
            Vector3 binormal = Vector3.Cross(tangent, normal);

            int quarter = x % 2 + 2 * (z % 2);

            int floorTexture = GetFloorTextureInt(x, z);

            if (floorTexture == 4 && edgeTilesOnly)
            {
                return;
            }

            Vector2 topRight = GetFloorCoordinate(floorTexture, new Vector2(0f, 1f));
            Vector2 bottomRight = GetFloorCoordinate(floorTexture, new Vector2(0f, 0f));
            Vector2 topLeft = GetFloorCoordinate(floorTexture, new Vector2(1f, 1f));
            Vector2 bottomLeft = GetFloorCoordinate(floorTexture, new Vector2(1f, 0f));

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

        private Vector2 GetFloorCoordinate(int floorTexInt, Vector2 coordinate)
        {
            Vector2 coordFromFragment = ConvertTextureIntToCoordinate(floorTexInt);
            Vector2 result = coordFromFragment + coordinate * new Vector2(1f / 3f, 1f / 3f);
            return result;
        }

        private Vector2 ConvertTextureIntToCoordinate(int num)
        {
            int i = (num % 3);
            int j = (num / 3);
            Vector2 result = new Vector2((1f / 3f) * i, (1f / 3f) * j);
            return result;
        }

        private int GetFloorTextureInt(int x, int y)
        {
            int result = 4;

            if (x > 0 && !data[x - 1, y])
                result--;
            else if (x < data.GetLength(0) - 1 && !data[x + 1, y])
                result++;

            if (y > 0 && !data[x, y - 1])
                result -= 3;
            else if (y < data.GetLength(1) - 1 && !data[x, y + 1])
                result += 3;

            return result;
        }

    }
}
