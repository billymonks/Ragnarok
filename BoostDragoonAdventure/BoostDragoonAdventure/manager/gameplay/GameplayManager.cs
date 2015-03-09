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
using wickedcrush.display.spriter;

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

        private GameBase _game;

        public GameplayScreen _screen;

        public bool testMode;

        public Scene scene;

        public GameplayManager(GameBase game, GameplayScreen screen, bool testMode)
        {
            _game = game;
            _screen = screen;
            this.testMode = testMode;

            Initialize();
        }

        private void Initialize()
        {
            w = new World(Vector2.Zero);
            w.Gravity = Vector2.Zero;

            scene = new Scene(_game);

            camera = new Camera(_game.playerManager);
            

            _game.soundManager.setCam(camera);

            
            
            if (entityManager == null)
                entityManager = new EntityManager(_game);
            else
                entityManager.RemoveAll();

            _playerManager = _game.playerManager;
            _networkManager = _game.networkManager;
            _roomManager = _game.roomManager;

            factory = new EntityFactory(_game, this, entityManager, _roomManager, w);

            LoadContent();

        }

        public void LoadContent()
        {
            
        }

        public void Update(GameTime gameTime)
        {
            //entityManager.DepthSort();
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
            map = new Map(mapStats.filename);
            _game.mapManager.LoadMap(this, map, mapStats);
            scene.BuildScene(_game, map, this);

            camera.maxCamPos = new Vector2(map.width /2, map.height /2);
            

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
                        camera.cameraPosition = new Vector3(_playerManager.getMeanPlayerPos().X - 320, _playerManager.getMeanPlayerPos().Y - 240, 75f);// new Vector3(320f, 240f, 75f);
                    }
                ));
        }

        

        

        

        

    }

}
