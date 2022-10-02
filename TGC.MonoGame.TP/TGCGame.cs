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
using TGC.MonoGame.TP.Models.Ball;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene;
using TGC.MonoGame.TP.Models.SkyBox;
using TGC.MonoGame.TP.Physics;
using NumericVector3 = System.Numerics.Vector3;
using Matrix = Microsoft.Xna.Framework.Matrix;
using TGC.MonoGame.TP.Utilities;


namespace TGC.MonoGame.TP
{
    public class TGCGame : Game
    {

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

       
        private GraphicsDeviceManager Graphics { get; }

        //Cameras
        private Camera Camera { get; set; }
        private FreeCamera FreeCamera { get; set; }
        private TargetCamera TargetCamera { get; set; }

        //Game objects
        private SkyBox SkyBox { get; set; }
        private List<IGameModel> gamesModels = new List<IGameModel>();
        private Ball player;
        private Scenario scenario;

        //Physics
        public Simulation Simulation { get; protected set; }
        public BufferPool BufferPool { get; private set; }
        public SimpleThreadDispatcher ThreadDispatcher { get; private set; }

        protected override void Initialize()
        {
            var screenSize = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0, 700, 5500), screenSize);

            TargetCamera = new TargetCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.One * 100f, Vector3.Zero);

            Camera = TargetCamera;

            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Opaque;

            // Configuro el tamaño de la pantalla
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            Graphics.ApplyChanges();

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

        private void PreloadResources()
        {
            LoadSkybox();
        }

        private void LoadSkybox()
        {
            var skyBox = Content.Load<Model>(ContentFolder3D + "skybox/cube");
            var skyBoxTexture = Content.Load<TextureCube>(ContentFolderTextures + "/skyboxes/skybox/skybox");
            var skyBoxEffect = Content.Load<Effect>(ContentFolderEffects + "SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 5000);
        }

        private void LoadScenarioSimulation()
        {
            //Create simulation
            Simulation = Simulation.Create(BufferPool, new NarrowPhaseCallbacks(),
                new PoseIntegratorCallbacks(new NumericVector3(0, -10 * 150, 0)), new PositionFirstTimestepper());


            //Add scene parts to simulation
            foreach (Model3D part in scenario.models)
            {
                Vector3 size = part.GetModelSize();
                Simulation.Statics.Add(new StaticDescription(new NumericVector3(part.Position.X, part.Position.Y, part.Position.Z),
                    new CollidableDescription(Simulation.Shapes.Add(new Box(size.X, size.Y, size.Z)), 0.1f)));
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
            player = new Ball(Content, new Vector3(0,150,0), Simulation);

            //Add to games model list
            gamesModels.Add(player);

            //Add player to simulation
            Simulation.Bodies.Add(player.BodyDescription);

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