﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.map;
using Microsoft.Xna.Framework;
using wickedcrush.manager.gameplay;
using wickedcrush.manager.map;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.display._3d.atlas
{
    public delegate void ThemeApplication(List<ArtLayer> a);

    public static class ThemeAtlas
    {
        public static Dictionary<String, ThemeApplication> themes;


        public static void PopulateScene(String theme, Scene scene)
        {
            switch (theme)
            {
                case "reverse":
                    scene.lightDictionary.Add("camera", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.06f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                    scene.lightDictionary.Add("camera2", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.05f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                    break;
                case "default":
                    scene.lightDictionary.Add("camera", new PointLightStruct(new Vector4(0.9f, 0.9f, 0.8f, 1f), 1f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 1000f));
                    scene.lightDictionary.Add("camera2", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.5f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                break;

                case "old_default":
                scene.lightDictionary.Add("camera", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.06f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                scene.lightDictionary.Add("camera2", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.05f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                break;

                case "new1":
                scene.lightDictionary.Add("camera", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.06f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                scene.lightDictionary.Add("camera2", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.05f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                break;

                case "rocky":
                scene.lightDictionary.Add("camera", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.06f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                scene.lightDictionary.Add("camera2", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.05f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                break;

                case "woodandbrick":
                scene.lightDictionary.Add("camera", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.06f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                scene.lightDictionary.Add("camera2", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.05f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                break;

                case "rocky2":
                scene.lightDictionary.Add("camera", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.06f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                scene.lightDictionary.Add("camera2", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.05f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                break;

                case "dust":
                scene.lightDictionary.Add("camera", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.9f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 1000f));
                scene.lightDictionary.Add("camera2", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.8f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                break;

                case "dustwalls":
                scene.lightDictionary.Add("camera", new PointLightStruct(new Vector4(0.8f, 0.9f, 0.9f, 1f), 0.8f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 1000f));
                scene.lightDictionary.Add("camera2", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.7f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                break;

                default:
                    scene.lightDictionary.Add("camera", new PointLightStruct(new Vector4(0.9f, 0.9f, 0.8f, 1f), 1f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 1000f));
                    scene.lightDictionary.Add("camera2", new PointLightStruct(new Vector4(0.7f, 0.9f, 0.9f, 1f), 0.5f, new Vector4(0.7f, 0.9f, 0.9f, 1f), 0f, Vector3.Zero, 500f));
                break;
            }

            
        }

        public static void PopulateBackgrounds(GameBase game, Map map, GameplayManager gameplay, MapStats mapStats)
        {
            gameplay._screen.bgs.Clear();
            switch(mapStats.theme)
            {
                case "dustwalls":
                    gameplay._screen.bgs.Enqueue(game.Content.Load<Texture2D>(@"img/tex/rock_greyscale"));
                    gameplay._screen.parallaxAmount = 1f;
                    gameplay._screen.bgScale = 0.5f;
                    gameplay._screen.scrollIncrement = new Vector2(0f, 0f);
                    break;
                
                default:
                    gameplay._screen.bgs.Enqueue(game.Content.Load<Texture2D>(@"img/tex/stolen_water_1"));
                    gameplay._screen.bgs.Enqueue(game.Content.Load<Texture2D>(@"img/tex/stolen_water_2"));
                    gameplay._screen.bgs.Enqueue(game.Content.Load<Texture2D>(@"img/tex/stolen_water_3"));
                    gameplay._screen.bgs.Enqueue(game.Content.Load<Texture2D>(@"img/tex/stolen_water_4"));
                    gameplay._screen.bgs.Enqueue(game.Content.Load<Texture2D>(@"img/tex/stolen_water_5"));
                    gameplay._screen.parallaxAmount = 1f;
                    gameplay._screen.bgScale = 2f;
                    break;
            }
        }

        public static void PopulateArtLayers(GameBase game, Map map, out List<ArtLayer> artLayers, String theme)
        {
            artLayers = new List<ArtLayer>();
            switch (theme)
            {
                case "reverse":
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART1].data, map.layerList[LayerType.ART2].data, true)), 2), -20, 0, "green_a_surface"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART1].data, map.layerList[LayerType.ART2].data, true)), 2), -20, 0, "pink_a_wall"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 0, 2, "townbricks"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 0, 2, "green_a_surface"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 2, true), 0, 3, "pink_a_wall"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 3, true), 0, 5, "pink_a_wall"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 2, true), 0, 3, "green_a_surface"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 3, true), 0, 5, "green_a_surface"));


                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "blue_a"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.DEATHSOUP].data, map.layerList[LayerType.WALL].data, true)), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "blue_a"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "blue_a"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "blue_a"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 2, 8, "blue_a"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 1, true), 7, 10, "blue_a"));

                    break;
                case "default":
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART1].data, map.layerList[LayerType.ART2].data, true)), 2), -1, 0, "dust_surface"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), 2), -3, 0, "dust_wall"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 0, 2, "cavewall",
                        LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 2, true)));
                    
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 2, true), 0, 3, "cavewall",
                        LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 3, true)));
                    
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 3, true), 0, 5, "cavewall"));


                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "pink_a_surface"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.DEATHSOUP].data, map.layerList[LayerType.WALL].data, true)), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "pink_a_wall"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "green_a_surface"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "pink_a_wall"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 2, 8, "blue_a"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 1, true), 7, 10, "blue_a"));

                    break;
                case "old_default":
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART1].data, map.layerList[LayerType.ART2].data, true)), 2), -1, 0, "cavefloor"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 0, 2, "cavewall",
                        LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 2, true)));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 2, true), 0, 3, "cavewall",
                        LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 3, true)));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 3, true), 0, 5, "cavewall"));


                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "pink_a_surface"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.DEATHSOUP].data, map.layerList[LayerType.WALL].data, true)), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "pink_a_wall"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "green_a_surface"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "pink_a_wall"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 2, 8, "blue_a"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 1, true), 7, 10, "blue_a"));

                    break;
                case "new1":
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART1].data, map.layerList[LayerType.ART2].data, true)), 2), -1, 0, "townbricks"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 0, 6, "townbricks"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "townbricks"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.DEATHSOUP].data, map.layerList[LayerType.WALL].data, true)), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "townbricks"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "townbricks"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "townbricks"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 2, 8, "townbricks"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 1, true), 7, 10, "blue_a"));


                    break;

                case "rocky":
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART1].data, map.layerList[LayerType.ART2].data, true)), 2), -1, 0, "rocky"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 0, 6, "steelandbrick"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "rocky"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.DEATHSOUP].data, map.layerList[LayerType.WALL].data, true)), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "rocky"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "rocky"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "rocky"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 2, 8, "rocky"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 1, true), 7, 10, "rocky"));


                    break;

                case "woodandbrick":
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART1].data, map.layerList[LayerType.ART2].data, true)), 2), -1, 0, "woodandbrick"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 0, 6, "steelandbrick"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "woodandbrick"));
                    //artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.DEATHSOUP].data, map.layerList[LayerType.WALL].data, true)), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "woodandbrick"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "steelandbrick"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "steelandbrick"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 2, 8, "steelandbrick"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 1, true), 7, 10, "steelandbrick"));


                    break;

                case "rocky2":
                    //artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), 2), -6, -4, "rocky_desaturated"));
                    //artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), 2), -4, -2, "rocky_saturated"));
                    //artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), 2), -2, 0, "rocky_desaturated"));
                    //artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(map.layerList[LayerType.WALL].data, 2), 0, 2, "rocky_saturated"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), 2), -2, 0, "rocky"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(map.layerList[LayerType.WALL].data, 2), 0, 2, "rocky_saturated"));

                    break;

                case "dust":
                    //artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), 2), -6, -4, "rocky_desaturated"));
                    //artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), 2), -4, -2, "rocky_saturated"));
                    //artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), 2), -2, 0, "rocky_desaturated"));
                    //artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(map.layerList[LayerType.WALL].data, 2), 0, 2, "rocky_saturated"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(
                        LayerTransformations.GetCompositeLayer(
                            map.layerList[LayerType.ART1].data,
                            LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data),
                            false),
                            2), -3, 0, "grass_surface"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(
                        LayerTransformations.SubtractLayer(
                            LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data),
                            map.layerList[LayerType.ART1].data),
                            2), -3, 0, "dust_surface"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), 2), -3, 0, "dust_wall"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(map.layerList[LayerType.WALL].data, 2), 0, 2, "rocky_saturated"));



                    break;

                case "dustwalls":

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(
                        LayerTransformations.GetCompositeLayer(
                            map.layerList[LayerType.ART1].data,
                            LayerTransformations.InvertLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.DEATHSOUP].data, true)),
                            false),
                            2), -3, 0, "grass_surface"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(
                        LayerTransformations.SubtractLayer(
                            LayerTransformations.InvertLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.DEATHSOUP].data, true)),
                            map.layerList[LayerType.ART1].data),
                            2), -3, 0, "dust_surface"));
                    

                    break;

                case "castle":
                    artLayers.Add(new ArtLayer(game, LayerTransformations.SubtractLayer(LayerTransformations.ScaleLayer(map.layerList[LayerType.WALL].data, 2), LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(map.layerList[LayerType.WALL].data, 2), 3, false)), 0, 2, "otherbricks"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(map.layerList[LayerType.WALL].data, 2), 3, false), 2, 4, "otherbricks"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(map.layerList[LayerType.WALL].data, 2), 6, false), 4, 7, "otherbricks"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(map.layerList[LayerType.WALL].data, 2), 8, false), 7, 8, "otherbricks"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(
                        LayerTransformations.SubtractLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), map.layerList[LayerType.WALL].data)
                        , 2), -2, 0, "brickslarge"));
                    break;

                case "casino":
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART1].data, map.layerList[LayerType.ART2].data, true)), 2), -1, 0, "blue_a"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), 2), -3, 0, "green_a_wall"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 0, 2, "pink_a",
                        LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 2, true)));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 2, true), 0, 3, "pink_a",
                        LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 3, true)));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 3, true), 0, 5, "pink_a"));


                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(map.layerList[LayerType.DEATHSOUP].data), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "pink_a_surface"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(LayerTransformations.InvertLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.DEATHSOUP].data, map.layerList[LayerType.WALL].data, true)), map.layerList[LayerType.ART2].data, false), 2), -2, 0, "pink_a"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "green_a_surface"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART2].data, false), 2), -2, 1, "pink_a_wall"));
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 2, 8, "blue_a"));

                    artLayers.Add(new ArtLayer(game, LayerTransformations.ShrinkLayer(LayerTransformations.ScaleLayer(LayerTransformations.GetCompositeLayer(map.layerList[LayerType.WALL].data, map.layerList[LayerType.ART1].data, false), 2), 1, true), 7, 10, "blue_a"));

                    break;
            }
        }

    }
}
