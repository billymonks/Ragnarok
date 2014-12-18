using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace wickedcrush.display._3d.vertex
{
    public struct WCVertex : IVertexType
    {
        #region fields
        Vector4 vPosition;
        Vector3 vNormal;
        Vector2 vTextureCoordinate;
        Vector2 vNormalCoordinate;
        Vector3 vTangent;
        Vector3 vBinormal;
        #endregion

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * 7, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 9, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(sizeof(float) * 11, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
            new VertexElement(sizeof(float) * 14, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0)
        );
        public WCVertex(Vector4 position, Vector3 normal, Vector2 textureCoordinate, Vector2 normalCoordinate, Vector3 tangent, Vector3 binormal)
        {
            vPosition = position;
            vNormal = normal;
            vTextureCoordinate = textureCoordinate;
            vNormalCoordinate = normalCoordinate;
            vTangent = tangent;
            vBinormal = binormal;
        }
        public Vector4 Position
        {
            get { return vPosition; }
            set { vPosition = value; }
        }
        public Vector3 Normal
        {
            get { return vNormal; }
            set { vNormal = value; }
        }
        public Vector2 TextureCoordinate
        {
            get { return vTextureCoordinate; }
            set { vTextureCoordinate = value; }
        }
        public Vector2 NormalCoordinate
        {
            get { return vNormalCoordinate; }
            set { vNormalCoordinate = value; }
        }
        public Vector3 Tangent
        {
            get { return vTangent; }
            set { vTangent = value; }
        }
        public Vector3 Binormal
        {
            get { return vBinormal; }
            set { vBinormal = value; }
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}
