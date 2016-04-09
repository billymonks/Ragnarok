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
using wickedcrush.display._3d.atlas;
using System.IO;
using wickedcrush.manager.map;

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

        public Effect normalMappingEffect, parallaxEffect, spriteEffect;

        Matrix viewMatrix;

        private GameBase game;

        Vector2 sceneDimensions;

        List<ArtLayer> artLayers = new List<ArtLayer>();

        public Dictionary<string, PointLightStruct> lightDictionary = new Dictionary<string,PointLightStruct>();

        public List<PointLightStruct> lightList = new List<PointLightStruct>();

        //combined scene texture and normal
        public Texture2D colorTexture;
        public Texture2D normalTexture;

        public List<WCVertex> solidGeomVertices;
        public VertexBuffer buffer;

        public List<VertexPositionColor> parallaxVertices;
        public VertexBuffer parallaxBuffer;

        TextureAtlas textureAtlas;

        public Texture2D background;

        public bool hqLight = false;

        public Scene(GameBase game)
        {
            this.game = game;
            
            normalMappingEffect = game.Content.Load<Effect>(@"fx/NormalMappingMultiLights");
            parallaxEffect = game.Content.Load<Effect>(@"fx/ParallaxEffect");
            spriteEffect = game.Content.Load<Effect>("fx/SpriteEffect");
            background = game.Content.Load<Texture2D>(@"img/tex/rock_normal2");

            hqLight = game.settings.hqLight;
            
        }

        public void AddLight(PointLightStruct light)
        {
            if (hqLight)
                lightList.Add(light);
        }

        public void RemoveLight(PointLightStruct light)
        {
            if (hqLight)
                lightList.Remove(light);
        }

        public void BuildScene(GameBase game, Map map, GameplayManager gameplay, MapStats mapStats)
        {
            ThemeAtlas.PopulateArtLayers(game, map, out artLayers, mapStats.theme);

            if(hqLight)
                ThemeAtlas.PopulateScene(mapStats.theme, this);

            ThemeAtlas.PopulateBackgrounds(game, map, gameplay, mapStats);

            if (gameplay._screen.bgs.Count > 0)
            {
                background = gameplay._screen.bgs.Dequeue();
                gameplay._screen.bgs.Enqueue(background);
            }

            CreateTextureAtlas(game, map);
            CombineVertices(map);

            //CreateParallaxVertices(map);

            SetEffectParameters(gameplay);


            //lightDictionary.Add("camera", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.06f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, new Vector3(cameraPosition.X + 10, 30f + 100, cameraPosition.Z - 120 + 300), 3000f));
            //lightDictionary.Add("camera2", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.05f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, new Vector3(cameraPosition.X + 10, 30f + 100, cameraPosition.Z - 120 + 300), 3000f));
            
            
        }

        protected void CreateParallaxVertices(Map map)
        {

            

            parallaxVertices = new List<VertexPositionColor>();

            bool[,] data = LayerTransformations.ScaleLayer(map.getLayer(LayerType.DEATHSOUP).data, 2);

            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (data[i, j])
                    {
                        AddParallaxVertices(i, 0, j);
                    }
                }
            }

            if (parallaxVertices.Count == 0)
                return;

            parallaxBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionColor), parallaxVertices.Count, BufferUsage.WriteOnly);
            parallaxBuffer.SetData(parallaxVertices.ToArray());
        }

        private void AddParallaxVertices(int x, int y, int z)
        {
            parallaxVertices.Add(new VertexPositionColor(
                new Vector3(x * ART_GRID_SIZE, y * ART_GRID_SIZE, z * ART_GRID_SIZE),
                Color.White));

            parallaxVertices.Add(new VertexPositionColor(
                new Vector3((x + 1) * ART_GRID_SIZE, y * ART_GRID_SIZE, z * ART_GRID_SIZE),
                Color.White));

            parallaxVertices.Add(new VertexPositionColor(
                new Vector3(x * ART_GRID_SIZE, y * ART_GRID_SIZE, (z + 1) * ART_GRID_SIZE),
                Color.White));

            parallaxVertices.Add(new VertexPositionColor(
                new Vector3((x + 1) * ART_GRID_SIZE, y * ART_GRID_SIZE, z * ART_GRID_SIZE),
                Color.White));

            parallaxVertices.Add(new VertexPositionColor(
                new Vector3((x + 1) * ART_GRID_SIZE, y * ART_GRID_SIZE, (z + 1) * ART_GRID_SIZE),
                Color.White));

            parallaxVertices.Add(new VertexPositionColor(
                new Vector3(x * ART_GRID_SIZE, y * ART_GRID_SIZE, (z + 1) * ART_GRID_SIZE),
                Color.White));
        }

        protected void CreateTextureAtlas(GameBase game, Map map)
        {

            Dictionary<String, Texture2D> textures = new Dictionary<String, Texture2D>();
            Dictionary<String, Rectangle> tempAtlas = new Dictionary<String, Rectangle>();

            foreach (ArtLayer artLayer in artLayers)
            {
                foreach (TileLayer tileLayer in artLayer.tileLayers)
                {
                    if(!textures.ContainsKey(tileLayer.tileset.tex))
                        textures.Add(tileLayer.tileset.tex, tileLayer.colorTexture);
                    if (!textures.ContainsKey(tileLayer.tileset.normal))
                        textures.Add(tileLayer.tileset.normal, tileLayer.normalTexture);
                }
            }

            foreach (ArtTile artTile in map.artTileList)
            {
                if (!textures.ContainsKey(artTile.color))
                    textures.Add(artTile.color, game.Content.Load<Texture2D>(@"img/tex/big/"+artTile.color));
                if (!textures.ContainsKey(artTile.normal))
                    textures.Add(artTile.normal, game.Content.Load<Texture2D>(@"img/tex/big/" + artTile.normal ));
            }

            foreach (TileCoord tileCoord in map.tileCoordList)
            {
                if (!textures.ContainsKey(tileCoord.color))
                    textures.Add(tileCoord.color, game.Content.Load<Texture2D>(@"img/tile/" + tileCoord.color));
                if (!textures.ContainsKey(tileCoord.normal))
                    textures.Add(tileCoord.normal, game.Content.Load<Texture2D>(@"img/tile/" + tileCoord.normal));
            }

            textureAtlas = new TextureAtlas(textures, game.GraphicsDevice);
            solidGeomVertices = new List<WCVertex>();

        }

        protected void CombineVertices(Map map)
        {
            foreach (ArtLayer artLayer in artLayers)
            {
                foreach (TileLayer tileLayer in artLayer.tileLayers)
                {
                    foreach (WCVertex vert in tileLayer.solidGeomVertices)
                    {
                        WCVertex tempVert = new WCVertex(vert.Position, vert.Normal,
                            textureAtlas.GetConvertedCoordinate(tileLayer.tileset.tex, vert.TextureCoordinate),
                            textureAtlas.GetConvertedCoordinate(tileLayer.tileset.normal, vert.NormalCoordinate),
                            vert.Tangent, vert.Binormal);

                        solidGeomVertices.Add(tempVert);

                    }
                    tileLayer.solidGeomVertices.Clear();
                }
            }

            foreach (ArtTile artTile in map.artTileList)
            {
                AddArtTile(artTile);
            }

            foreach (TileCoord tileCoord in map.tileCoordList)
            {
                AddTileCoord(tileCoord);
            }

            buffer = new VertexBuffer(game.GraphicsDevice, typeof(WCVertex), solidGeomVertices.Count, BufferUsage.WriteOnly);
            buffer.SetData(solidGeomVertices.ToArray());
        }

        private void AddTileCoord(TileCoord tileCoord)
        {
            Vector3 normal = Vector3.Up;
            Vector3 tangent = Vector3.Forward;
            Vector3 binormal = Vector3.Cross(tangent, normal);

            float padding = 0.03f;

            //tileCoord.pos;
            //tileCoord.tpos;

            Vector2 topRightTexCoord = textureAtlas.GetConvertedTileCoord(tileCoord.color, new Vector2((tileCoord.tpos.X + 0f) * 20f + padding, (tileCoord.tpos.Y + 1f) * 20f - padding));
            Vector2 bottomRightTexCoord = textureAtlas.GetConvertedTileCoord(tileCoord.color, new Vector2((tileCoord.tpos.X + 0f) * 20f + padding, (tileCoord.tpos.Y + 0f) * 20f + padding));
            Vector2 topLeftTexCoord = textureAtlas.GetConvertedTileCoord(tileCoord.color, new Vector2((tileCoord.tpos.X + 1f) * 20f - padding, (tileCoord.tpos.Y + 1f) * 20f - padding));
            Vector2 bottomLeftTexCoord = textureAtlas.GetConvertedTileCoord(tileCoord.color, new Vector2((tileCoord.tpos.X + 1f) * 20f - padding, (tileCoord.tpos.Y + 0f) * 20f + padding));

            Vector2 topRightNormalCoord = textureAtlas.GetConvertedTileCoord(tileCoord.normal, new Vector2((tileCoord.tpos.X + 0f) * 20f + padding, (tileCoord.tpos.Y + 1f) * 20f - padding));
            Vector2 bottomRightNormalCoord = textureAtlas.GetConvertedTileCoord(tileCoord.normal, new Vector2((tileCoord.tpos.X + 0f) * 20f + padding, (tileCoord.tpos.Y + 0f) * 20f + padding));
            Vector2 topLeftNormalCoord = textureAtlas.GetConvertedTileCoord(tileCoord.normal, new Vector2((tileCoord.tpos.X + 1f) * 20f - padding, (tileCoord.tpos.Y + 1f) * 20f - padding));
            Vector2 bottomLeftNormalCoord = textureAtlas.GetConvertedTileCoord(tileCoord.normal, new Vector2((tileCoord.tpos.X + 1f) * 20f - padding, (tileCoord.tpos.Y + 0f) * 20f + padding));

            Vector4 topRightVertPos = new Vector4((tileCoord.pos.X + 0f) * 20f, padding + tileCoord.layer * 20f, (tileCoord.pos.Y + 1f) * 20f + tileCoord.layer * 20f, 1);
            Vector4 bottomRightVertPos = new Vector4((tileCoord.pos.X + 0f) * 20f, padding + tileCoord.layer * 20f, (tileCoord.pos.Y + 0f) * 20f + tileCoord.layer * 20f, 1);
            Vector4 topLeftVertPos = new Vector4((tileCoord.pos.X + 1f) * 20f, padding + tileCoord.layer * 20f, (tileCoord.pos.Y + 1f) * 20f + tileCoord.layer * 20f, 1);
            Vector4 bottomLeftVertPos = new Vector4((tileCoord.pos.X + 1f) * 20f, padding + tileCoord.layer * 20f, (tileCoord.pos.Y + 0f) * 20f + tileCoord.layer * 20f, 1);

            //Vector4 topRightVertPos = new Vector4((artTile.pos.X + 0), (artTile.height + 0), (artTile.pos.Y + artTile.size.Y + padding - 10), 1);
            //Vector4 bottomRightVertPos = new Vector4((artTile.pos.X + 0), (artTile.height + artTile.size.Y), (artTile.pos.Y + artTile.size.Y + padding - 10), 1);
            //Vector4 topLeftVertPos = new Vector4((artTile.pos.X + artTile.size.X), (artTile.height + 0), (artTile.pos.Y + artTile.size.Y + padding - 10), 1);
            //Vector4 bottomLeftVertPos = new Vector4((artTile.pos.X + artTile.size.X), (artTile.height + artTile.size.Y), (artTile.pos.Y + artTile.size.Y + padding - 10), 1);

            solidGeomVertices.Add(new WCVertex(
                bottomRightVertPos,
                normal,
                bottomRightTexCoord,
                bottomRightNormalCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                bottomLeftVertPos,
                normal,
                bottomLeftTexCoord,
                bottomLeftNormalCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                topRightVertPos,
                normal,
                topRightTexCoord,
                topRightNormalCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                bottomLeftVertPos,
                normal,
                bottomLeftTexCoord,
                bottomLeftNormalCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                topLeftVertPos,
                normal,
                topLeftTexCoord,
                topLeftNormalCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                topRightVertPos,
                normal,
                topRightTexCoord,
                topRightNormalCoord,
                tangent,
                binormal));
        }

        private void AddArtTile(ArtTile artTile)
        {
            Vector3 normal = Vector3.Backward;
            Vector3 tangent = Vector3.Up;
            Vector3 binormal = Vector3.Cross(tangent, normal);

            //int quarter = x % 2 + 2 * (z % 2);

            //int floorTexture = GetWallTextureInt(x, y, z);

            //if (floorTexture == 4 && edgeTilesOnly)
            //{
                //return;
            //}

            float padding = 0.03f;

            Vector2 topRightTexCoord = textureAtlas.GetConvertedCoordinate(artTile.color, new Vector2(0f + padding, 1f - padding));
            Vector2 bottomRightTexCoord = textureAtlas.GetConvertedCoordinate(artTile.color, new Vector2(0f + padding, 0f + padding));
            Vector2 topLeftTexCoord = textureAtlas.GetConvertedCoordinate(artTile.color, new Vector2(1f - padding, 1f - padding));
            Vector2 bottomLeftTexCoord = textureAtlas.GetConvertedCoordinate(artTile.color, new Vector2(1f - padding, 0f + padding));

            Vector2 topRightNormalCoord = textureAtlas.GetConvertedCoordinate(artTile.normal, new Vector2(0f + padding, 1f - padding));
            Vector2 bottomRightNormalCoord = textureAtlas.GetConvertedCoordinate(artTile.normal, new Vector2(0f + padding, 0f + padding));
            Vector2 topLeftNormalCoord = textureAtlas.GetConvertedCoordinate(artTile.normal, new Vector2(1f - padding, 1f - padding));
            Vector2 bottomLeftNormalCoord = textureAtlas.GetConvertedCoordinate(artTile.normal, new Vector2(1f - padding, 0f + padding));

            Vector4 topRightVertPos = new Vector4((artTile.pos.X + 0), (artTile.height + 0), (artTile.pos.Y + artTile.size.Y + padding - 10), 1);
            Vector4 bottomRightVertPos = new Vector4((artTile.pos.X + 0), (artTile.height + artTile.size.Y), (artTile.pos.Y + artTile.size.Y + padding - 10), 1);
            Vector4 topLeftVertPos = new Vector4((artTile.pos.X + artTile.size.X), (artTile.height + 0), (artTile.pos.Y + artTile.size.Y + padding - 10), 1);
            Vector4 bottomLeftVertPos = new Vector4((artTile.pos.X + artTile.size.X), (artTile.height + artTile.size.Y), (artTile.pos.Y + artTile.size.Y + padding - 10), 1);

            solidGeomVertices.Add(new WCVertex(
                bottomRightVertPos,
                normal,
                bottomRightTexCoord,
                bottomRightNormalCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                bottomLeftVertPos,
                normal,
                bottomLeftTexCoord,
                bottomLeftNormalCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                topRightVertPos,
                normal,
                topRightTexCoord,
                topRightNormalCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                bottomLeftVertPos,
                normal,
                bottomLeftTexCoord,
                bottomLeftNormalCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                topLeftVertPos,
                normal,
                topLeftTexCoord,
                topLeftNormalCoord,
                tangent,
                binormal));

            solidGeomVertices.Add(new WCVertex(
                topRightVertPos,
                normal,
                topRightTexCoord,
                topRightNormalCoord,
                tangent,
                binormal));
        }

        public void DrawScene(GameBase game, GameplayManager gameplay, RenderTarget2D renderTarget, RenderTarget2D depthTarget, RenderTarget2D spriteTarget, bool depthPass)
        {
            cameraPosition = new Vector3(gameplay.camera.cameraPosition.X + 320, 100f, gameplay.camera.cameraPosition.Y + 380);
            viewMatrix = Matrix.CreateLookAt(cameraPosition, new Vector3(cameraPosition.X, 0f, cameraPosition.Z - 100), new Vector3(0f, 0.5f, -0.5f));

            normalMappingEffect.Parameters["View"].SetValue(viewMatrix);
            normalMappingEffect.Parameters["EyePosition"].SetValue(cameraPosition);
            normalMappingEffect.CurrentTechnique = normalMappingEffect.Techniques["MultiPassLight"]; //geom has normal map (some sprites do not)

            parallaxEffect.Parameters["View"].SetValue(viewMatrix);
            parallaxEffect.Parameters["EyePosition"].SetValue(cameraPosition);

            if (hqLight)
            {
                lightDictionary["camera"].PointLightPosition = new Vector3(cameraPosition.X + 10, 30f + 100, cameraPosition.Z - 250);
                lightDictionary["camera2"].PointLightPosition = new Vector3(cameraPosition.X + 100, 0, cameraPosition.Z + 250);
            }
            //lightDictionary["character"].PointLightPosition = new Vector3(game.playerManager.getMeanPlayerPos().X + 10, 30f, game.playerManager.getMeanPlayerPos().Y + 20);
            

            if (solidGeomVertices.Count <= 0)
                return;

            
            game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            normalMappingEffect.Parameters["ColorMap"].SetValue(textureAtlas.texture);

            

            game.GraphicsDevice.SetVertexBuffer(buffer);


            if (depthPass)
            {
                game.GraphicsDevice.BlendState = BlendState.Opaque;


                //return;
                game.GraphicsDevice.SetRenderTarget(depthTarget);
                //game.GraphicsDevice.Clear(Color.Black);

                normalMappingEffect.CurrentTechnique = normalMappingEffect.Techniques["DisplayDepth"];
                normalMappingEffect.CurrentTechnique.Passes["Depth"].Apply();

                game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, solidGeomVertices.Count / 3);

                //game.GraphicsDevice.SetVertexBuffer(parallaxBuffer);
                //parallaxEffect.CurrentTechnique = parallaxEffect.Techniques["DisplayDepth"];
                //parallaxEffect.CurrentTechnique.Passes["Depth"].Apply();
                //if(parallaxVertices.Count>0)
                //game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, parallaxVertices.Count / 3);

            }
            else
            {

                game.GraphicsDevice.SetRenderTarget(renderTarget);
                //game.GraphicsDevice.Clear(Color.Black);

                //game.spriteBatch.Begin(0, BlendState.Opaque, null, null, null, spriteEffect);
                //game.spriteBatch.Begin();
                //game.spriteBatch.Draw(background, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), null, Color.White);
                //game.spriteBatch.End();

                game.GraphicsDevice.SetVertexBuffer(buffer);
                game.GraphicsDevice.BlendState = BlendState.Opaque;

                normalMappingEffect.CurrentTechnique = normalMappingEffect.Techniques["MultiPassLight"];
                normalMappingEffect.CurrentTechnique.Passes["Ambient"].Apply();

                game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, solidGeomVertices.Count / 3);

                game.GraphicsDevice.BlendState = BlendState.Additive;



                foreach (KeyValuePair<string, PointLightStruct> s in lightDictionary)
                {
                    normalMappingEffect.Parameters["DiffuseColor"].SetValue(s.Value.DiffuseColor);
                    normalMappingEffect.Parameters["DiffuseIntensity"].SetValue(s.Value.DiffuseIntensity);
                    normalMappingEffect.Parameters["SpecularColor"].SetValue(s.Value.SpecularColor);
                    normalMappingEffect.Parameters["SpecularIntensity"].SetValue(s.Value.SpecularIntensity);
                    normalMappingEffect.Parameters["PointLightPosition"].SetValue(s.Value.PointLightPosition);
                    normalMappingEffect.Parameters["PointLightRange"].SetValue(s.Value.PointLightRange);

                    normalMappingEffect.CurrentTechnique.Passes["Point"].Apply();
                    game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, solidGeomVertices.Count / 3);
                }

                foreach (PointLightStruct s in lightList)
                {
                    normalMappingEffect.Parameters["DiffuseColor"].SetValue(s.DiffuseColor);
                    normalMappingEffect.Parameters["DiffuseIntensity"].SetValue(s.DiffuseIntensity);
                    normalMappingEffect.Parameters["SpecularColor"].SetValue(s.SpecularColor);
                    normalMappingEffect.Parameters["SpecularIntensity"].SetValue(s.SpecularIntensity);
                    normalMappingEffect.Parameters["PointLightPosition"].SetValue(s.PointLightPosition);
                    normalMappingEffect.Parameters["PointLightRange"].SetValue(s.PointLightRange);

                    normalMappingEffect.CurrentTechnique.Passes["Point"].Apply();
                    game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, solidGeomVertices.Count / 3);
                }



                if (game.controlsManager.debugControls.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F1))
                {
                    DateTime date = DateTime.Now; //Get the date for the file name
                    Stream stream = File.Create("depth" + date.ToString("MM-dd-yy H;mm;ss") + ".png");

                    game.GraphicsDevice.SetRenderTarget(null);
                    depthTarget.SaveAsPng(stream, renderTarget.Width, renderTarget.Height);
                    game.GraphicsDevice.SetRenderTarget(renderTarget);
                }

                //game.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            }
            
            
        }

        private void SetEffectParameters(GameplayManager gameplay)
        {
            normalMappingEffect.Parameters["World"].SetValue(Matrix.Identity);

            sceneDimensions = new Vector2(240 * gameplay.camera.zoom * game.aspectRatio, 240 * gameplay.camera.zoom);
            normalMappingEffect.Parameters["Projection"].SetValue(Matrix.CreateOrthographic(240 * game.aspectRatio * gameplay.camera.zoom, 240 * gameplay.camera.zoom, -800, 800));
            //normalMappingEffect.Parameters["Projection"].SetValue(Matrix.CreatePerspective(32, 16, 10, 1600));

            normalMappingEffect.Parameters["AmbientColor"].SetValue(new Vector4(0.8f, 1f, 1f, 1f));
            
            if(hqLight)
                normalMappingEffect.Parameters["AmbientIntensity"].SetValue(0.015f);
            else
                normalMappingEffect.Parameters["AmbientIntensity"].SetValue(0.815f);

            normalMappingEffect.Parameters["baseColor"].SetValue(new Vector4(0.02f, 0.05f, 0.05f, 1f));

            parallaxEffect.Parameters["World"].SetValue(Matrix.Identity);
            parallaxEffect.Parameters["Projection"].SetValue(Matrix.CreateOrthographic(240 * game.aspectRatio * gameplay.camera.zoom, 240 * gameplay.camera.zoom, -800, 800));

        }

        
    }
}
