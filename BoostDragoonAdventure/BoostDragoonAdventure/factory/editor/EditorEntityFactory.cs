using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.editor;
using Microsoft.Xna.Framework;
using wickedcrush.entity;
using wickedcrush.helper;

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

        private EditorEntity preview;

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

        public EditorEntity getEntity(String code, Vector2 pos, Direction angle)
        {
            UpdatePreview(code, pos, angle);

            return preview;
        }

        public void LoadEntity(String code, Vector2 pos, Direction angle)
        {
            InitializePreview(code, pos, angle);
        }

        public void AddEntity(String code, Vector2 pos, Direction angle)
        {
            if (!CanPlace(code, pos, angle))
                return;

            map.entityList.Add(new EditorEntity(code, data[code].name, getCorrectedPos(pos), data[code].size, data[code].origin, data[code].canRotate, angle));
        }

        public bool CanPlace(String code, Vector2 pos, Direction angle)
        {

            EditorEntity temp = getEntity(code, pos, Direction.East);
                //new EditorEntity(code, data[code].name, getCorrectedPos(pos), data[code].size, data[code].origin, data[code].canRotate, Direction.East);

            foreach (EditorEntity e in map.entityList)
            {
                if (e.Collision(temp))
                    return false;
            }

            if (map.layerCollision(temp, LayerType.WALL) || map.layerCollision(temp, LayerType.DEATHSOUP))
                return false;

            return true;
        }

        private Vector2 getCorrectedPos(Vector2 pos)
        {
            Point coordinate = Helper.convertPositionToCoordinate(pos, map, LayerType.ENTITY);
            return new Vector2(coordinate.X * 10f, coordinate.Y * 10f);
        }

        private void InitializePreview(String code, Vector2 pos, Direction angle)
        {
            if (preview != null)
                return;

            preview = new EditorEntity(code, data[code].name, getCorrectedPos(pos), data[code].size, data[code].origin, data[code].canRotate, angle);
        }

        private void UpdatePreview(String code, Vector2 pos, Direction angle)
        {
            if (preview == null)
            {
                InitializePreview(code, pos, angle);
                return;
            }

            preview.code = code;
            preview.name = data[code].name;
            preview.pos = getCorrectedPos(pos);
            preview.size = data[code].size;
            preview.origin = data[code].origin;
            preview.canRotate = data[code].canRotate;
            preview.angle = angle;
        }
    }
}
