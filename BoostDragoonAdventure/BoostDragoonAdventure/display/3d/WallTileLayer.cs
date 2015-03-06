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
    public class WallTileLayer : TileLayer
    {

        int baseHeight; //height = height of top, baseheight = height of base, aka baseheight = 3, height = 4 means 1 grid high

        public WallTileLayer(GameBase game, bool[,] data, int height, int baseHeight, String tilesetPath)
            : base(game, data, height, tilesetPath)
        {
            this.baseHeight = baseHeight;
            BuildScene(game);
            
        }

        protected override void AddGeometry()
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (data[i, j] && (j == data.GetLength(1)-1 || !data[i, j+1]))
                    {
                        for(int k = baseHeight; k < height; k++)
                            AddWallVertices(i, k, j);
                    }
                }
            }
        }

        private void AddWallVertices(int x, int y, int z)
        {
            Vector3 normal = Vector3.Backward;
            Vector3 tangent = Vector3.Up;
            Vector3 binormal = Vector3.Cross(tangent, normal);

            int quarter = x % 2 + 2 * (z % 2);

            int floorTexture = GetWallTextureInt(x, y, z);

            Vector2 topRightTexCoord = GetWallCoordinate(floorTexture, new Vector2(0.03f, 0.97f));
            Vector2 bottomRightTexCoord = GetWallCoordinate(floorTexture, new Vector2(0.03f, 0.03f));
            Vector2 topLeftTexCoord = GetWallCoordinate(floorTexture, new Vector2(0.97f, 0.97f));
            Vector2 bottomLeftTexCoord = GetWallCoordinate(floorTexture, new Vector2(0.97f, 0.03f));

            Vector4 topRightVertPos = new Vector4((x + 0) * ART_GRID_SIZE, (y + 0) * ART_GRID_SIZE, (z + 1) * ART_GRID_SIZE, 1);
            Vector4 bottomRightVertPos = new Vector4((x + 0) * ART_GRID_SIZE, (y + 1) * ART_GRID_SIZE, (z + 1) * ART_GRID_SIZE, 1);
            Vector4 topLeftVertPos = new Vector4((x + 1) * ART_GRID_SIZE, (y + 0) * ART_GRID_SIZE, (z + 1) * ART_GRID_SIZE, 1);
            Vector4 bottomLeftVertPos = new Vector4((x + 1) * ART_GRID_SIZE, (y + 1) * ART_GRID_SIZE, (z + 1) * ART_GRID_SIZE, 1);

            solidGeomVertices.Add(new WCVertex(
                bottomRightVertPos,
                normal,
                bottomRightTexCoord,
                bottomRightTexCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                bottomLeftVertPos,
                normal,
                bottomLeftTexCoord,
                bottomLeftTexCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                topRightVertPos,
                normal,
                topRightTexCoord,
                topRightTexCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                bottomLeftVertPos,
                normal,
                bottomLeftTexCoord,
                bottomLeftTexCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                topLeftVertPos,
                normal,
                topLeftTexCoord,
                topLeftTexCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                topRightVertPos,
                normal,
                topRightTexCoord,
                topRightTexCoord,
                tangent,
                binormal));
        }

        private Vector2 GetWallCoordinate(int floorTexInt, Vector2 coordinate)
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

        private int GetWallTextureInt(int x, int y, int z)
        {
            int result = 4;

            if (x > 0 && !data[x - 1, z])
                result--;
            
            if (x < data.GetLength(0) - 1 && !data[x + 1, z])
                result++;

            if (y == baseHeight)
                result += 3;
            
            if (y == height-1)
                result -= 3;

            return result;
        }
    }
}
