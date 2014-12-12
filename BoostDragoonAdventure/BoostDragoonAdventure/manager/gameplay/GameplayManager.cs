using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using wickedcrush.display._3d;
using wickedcrush.entity;
using wickedcrush.factory.entity;
using wickedcrush.helper;
using wickedcrush.manager.audio;
using wickedcrush.manager.controls;
using wickedcrush.manager.entity;
using wickedcrush.manager.player;
using wickedcrush.manager.gameplay.room;
using wickedcrush.map;
using wickedcrush.map.circuit;
using wickedcrush.map.layer;
using wickedcrush.stats;
using wickedcrush.manager.network;
using wickedcrush.screen.transition;
using wickedcrush.task;
using wickedcrush.manager.map;
using wickedcrush.screen;

namespace wickedcrush.manager.gameplay
{
    public class GameplayManager
    {
        
        public Map map;

        public EntityManager entityManager;
        public PlayerManager _playerManager; //replace with panelManager
        public NetworkManager _networkManager;
        public RoomManager _roomManager;

        public EntityFactory factory;

        public Camera camera;

        public World w;

        public Connection activeConnection;

        private Game _game;

        public GameplayScreen _screen; 

        public GameplayManager(Game game, GameplayScreen screen)
        {
            _game = game;
            _screen = screen;

            Initialize();
        }

        private void Initialize()
        {
            w = new World(Vector2.Zero);
            w.Gravity = Vector2.Zero;

            camera = new Camera(_game.playerManager);
            camera.cameraPosition = new Vector3(320f, 240f, 75f);

            _game.soundManager.setCam(camera);
            
            if (entityManager == null)
                entityManager = new EntityManager(_game);
            else
                entityManager.RemoveAll();

            _playerManager = _game.playerManager;
            _networkManager = _game.networkManager;
            _roomManager = _game.roomManager;

            factory = new EntityFactory(_game, this, entityManager, _roomManager, w);

        }

        public void Update(GameTime gameTime)
        {
            _playerManager.Update(gameTime); //nothing but panels
            factory.Update();

            entityManager.Update(gameTime);
            camera.Update();

            w.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));

            if (_playerManager.checkForTransition(map))
            {
                TransitionMap();
            }
        }

        public bool getFreezeFrame()
        {
            return _playerManager.pollDodgeSuccess();
        }

        

        public void TransitionMap()
        {
            if (activeConnection.Equals(null))
                return;
            
            EnqueueMapTransition();
        }

        public void LoadMap(String mapName)
        {
            Initialize();
            MapStats mapStats = _game.mapManager.getMapStatsFromAtlas(mapName);
            map = new Map(mapStats.filename, w, this);
            _game.mapManager.LoadMap(this, map, mapStats);
        }

        private void EnqueueMapTransition()
        {
            _game.playerManager.startTransition();

            SolidColorFadeTransition fadeOutTransition = new SolidColorFadeTransition(_game, 1000, true, new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 1f));
            SolidColorFadeTransition fadeInTransition = new SolidColorFadeTransition(_game, 1000, false, new Color(0f, 0f, 0f, 1f), new Color(0f, 0f, 0f, 0f));
            _game.taskManager.EnqueueTask(
                new GameTask(
                    g => true,
                    g => { g.screenManager.AddScreen(fadeOutTransition); }
                ));

            _game.taskManager.EnqueueTask(
                new GameTask(
                    g => fadeOutTransition.finished,
                    g => {
                        fadeOutTransition.Dispose();
                        g.screenManager.StartLoading();
                        LoadMap(activeConnection.mapName);
                        factory.spawnPlayers(activeConnection.doorIndex);
                        g.playerManager.endTransition();
                        g.screenManager.AddScreen(fadeInTransition, true);
                    }
                ));
        }

        

        

        

        

    }

}
