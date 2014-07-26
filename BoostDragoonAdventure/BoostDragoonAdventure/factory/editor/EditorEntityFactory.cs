using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.editor;
using Microsoft.Xna.Framework;
using wickedcrush.entity;

namespace wickedcrush.factory.editor
{
    struct EditorEntityData
    {
        public String name;
        public Vector2 size, origin;
        public bool canRotate;
    }

    public class EditorEntityFactory
    {
        private EditorMap map;

        private Dictionary<String, EditorEntityData> data;

        public EditorEntityFactory(EditorMap map)
        {
            this.map = map;

            InitializeData();
        }

        public void SetMap(EditorMap map)
        {
            this.map = map;
        }

        private void InitializeData()
        {
            EditorEntityData temp = new EditorEntityData();
            data = new Dictionary<String, EditorEntityData>();
            
            temp.name = "CHEST";
            temp.size = new Vector2(20f, 20f);
            temp.origin = new Vector2(10f, 10f);
            temp.canRotate = false;
            data.Add(temp.name, temp);

            temp.name = "TURRET";
            temp.size = new Vector2(20f, 20f);
            temp.origin = new Vector2(10f, 10f);
            temp.canRotate = true;
            data.Add(temp.name, temp);

            temp.name = "POT";
            temp.size = new Vector2(20f, 20f);
            temp.origin = new Vector2(10f, 10f);
            temp.canRotate = false;
            data.Add(temp.name, temp);

            temp.name = "GRASS";
            temp.size = new Vector2(20f, 20f);
            temp.origin = new Vector2(10f, 10f);
            temp.canRotate = false;
            data.Add(temp.name, temp);

            temp.name = "FLOOR_SWITCH";
            temp.size = new Vector2(20f, 20f);
            temp.origin = new Vector2(10f, 10f);
            temp.canRotate = false;
            data.Add(temp.name, temp);

            temp.name = "TIMER";
            temp.size = new Vector2(20f, 20f);
            temp.origin = new Vector2(10f, 10f);
            temp.canRotate = false;
            data.Add(temp.name, temp);
        }

        public void AddEntity(String code, Vector2 pos, Direction direction)
        {
            map.entityList.Add(new EditorEntity(code, data[code].name, pos, data[code].size, data[code].origin, data[code].canRotate, direction));
        }
    }
}
