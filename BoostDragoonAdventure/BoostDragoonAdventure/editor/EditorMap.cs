using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.map.layer;

namespace wickedcrush.editor
{
    public class EditorMap
    {
        public Dictionary<LayerType, Layer> layerList;
        public List<EditorEntity> entityList;
    }
}
