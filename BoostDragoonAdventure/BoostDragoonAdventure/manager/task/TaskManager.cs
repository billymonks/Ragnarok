using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.task;
using Microsoft.Xna.Framework;

namespace wickedcrush.manager.task
{
    public class TaskManager
    {
        private GameBase _game;

        private List<GameTask> taskList;
        private List<GameTask> removeList;
        private List<GameTask> addList;

        public TaskManager(GameBase g)
        {
            _game = g;
            taskList = new List<GameTask>();
            removeList = new List<GameTask>();
            addList = new List<GameTask>();
        }

        public void Update(GameTime gameTime)
        {
            foreach (GameTask t in taskList)
            {
                if (t.testCondition(_game))
                {
                    t.runAction(_game);
                    removeList.Add(t);
                }
            }

            foreach (GameTask t in removeList)
            {
                taskList.Remove(t);
            }
            foreach (GameTask t in addList)
            {
                taskList.Add(t);
            }
            removeList.Clear();
            addList.Clear();
        }

        public void EnqueueTask(GameTask task)
        {
            addList.Add(task);
        }
    }
}
