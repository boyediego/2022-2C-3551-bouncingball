using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Models.Ball;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene;

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

            base.Initialize();
        }

        private void PreloadResources()
        {
            
        }
        Ball player;
        protected override void LoadContent()
        {
            PreloadResources();

            player = new Ball(Content);
            gamesModels.Add(new Scenario(Content));
            gamesModels.Add(player);
            TargetCamera.Target = player;
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            
            float time = (float)gameTime.TotalGameTime.Milliseconds;

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

            Camera.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            foreach (IGameModel m in gamesModels)
            {
                m.Draw(gameTime, Camera.View, Camera.Projection);
            }
        }

        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();
            base.UnloadContent();
        }
    }
}