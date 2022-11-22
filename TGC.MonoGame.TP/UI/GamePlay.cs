using BepuPhysics;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using NumericVector3 = System.Numerics.Vector3;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Players;
using TGC.MonoGame.TP.Models.SkyBox;
using TGC.MonoGame.TP.Physics;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Shared;
using TGC.MonoGame.TP.Models.Scene;
using TGC.MonoGame.TP.Utilities;
using BepuPhysics.CollisionDetection;
using static TGC.MonoGame.TP.Physics.Collider;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TGC.MonoGame.TP.UI
{
    public class GamePlay : IRenderUI
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

        private TGCGame Game { get; set; }

        private const int ShadowmapSize = 2048;
        private const int EnvironmentmapSize = 512;

        private readonly float LightCameraFarPlaneDistance = 90000f;
        private readonly float LightCameraNearPlaneDistance = 1f;

        //Collision info
        public List<CollisionData> collisionInnfo = new List<CollisionData>();
        public static object CollisionSemaphoreList = new object();


        //Cameras
        private Camera Camera { get; set; }
        private FreeCamera FreeCamera { get; set; }
        private TargetCamera TargetCamera { get; set; }
        private StaticCamera CubeMapCamera { get; set; }

        private FixedTargetCamera LightCamera { get; set; }

        //Game objects
        private SkyBox SkyBox { get; set; }
        private List<IGameModel> gamesModels = new List<IGameModel>();
        private Ball player;
        private IScene scenario;

        //Physics objects
        private Simulation Simulation { get; set; }
        private BufferPool BufferPool { get; set; }
        private SimpleThreadDispatcher ThreadDispatcher { get; set; }

        private RenderTarget2D ShadowMapRenderTarget;

        private int selectedPlayer;

        private GraphicsDevice GraphicsDevice
        {
            get { return SharedObjects.graphicsDeviceManager.GraphicsDevice; }
        }

        public GamePlay(TGCGame game, int selectedPlayer)
        {
            this.selectedPlayer = selectedPlayer;
            Initialize(game);
        }

        public void Initialize(TGCGame game)
        {
            //Set simulation
            InitPhysics();

            //Create simulation
            PositionFirstTimestepper p = new PositionFirstTimestepper();
            Simulation = Simulation.Create(BufferPool, new NarrowPhaseCallbacks(),
                new PoseIntegratorCallbacks(new NumericVector3(0, -10 * 150, 0)), p);

            this.Game = game;

            //Get Viewport screen size
            var screenSize = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            //Create free camera object
            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0, 400, 0), screenSize);

            //Create Target camera object
            TargetCamera = new TargetCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.One * 100f, Vector3.Zero);

            //Set inital camera
            Camera = TargetCamera;
            // Camera = FreeCamera;

            SharedObjects.CurrentCamera = Camera;

            //Init cubeface camera
            CubeMapCamera = new StaticCamera(1f, Vector3.Zero, Vector3.UnitX, Vector3.Up);
            CubeMapCamera.BuildProjection(1f, 1f, 5000f, MathHelper.PiOver2);


            //Init post-procesing renders
            ShadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, ShadowmapSize, ShadowmapSize, false,
            SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);

            SharedObjects.CurrentEnvironmentMapRenderTarget = new RenderTargetCube(GraphicsDevice, EnvironmentmapSize, false,
                       SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);


            switch (selectedPlayer)
            {
                case Ball.Metal:
                    this.player = new MetalBall(Simulation, new Vector3(300, 350, 400));
                    break;

                case Ball.Wood:
                    this.player = new WoodBall(Simulation, new Vector3(300, 350, 400));
                    break;

                case Ball.Plastic:
                    this.player = new PlasticBall(Simulation, new Vector3(300, 350, 400));
                    break;
            }


            CreateScenario(new Scenario(), this.player);

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.Play(MusicHolder<Song>.Get("GamePlay"));

        }

        private void InitPhysics()
        {
            BufferPool = new BufferPool();

            var targetThreadCount = Math.Max(1,
              Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
            ThreadDispatcher = new SimpleThreadDispatcher(targetThreadCount);
        }

        private void CreateScenario(IScene scene, Ball player)
        {
            //Create skybox
            CreteSkybox();

            //Create scenario         
            scenario = scene;

            scene.EndGame += Scene_EndGame;

            //Set current scene
            SharedObjects.CurrentScene = scenario;

            //Add to games model list
            gamesModels.Add(scenario);

            //Load physics simulationn
            LoadScenarioSimulation();

            //Add to games model list
            gamesModels.Add(player);

            //Set camera to target player
            TargetCamera.Target = player;

            //Set Light Position
            LightCamera = new FixedTargetCamera(1f, SharedObjects.CurrentScene.LightPosition, player.Position);
            LightCamera.BuildProjection(1f, LightCameraNearPlaneDistance, LightCameraFarPlaneDistance,
                MathHelper.PiOver2);
            LightCamera.BuildView();
        }

        private void Scene_EndGame(object sender, EventArgs e)
        {
            Game.EndGameplay(this);
        }

        private void CreteSkybox()
        {
            var skyBox = ModelsHolder.Get("SkyBoxCube");
            var skyBoxTexture = TexturesHolder<TextureCube>.Get("SkyBox");
            var skyBoxEffect = EffectsHolder.Get("SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 5000);
        }

        private void LoadScenarioSimulation()
        {
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


        public void CollisionDetected(CollidablePair pair, CollisionInformation info)
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

        public void Upate(GameTime gameTime)
        {
            //Simulation timestep
            Simulation.Timestep(1 / 60f, ThreadDispatcher);

            //Camera controls 
            if (Keyboard.GetState().IsKeyDown(Keys.T) && !(Camera is TargetCamera))
            {
                Camera = TargetCamera;
                SharedObjects.CurrentCamera = Camera;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.F) && !(Camera is FreeCamera))
            {
                Camera = FreeCamera;
                SharedObjects.CurrentCamera = Camera;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.L) && !(Camera is FixedTargetCamera))
            {
                Camera = LightCamera;
                SharedObjects.CurrentCamera = Camera;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Escape) )
            {
                Game.EndGameplay(this);
                return;
            }

            //Before update check collision
            lock (CollisionSemaphoreList)
            {
                foreach (var c in collisionInnfo)
                {
                    c.player.Collide(gameTime, c.sceneObject);
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

            //Update Light Position and Light camera
            SharedObjects.CurrentScene.LightPosition = player.Position + new Vector3(4000, 1500, 1400);

            LightCamera.Position = SharedObjects.CurrentScene.LightPosition;
            LightCamera.TargetPosition = player.Position;
            LightCamera.BuildView();

            //Update cubmap camera
            CubeMapCamera.Position = player.Position + new Vector3(0, 0, 0);
        }

        public void Draw(GameTime gameTime)
        {
            //Draw depth for shadows
            DrawDepthPass(gameTime);

            //Draw Enviroment map
            if (player.HasEnviromentMap)
            {
                DrawEnviromentMap(gameTime);
            }
            //Draw screen
            DrawGameplay(gameTime);
        }

        private void DrawGameplay(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
            // SkyBox.Texture = SharedObjects.CurrentEnvironmentMapRenderTarget;


            DrawSkyBox(this.Camera);
            foreach (IGameModel m in gamesModels)
            {
                EffectsHolder.Get("LightEffect").Parameters["eyePosition"].SetValue(SharedObjects.CurrentCamera.Position);
                EffectsHolder.Get("LightEffect").Parameters["lightPosition"].SetValue(SharedObjects.CurrentScene.LightPosition);
                EffectsHolder.Get("LightEffect").Parameters["ambientColor"].SetValue(SharedObjects.CurrentScene.AmbientLightColor);
                EffectsHolder.Get("LightEffect").Parameters["diffuseColor"].SetValue(SharedObjects.CurrentScene.DiffuseLightColor);
                EffectsHolder.Get("LightEffect").Parameters["specularColor"].SetValue(SharedObjects.CurrentScene.SpecularLightColor);
                EffectsHolder.Get("LightEffect").Parameters["shadowMap"].SetValue(ShadowMapRenderTarget);
                EffectsHolder.Get("LightEffect").Parameters["shadowMapSize"].SetValue(Vector2.One * ShadowmapSize);
                EffectsHolder.Get("LightEffect").Parameters["LightViewProjection"].SetValue(LightCamera.View * LightCamera.Projection);
                EffectsHolder.Get("LightEffect").Parameters["hasEnviroment"].SetValue(0f);
                EffectsHolder.Get("LightEffect").Parameters["environmentMap"].SetValue(SharedObjects.CurrentEnvironmentMapRenderTarget);
                m.Draw(gameTime, Camera.View, Camera.Projection, "LightAndShadow");
            }
        }

        private void DrawSkyBox(Camera Camera)
        {
            var originalRasterizerState = GraphicsDevice.RasterizerState;
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            SharedObjects.graphicsDeviceManager.GraphicsDevice.RasterizerState = rasterizerState;
            SkyBox.Draw(Camera.View, Camera.Projection, Camera.Position);
            GraphicsDevice.RasterizerState = originalRasterizerState;
        }

        public void DrawDepthPass(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SetRenderTarget(ShadowMapRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);


            foreach (IGameModel m in gamesModels)
            {
                m.Draw(gameTime, LightCamera.View, LightCamera.Projection, "DepthPass");
            }
        }

        public void DrawEnviromentMap(GameTime gameTime)
        {
            //Draw cubface for enviroment map
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            // Draw to our cubemap from the robot position
            for (var face = CubeMapFace.PositiveX; face <= CubeMapFace.NegativeZ; face++)
            {
                // Set the render target as our cubemap face, we are drawing the scene in this texture
                GraphicsDevice.SetRenderTarget(SharedObjects.CurrentEnvironmentMapRenderTarget, face);
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1f, 0);

                if (face == CubeMapFace.NegativeZ || face == CubeMapFace.PositiveX || face== CubeMapFace.NegativeX)
                {
                    //Set camera orientation
                    SetCubemapCameraForOrientation(face);
                    CubeMapCamera.BuildView();

                    // Draw our scene. Do not draw our bakl as it would be occluded by itself 
                    // (if it has backface culling on)

                    // DrawSkyBox(CubeMapCamera);
                    foreach (IGameModel m in gamesModels)
                    {
                        if (m != player)
                        {
                            EffectsHolder.Get("LightEffect").Parameters["eyePosition"].SetValue(player.Position);
                            EffectsHolder.Get("LightEffect").Parameters["lightPosition"].SetValue(SharedObjects.CurrentScene.LightPosition);
                            EffectsHolder.Get("LightEffect").Parameters["ambientColor"].SetValue(SharedObjects.CurrentScene.AmbientLightColor);
                            EffectsHolder.Get("LightEffect").Parameters["diffuseColor"].SetValue(SharedObjects.CurrentScene.DiffuseLightColor);
                            EffectsHolder.Get("LightEffect").Parameters["specularColor"].SetValue(SharedObjects.CurrentScene.SpecularLightColor);
                            EffectsHolder.Get("LightEffect").Parameters["shadowMap"].SetValue(ShadowMapRenderTarget);
                            EffectsHolder.Get("LightEffect").Parameters["shadowMapSize"].SetValue(Vector2.One * ShadowmapSize);
                            EffectsHolder.Get("LightEffect").Parameters["LightViewProjection"].SetValue(LightCamera.View * LightCamera.Projection);
                            EffectsHolder.Get("LightEffect").Parameters["hasEnviroment"].SetValue(0f);
                            m.Draw(gameTime, CubeMapCamera.View, CubeMapCamera.Projection, "NormalMapping");
                        }
                    }

                }
            }
        }

        private void SetCubemapCameraForOrientation(CubeMapFace face)
        {
            switch (face)
            {
                default:
                case CubeMapFace.PositiveX:
                    CubeMapCamera.FrontDirection = -Vector3.UnitX;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.NegativeX:
                    CubeMapCamera.FrontDirection = Vector3.UnitX;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.PositiveY:
                    CubeMapCamera.FrontDirection = Vector3.Down;
                    CubeMapCamera.UpDirection = Vector3.UnitZ;
                    break;

                case CubeMapFace.NegativeY:
                    CubeMapCamera.FrontDirection = Vector3.Up;
                    CubeMapCamera.UpDirection = -Vector3.UnitZ;
                    break;

                case CubeMapFace.PositiveZ:
                    CubeMapCamera.FrontDirection = -Vector3.UnitZ;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.NegativeZ:
                    CubeMapCamera.FrontDirection = Vector3.UnitZ;
                    CubeMapCamera.UpDirection = Vector3.Down;
                    break;
            }
        }

        public void Dispose()
        {
            MediaPlayer.Stop();
            Simulation.Dispose();

            BufferPool.Clear();

            ThreadDispatcher.Dispose();

        }
    }
}
