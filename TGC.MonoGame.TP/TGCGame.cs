using System;
using System.Collections.Generic;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Models.Players;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene;
using TGC.MonoGame.TP.Models.SkyBox;
using TGC.MonoGame.TP.Physics;
using NumericVector3 = System.Numerics.Vector3;
using TGC.MonoGame.TP.Utilities;
using System.Drawing;
using Point = Microsoft.Xna.Framework.Point;
using Color = Microsoft.Xna.Framework.Color;
using static TGC.MonoGame.TP.Physics.Collider;
using BepuUtilities.Collections;
using TGC.MonoGame.TP.Models.Scene.Parts.Roads;
using TGC.MonoGame.TP.Shared;
using Extreme.Mathematics;
using BepuPhysics.Constraints;
using TGC.MonoGame.TP.Models.Scene.Parts.Obstacule;

namespace TGC.MonoGame.TP
{
    public class TGCGame : Game
    {
        

        //Cameras
        public  Camera Camera { get; set; }       
        private FreeCamera FreeCamera { get; set; }
        private TargetCamera TargetCamera { get; set; }

        //Game objects
        private SkyBox SkyBox { get; set; }
        private List<IGameModel> gamesModels = new List<IGameModel>();
        public static Ball player;  //FIXME static
        private Scenario scenario;

        //Collision info
        public List<CollisionData> collisionInnfo = new List<CollisionData>();
        public static object CollisionSemaphoreList = new object();

        //Physics
        public Simulation Simulation { get; protected set; }
        public BufferPool BufferPool { get; private set; }
        public SimpleThreadDispatcher ThreadDispatcher { get; private set; }

        #region Initialize GameParams & Content
        public TGCGame()
        {
            SharedObjects.graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //Get Viewport screen size
            var screenSize = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            //Create free camera object
            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(5000, 1500, 19000), screenSize);

            //Create Target camera object
            TargetCamera = new TargetCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.One * 100f, Vector3.Zero);

            //Set inital camera
             Camera = TargetCamera;
            // Camera = FreeCamera;

            SharedObjects.CurrentCamera = Camera;

            //Set rasterizerState
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Opaque;

            // Configuro el tamaño de la pantalla
            SharedObjects.graphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            SharedObjects.graphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            SharedObjects.graphicsDeviceManager.ApplyChanges();

            //Attach to detection collider events
            Collider.CollisionDetected += Collider_CollisionDetected;

            //Init buffer & threads for physic engine
            InitPhysics();
            base.Initialize();
        }

        private void InitPhysics()
        {
            BufferPool = new BufferPool();

            var targetThreadCount = Math.Max(1,
              Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
            ThreadDispatcher = new SimpleThreadDispatcher(targetThreadCount);
        }

        protected override void LoadContent()
        {
            //Load resources
            PreloadResources();

            //Create skybox
            CreteSkybox();

            //Create scenario         
            scenario = new Scenario();

            //Add to games model list
            gamesModels.Add(scenario);

            //Load physics simulationn
            LoadScenarioSimulation();

            //Create player  
            player = new MetalBall(Simulation, new Vector3(300, 350, 400));

            //Add to games model list
            gamesModels.Add(player);

            //Set camera to target player
            TargetCamera.Target = player;

            base.LoadContent();
        }
        
        private void PreloadResources()
        {
            LoadModels();
            LoadTextures();
            LoadEffects();
        }

        private void CreteSkybox()
        {
            var skyBox = ModelsHolder.Get("SkyBoxCube");
            var skyBoxTexture = TexturesHolder<TextureCube>.Get("SkyBox");
            var skyBoxEffect = EffectsHolder.Get("SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 5000);
        }
        private void LoadModels()
        {
            ModelsHolder.Load(Content, "SkyBoxCube", "skybox/cube");
            ModelsHolder.Load(Content, "Sphere", "balls/sphere");
            ModelsHolder.Load(Content, "SphereObstacule", "balls/sphere1");
            ModelsHolder.Load(Content, "Platform", "scene/basics/road2");
            ModelsHolder.Load(Content, "CubeModel", "scene/basics/cubo");
            ModelsHolder.Load(Content, "Powerup", "scene/basics/powerup");
        }

        private void LoadTextures()
        {
            //Skybox textures
            TexturesHolder<TextureCube>.Load(Content, "SkyBox", "skyboxes/skybox/skybox");

            //Balls Textures
            TexturesHolder<Texture2D>.Load(Content, "Test", "extras/test");
            TexturesHolder<Texture2D>.Load(Content, "Test-Normal", "extras/test-norma");

            //Roads Textures
            TexturesHolder<Texture2D>.Load(Content, "Cemento", "extras/cemento");
            TexturesHolder<Texture2D>.Load(Content, "Cemento-Normal-Map", "extras/cemento-normal-map");
        }

        private void LoadEffects()
        {
            EffectsHolder.Load(Content, "BasicShader", "BasicShader");
            EffectsHolder.Load(Content, "TextureShader", "TextureShader");
            EffectsHolder.Load(Content, "SkyBox", "SkyBox");
            EffectsHolder.Load(Content, "LightEffect", "LightEffect");
        }

        private void LoadScenarioSimulation()
        {
            PositionFirstTimestepper p = new PositionFirstTimestepper();

            //Create simulation
            Simulation = Simulation.Create(BufferPool, new NarrowPhaseCallbacks(),
                new PoseIntegratorCallbacks(new NumericVector3(0, -10 * 150, 0)), p);

            //Add scene parts to simulation
            foreach (Model3D part in scenario.models)
            {
                if (part.PhysicsType == PhysicsTypeHome.Static)
                {
                    part.GetStaticDescription(Simulation);
                }
                else if (part.PhysicsType == PhysicsTypeHome.Kinematic || part.PhysicsType == PhysicsTypeHome.Dynamic)
                {
                    part.GetBodyDescription(Simulation);
                }
            }
        }

        #endregion

        #region Collide Event Handler
        private void Collider_CollisionDetected(CollidablePair pair, CollisionInformation info)
        {
            if (!PlayerPresent(pair))
            {
                return;
            }

            CollidableReference collisionObject = GetCollisionObject(pair);
            int handle = collisionObject.Mobility == CollidableMobility.Static ? collisionObject.StaticHandle.Value : collisionObject.BodyHandle.Value;



            Model3D x = scenario.models.Find(x => (
                    (x.SimulationHandle == handle && x.PhysicsType == PhysicsTypeHome.Static && collisionObject.Mobility == CollidableMobility.Static) ||
                    (x.SimulationHandle == handle && x.PhysicsType != PhysicsTypeHome.Static && collisionObject.Mobility != CollidableMobility.Static)
            ));

            if (x == null)
            {
                return;
            }

            lock (CollisionSemaphoreList)
            {
                CollisionData data = collisionInnfo.Find(y => y.sceneObject == x);
                if (data == null)
                {
                    collisionInnfo.Add(new CollisionData(player, x));
                }
            }

        }

        //Check Before is player isPresent
        private CollidableReference GetCollisionObject(CollidablePair pair)
        {
            if (pair.A.BodyHandle.Value == player.playerHanle.Value && pair.A.Mobility == CollidableMobility.Dynamic)
            {
                return pair.B;
            }

            return pair.A;
        }

        private bool PlayerPresent(CollidablePair pair)
        {
            return (pair.A.BodyHandle.Value == player.playerHanle.Value && pair.A.Mobility == CollidableMobility.Dynamic) || (pair.B.BodyHandle.Value == player.playerHanle.Value && pair.B.Mobility == CollidableMobility.Dynamic);
        }
        #endregion

        #region Update game
        protected override void Update(GameTime gameTime)
        {
            //Gametime
            float time = (float)gameTime.TotalGameTime.Milliseconds;
            Simulation.Timestep(1 / 60f, ThreadDispatcher);


            //Exit Game
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Camera controls 
            if (Keyboard.GetState().IsKeyDown(Keys.T) && (Camera is FreeCamera))
            {
                Camera = TargetCamera;
                SharedObjects.CurrentCamera = Camera;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.F) && (Camera is TargetCamera))
            {
                Camera = FreeCamera;
                SharedObjects.CurrentCamera = Camera;
            }


            //Before update check collision
            lock (CollisionSemaphoreList)
            {
                foreach (var c in collisionInnfo)
                {
                    c.player.Collide(c.sceneObject);
                    c.procesed = true;
                }

                collisionInnfo.Clear();
            }

            //Update each model in the game
            foreach (IGameModel m in gamesModels)
            {
                m.Update(gameTime, Keyboard.GetState());
            }

            //Update camera position.
            Camera.Update(gameTime);
            base.Update(gameTime);
        }

        #endregion

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            DrawSkyBox();

            foreach (IGameModel m in gamesModels)
            {
                m.Draw(gameTime, Camera.View, Camera.Projection);
            }



        }

        private void DrawSkyBox()
        {
            var originalRasterizerState = GraphicsDevice.RasterizerState;
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            SharedObjects.graphicsDeviceManager.GraphicsDevice.RasterizerState = rasterizerState;
            SkyBox.Draw(Camera.View, Camera.Projection, Camera.Position);
            GraphicsDevice.RasterizerState = originalRasterizerState;
        }
        #endregion

        #region Unload content
        protected override void UnloadContent()
        {
            Simulation.Dispose();

            BufferPool.Clear();

            ThreadDispatcher.Dispose();

            // Libero los recursos.
            Content.Unload();
            base.UnloadContent();
        }
        #endregion

        public class CollisionData
        {
            public Model3D player;
            public Model3D sceneObject;
            public Boolean procesed;

            public CollisionData(Model3D player, Model3D sceneObject)
            {
                this.player = player;
                this.sceneObject = sceneObject;

            }
        }
    }
}