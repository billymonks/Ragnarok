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
        private Game _game;

        private List<GameTask> taskList;
        private List<GameTask> removeList;

        public TaskManager(Game g)
        {
            _game = g;
            taskList = new List<GameTask>();
            removeList = new List<GameTask>();
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
            removeList.Clear();
        }

        public void EnqueueTask(GameTask task)
        {
            taskList.Add(task);
        }
    }
}
