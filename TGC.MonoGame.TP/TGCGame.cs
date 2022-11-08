using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuUtilities;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Models.Players;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene;
using TGC.MonoGame.TP.Models.SkyBox;
using TGC.MonoGame.TP.Physics;
using NumericVector3 = System.Numerics.Vector3;
using Matrix = Microsoft.Xna.Framework.Matrix;
using TGC.MonoGame.TP.Utilities;
using System.Drawing;
using Point = Microsoft.Xna.Framework.Point;
using Color = Microsoft.Xna.Framework.Color;
using static TGC.MonoGame.TP.Physics.Collider;
using BepuUtilities.Collections;
using TGC.MonoGame.TP.Models.Scene.Parts;

namespace TGC.MonoGame.TP
{
    public class TGCGame : Game
    {
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


        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";



        public TGCGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        public static GraphicsDeviceManager Graphics { get; set; }

        //Cameras
        public static Camera Camera { get; set; }//FIXME
        private FreeCamera FreeCamera { get; set; }
        private TargetCamera TargetCamera { get; set; }

        //Game objects
        private SkyBox SkyBox { get; set; }
        private List<IGameModel> gamesModels = new List<IGameModel>();
        public static Ball player;
        private Scenario scenario;

        //Collision info
        public List<CollisionData> collisionInnfo = new List<CollisionData>();
        public static object CollisionSemaphoreList = new object();

        //Physics
        public Simulation Simulation { get; protected set; }
        public BufferPool BufferPool { get; private set; }
        public SimpleThreadDispatcher ThreadDispatcher { get; private set; }

        CustomRoad road = null;
        CustomRoad road1 = null;

        protected override void Initialize()
        {
            var screenSize = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(5000, 1500, 19000), screenSize);

            TargetCamera = new TargetCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.One * 100f, Vector3.Zero);

            Camera = TargetCamera;
            // Camera = FreeCamera;

            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Opaque;

            // Configuro el tamaño de la pantalla
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            Graphics.ApplyChanges();

            Collider.CollisionDetected += Collider_CollisionDetected;

            InitPhysics();
            base.Initialize();
        }

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
                // Debug.WriteLine("Collision with : " + x.GetType());
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

        private void InitPhysics()
        {
            BufferPool = new BufferPool();

            var targetThreadCount = Math.Max(1,
              Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
            ThreadDispatcher = new SimpleThreadDispatcher(targetThreadCount);

        }

        private void PreloadResources()
        {
            LoadSkybox();
            
        }

        public static Effect LightEffects;
        private void LoadSkybox()
        {
            var skyBox = Content.Load<Model>(ContentFolder3D + "skybox/cube");
            var skyBoxTexture = Content.Load<TextureCube>(ContentFolderTextures + "/skyboxes/skybox/skybox");
            var skyBoxEffect = Content.Load<Effect>(ContentFolderEffects + "SkyBox");
            LightEffects = Content.Load<Effect>(ContentFolderEffects + "LightEffect");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 5000);
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


        protected override void LoadContent()
        {

            //Load resources
            PreloadResources();

            //Create scenario         
            scenario = new Scenario(Content);

            //Add to games model list
            gamesModels.Add(scenario);

            //Load physics simulationn
            LoadScenarioSimulation();

            //Create player  
            player = new Ball(Content, new Vector3(300, 350, 400), Simulation);
            player.Graphics = Graphics;

            //Add to games model list
            gamesModels.Add(player);


            //Set camera to target player
            TargetCamera.Target = player;



            base.LoadContent();
        }


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
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.F) && (Camera is TargetCamera))
            {
                Camera = FreeCamera;
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
                m.Update(gameTime, Keyboard.GetState(), gamesModels);
            }

            //Update camera position.
            Camera.Update(gameTime);
            base.Update(gameTime);
        }

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
            Graphics.GraphicsDevice.RasterizerState = rasterizerState;
            SkyBox.Draw(Camera.View, Camera.Projection, Camera.Position);
            GraphicsDevice.RasterizerState = originalRasterizerState;
        }

        protected override void UnloadContent()
        {
            Simulation.Dispose();

            BufferPool.Clear();

            ThreadDispatcher.Dispose();

            // Libero los recursos.
            Content.Unload();
            base.UnloadContent();
        }
    }
}