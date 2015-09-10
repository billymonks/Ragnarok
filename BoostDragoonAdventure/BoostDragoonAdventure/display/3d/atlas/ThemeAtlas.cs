using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.map;

namespace wickedcrush.display._3d.atlas
{
    public delegate void ThemeApplication(List<ArtLayer> a);

    public static class ThemeAtlas
    {
        public static Dictionary<String, ThemeApplication> themes;

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
                    artLayers.Add(new ArtLayer(game, LayerTransformations.ScaleLayer(LayerTransformations.SubtractLayer(map.layerList[LayerType.WALL].data, LayerTransformations.GetCompositeLayer(map.layerList[LayerType.ART2].data, map.layerList[LayerType.ART2].data, true)), 2), 0, 6, "rocky"));

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
            }
        }

    }
}
