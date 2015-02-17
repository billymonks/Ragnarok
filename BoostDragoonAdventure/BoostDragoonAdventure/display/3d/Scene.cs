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

        List<ArtLayer> artLayers = new List<ArtLayer>();

        public Dictionary<string, PointLightStruct> lightList = new Dictionary<string,PointLightStruct>();

        public Scene(GameBase game)
        {
            this.game = game;
            
            normalMappingEffect = game.Content.Load<Effect>(@"fx/NormalMappingMultiLights");
            
        }

        public void BuildScene(GameBase game, Map map)
        {
            artLayers.Add(new ArtLayer(game, LayerTransformations.InvertLayer(LayerTransformations.ScaleLayer(map.layerList[LayerType.DEATHSOUP].data, 2)), -1, 0, "cavefloor"));
            artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(map.layerList[LayerType.WALL].data, 2), 0, 2, "cavewall"));
            

            SetEffectParameters();

            
            lightList.Add("camera", new PointLightStruct(new Vector4(0.7f, 0.75f, 0.9f, 1f), 0.6f, new Vector4(0.7f, 0.75f, 0.9f, 1f), 0f, new Vector3(cameraPosition.X + 10, 30f + 100, cameraPosition.Z - 120 + 300), 1000f));
            lightList.Add("character", new PointLightStruct(new Vector4(1f, 0.65f, 0.5f, 1f), 0.9f, new Vector4(1f, 0.65f, 0.5f, 1f), 1f, new Vector3(cameraPosition.X + 10, 30f, cameraPosition.Z - 120), 1000f));
            lightList.Add("character2", new PointLightStruct(new Vector4(0.5f, 0.85f, 0.5f, 1f), 0.9f, new Vector4(1f, 0.65f, 0.5f, 1f), 0f, new Vector3(100f, 30f, 100f), 1000f));
            lightList.Add("character3", new PointLightStruct(new Vector4(0.5f, 0.85f, 0.5f, 1f), 0.9f, new Vector4(1f, 0.65f, 0.5f, 1f), 0f, new Vector3(500f, 30f, 500f), 1000f));
            lightList.Add("character4", new PointLightStruct(new Vector4(0.85f, 0.5f, 0.85f, 1f), 0.9f, new Vector4(1f, 0.65f, 0.5f, 1f), 0f, new Vector3(100f, 30f, 500f), 1000f));
            lightList.Add("character5", new PointLightStruct(new Vector4(0.85f, 0.5f, 0.85f, 1f), 0.9f, new Vector4(1f, 0.65f, 0.5f, 1f), 0f, new Vector3(1000f, 30f, 500f), 1000f));
            lightList.Add("character6", new PointLightStruct(new Vector4(0.5f, 0.85f, 0.5f, 1f), 0.9f, new Vector4(1f, 0.65f, 0.5f, 1f), 0f, new Vector3(1500f, 30f, 1000f), 1000f));
            
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

            foreach (ArtLayer artLayer in artLayers)
            {
                artLayer.DrawLayer(game, gameplay, normalMappingEffect, lightList);
            }

            

            game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            
            
        }

        private void SetEffectParameters()
        {
            normalMappingEffect.Parameters["World"].SetValue(Matrix.Identity);

            sceneDimensions = new Vector2(480 * game.aspectRatio, 480);
            normalMappingEffect.Parameters["Projection"].SetValue(Matrix.CreateOrthographic(480 * game.aspectRatio, 480, -200, 400));
            //normalMappingEffect.Parameters["Projection"].SetValue(Matrix.CreatePerspective(32, 16, 10, 1600));
            
            normalMappingEffect.Parameters["AmbientColor"].SetValue(new Vector4(1f, 1f, 1f, 1f));
            normalMappingEffect.Parameters["AmbientIntensity"].SetValue(0.015f);

            normalMappingEffect.Parameters["baseColor"].SetValue(new Vector4(0.02f, 0.02f, 0.05f, 1f));

        }

        
    }
}
