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
        private Camera Camera { get; set; }
        private FreeCamera FreeCamera { get; set; }
        private TargetCamera TargetCamera { get; set; }

        private List<IGameModel> gamesModels = new List<IGameModel>();


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


            BufferPool = new BufferPool();

            var targetThreadCount = Math.Max(1,
              Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
            ThreadDispatcher = new SimpleThreadDispatcher(targetThreadCount);

            base.Initialize();
        }

        private void PreloadResources()
        {

        }
        Ball player;
        private SkyBox SkyBox { get; set; }
        private BodyHandle playerHanle;
        protected override void LoadContent()
        {
            PreloadResources();

            var skyBox = Content.Load<Model>(ContentFolder3D + "skybox/cube");
            var skyBoxTexture = Content.Load<TextureCube>(ContentFolderTextures + "/skyboxes/skybox/skybox");
            var skyBoxEffect = Content.Load<Effect>(ContentFolderEffects + "SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 5000);


            player = new Ball(Content);
            Scenario scenario = new Scenario(Content);
            gamesModels.Add(scenario);
            gamesModels.Add(player);
            TargetCamera.Target = player;


            Simulation = Simulation.Create(BufferPool, new NarrowPhaseCallbacks(),
             new PoseIntegratorCallbacks(new NumericVector3(0, -10 * 150, 0)), new PositionFirstTimestepper());


            foreach (Model3D part in scenario.models)
            {
                Vector3 size = part.GetModelSize();
                Simulation.Statics.Add(new StaticDescription(new NumericVector3(part.Position.X, part.Position.Y, part.Position.Z),
                    new CollidableDescription(Simulation.Shapes.Add(new Box(size.X, size.Y, size.Z)), 0.1f)));
            }


            var position = new NumericVector3(player.Position.X, player.Position.Y, player.Position.Z);
            var boundingPlayer = player.GetBoundingSphere();
            var simulationPlayer = new Sphere(boundingPlayer.Radius - 60);

             var bodyDescription = BodyDescription.CreateConvexDynamic(position,
                 1f, Simulation.Shapes, simulationPlayer);

            /*var bodyDescription = BodyDescription.CreateConvexKinematic(new RigidPose() { Position=position},Simulation.Shapes, simulationPlayer);
            bodyDescription.LocalInertia.InverseMass = 1;*/
            
            playerHanle = Simulation.Bodies.Add(bodyDescription);

            vecAnterior = new Vector3(1, 1, 1);
           

            base.LoadContent();
        }

        private Vector3 vecAnterior;
        private Vector3 velAnterior;
        protected override void Update(GameTime gameTime)
        {

            float time = (float)gameTime.TotalGameTime.Milliseconds;

            Simulation.Timestep(1 / 60f, ThreadDispatcher);


            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            if (Keyboard.GetState().IsKeyDown(Keys.T) && (Camera is FreeCamera))
            {
                Camera = TargetCamera;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.F) && (Camera is TargetCamera))
            {
                Camera = FreeCamera;
            }


            foreach (IGameModel m in gamesModels)
            {
                m.Update(gameTime, Keyboard.GetState(), gamesModels);
            }



            var bodyReference = Simulation.Bodies.GetBodyReference(playerHanle);
            var position = bodyReference.Pose.Position;
            var quaternion = bodyReference.Pose.Orientation;

            
            
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                bodyReference.Awake = true;
                bodyReference.ApplyLinearImpulse(new NumericVector3(0, 0, -100));
                
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                bodyReference.Awake = true;
                bodyReference.ApplyLinearImpulse(new NumericVector3(0, 0, 20));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                bodyReference.Awake = true;
                bodyReference.ApplyLinearImpulse(new NumericVector3(-25, 0, 0));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                bodyReference.Awake = true;
                bodyReference.ApplyLinearImpulse(new NumericVector3(25, 0, 0));
            }


            Matrix rotation = Matrix.CreateFromQuaternion(new Quaternion(quaternion.X, quaternion.Y, quaternion.Z,
                   quaternion.W));

            player.setWorldMatrix(rotation, Microsoft.Xna.Framework.Matrix.CreateTranslation(new Vector3(position.X, position.Y, position.Z)));


            
            var vector = new Vector3(bodyReference.Velocity.Linear.X, 0, bodyReference.Velocity.Linear.Z);

            if (vector.LengthSquared() > 0)
            {
                velAnterior = vector;
                velAnterior.Normalize();
            }




            vecAnterior = Vector3.Lerp(vecAnterior, -velAnterior, 0.05f);

            Camera.View = Matrix.CreateLookAt(player.WorldMatrix.Translation + new Vector3(0, 550,0) + vecAnterior * 1000f, 
                player.WorldMatrix.Translation, 
                Vector3.Up);
            

            //Camera.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var originalRasterizerState = GraphicsDevice.RasterizerState;
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            Graphics.GraphicsDevice.RasterizerState = rasterizerState;

            SkyBox.Draw(Camera.View, Camera.Projection, Camera.Position);

            GraphicsDevice.RasterizerState = originalRasterizerState;

            foreach (IGameModel m in gamesModels)
            {
                m.Draw(gameTime, Camera.View, Camera.Projection);
            }


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