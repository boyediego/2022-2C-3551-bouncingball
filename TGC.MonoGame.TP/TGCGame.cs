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

        private const int ShadowmapSize = 2048;
        private const int EnvironmentmapSize = 2048;

        private readonly float LightCameraFarPlaneDistance = 90000f;
        private readonly float LightCameraNearPlaneDistance = 1f;


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
        private Scenario scenario;

        //Collision info
        public List<CollisionData> collisionInnfo = new List<CollisionData>();
        public static object CollisionSemaphoreList = new object();

        //Physics
        private Simulation Simulation { get;  set; }
        private BufferPool BufferPool { get;  set; }
        private SimpleThreadDispatcher ThreadDispatcher { get;  set; }

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

            //Set current scene
            SharedObjects.CurrentScene = scenario;

            //Add to games model list
            gamesModels.Add(scenario);

            //Load physics simulationn
            LoadScenarioSimulation();

            //Create player  
            player = new PlasticBall(Simulation, new Vector3(300, 350, 400));

            //Add to games model list
            gamesModels.Add(player);

            //Set camera to target player
            TargetCamera.Target = player;


            //Set Light Position
            LightCamera = new FixedTargetCamera(1f, SharedObjects.CurrentScene.LightPosition, player.Position);
            LightCamera.BuildProjection(1f, LightCameraNearPlaneDistance, LightCameraFarPlaneDistance,
                MathHelper.PiOver2);
            LightCamera.BuildView();

            //Set render for cube enviroment
            SharedObjects.CurrentEnvironmentMapRenderTarget = new RenderTargetCube(GraphicsDevice, EnvironmentmapSize, false,
               SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);


            base.LoadContent();
        }

        private void PreloadResources()
        {
            ShadowMapRenderTarget = new RenderTarget2D(GraphicsDevice, ShadowmapSize, ShadowmapSize, false,
                        SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);

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
            TexturesHolder<Texture2D>.Load(Content, "Metal", "balls/metal");
            TexturesHolder<Texture2D>.Load(Content, "Metal-Normal", "balls/metal-normal");
            TexturesHolder<Texture2D>.Load(Content, "Bronze", "balls/bronze");
            TexturesHolder<Texture2D>.Load(Content, "Bronze-Normal", "balls/bronze-normal");
            TexturesHolder<Texture2D>.Load(Content, "OldWood", "balls/OldWood");
            TexturesHolder<Texture2D>.Load(Content, "OldWood-Normal", "balls/OldWood-normal");
            TexturesHolder<Texture2D>.Load(Content, "Plastic", "balls/Plastic");
            TexturesHolder<Texture2D>.Load(Content, "Plastic-Normal", "balls/Plastic-normal");

            //Roads Textures
            TexturesHolder<Texture2D>.Load(Content, "Road-Type-2", "roads/road-type-2");
            TexturesHolder<Texture2D>.Load(Content, "Road-Type-2-Normal", "roads/road-type-2-normal");
            TexturesHolder<Texture2D>.Load(Content, "Plataform-Type-1", "roads/Plataform-Type-1");
            TexturesHolder<Texture2D>.Load(Content, "Plataform-Type-1-Normal", "roads/Plataform-Type-1-normal");
            TexturesHolder<Texture2D>.Load(Content, "Gravel", "roads/Gravel");
            TexturesHolder<Texture2D>.Load(Content, "Gravel-Normal", "roads/Gravel-normal");

            //Powerups Textures
            TexturesHolder<Texture2D>.Load(Content, "Powerup", "powerups/Powerup");
            TexturesHolder<Texture2D>.Load(Content, "Powerup-Normal", "powerups/Powerup-normal");

            //Obstacule Textures
            TexturesHolder<Texture2D>.Load(Content, "Stone-Type-1", "obstacules/Stone-Type-1");
            TexturesHolder<Texture2D>.Load(Content, "Stone-Type-1-Normal", "obstacules/Stone-Type-1-normal");

            //Extras
            TexturesHolder<Texture2D>.Load(Content, "Grass-Type-1", "extras/Grass-Type-1");
            TexturesHolder<Texture2D>.Load(Content, "Grass-Type-1-Normal", "extras/Grass-Type-1-normal");
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
            else if (Keyboard.GetState().IsKeyDown(Keys.C) && !(Camera is StaticCamera))
            {
                SetCubemapCameraForOrientation(CubeMapFace.PositiveY);
                CubeMapCamera.BuildView();
                Camera = CubeMapCamera;
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

            //Update Light Position and Light camera
            SharedObjects.CurrentScene.LightPosition = player.Position + new Vector3(4000, 1500, 1400);

            LightCamera.Position = SharedObjects.CurrentScene.LightPosition;
            LightCamera.TargetPosition = player.Position;
            LightCamera.BuildView();

            //Update cubmap camera
            CubeMapCamera.Position = player.Position + new Vector3(0, 0, 0); 


            base.Update(gameTime);
        }

        #endregion

        #region Draw

        private RenderTarget2D ShadowMapRenderTarget;
        

        protected override void Draw(GameTime gameTime)
        {
            DrawGame(gameTime);
        }

        private void DrawGame(GameTime gameTime)
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
                        m.Draw(gameTime, CubeMapCamera.View, CubeMapCamera.Projection, "LightAndShadow");
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