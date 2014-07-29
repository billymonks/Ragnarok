﻿using System;
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

        public EditorEntity getEntity(String code)
        {
            //return preview;
            return new EditorEntity(code, data[code].name, new Vector2(0f, 0f), data[code].size, data[code].origin, data[code].canRotate, Direction.East);
        }

        public void LoadEntity(String code, Vector2 pos, Direction direction)
        {
            preview = new EditorEntity(code, data[code].name, new Vector2(0f, 0f), data[code].size, data[code].origin, data[code].canRotate, Direction.East);
        }

        public void AddEntity(String code, Vector2 pos, Direction direction)
        {
            if (!CanPlace(code, pos))
                return;

            Point coordinate = Helper.convertPositionToCoordinate(pos, map, LayerType.ENTITY);
            Vector2 correctedPos = new Vector2(coordinate.X * 10f, coordinate.Y * 10f);
            map.entityList.Add(new EditorEntity(code, data[code].name, correctedPos, data[code].size, data[code].origin, data[code].canRotate, direction));
        }

        public bool CanPlace(String code, Vector2 pos)
        {
            //Point size = new Point((int)(preview.size.X / 10f), (int)(preview.size.Y / 10f));
            Point coordinate = Helper.convertPositionToCoordinate(pos, map, LayerType.ENTITY);
            Vector2 correctedPos = new Vector2(coordinate.X * 10f, coordinate.Y * 10f);

            EditorEntity temp = new EditorEntity(code, data[code].name, correctedPos, data[code].size, data[code].origin, data[code].canRotate, Direction.East);

            foreach (EditorEntity e in map.entityList)
            {
                if (e.Collision(temp))
                    return false;
            }

            return true;
        }
    }
}
