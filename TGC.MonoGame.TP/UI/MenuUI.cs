using BepuPhysics;
using BepuPhysics.CollisionDetection;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Players;
using TGC.MonoGame.TP.Models.Scene.Parts.Roads;
using TGC.MonoGame.TP.Models.SkyBox;
using TGC.MonoGame.TP.Physics;
using TGC.MonoGame.TP.Shared;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;

namespace TGC.MonoGame.TP.UI
{
    public class MenuUI : IRenderUI
    {
        private TGCGame Game { get; set; }

        private StaticCamera Camera { get; set; }

        private List<Model3D> sceneObjects = new List<Model3D>();
        private SkyBox SkyBox { get; set; }

        private WoodBall woodBall = null;
        private MetalBall metalBall = null;
        private PlasticBall plasticBall = null;

        private Ball selectedBall = null;
        private SpriteBatch SpriteBatch { get; set; }
        private SpriteFont font;


        private Vector2 TitlePosition;
        private Vector2 TextPosition;

        private SoundEffect Sound { get; set; }

        private GraphicsDevice GraphicsDevice
        {
            get { return SharedObjects.graphicsDeviceManager.GraphicsDevice; }
        }

        public MenuUI(TGCGame game)
        {
            Initialize(game);
        }

        public void Initialize(TGCGame game)
        {
            this.Game = game;

            Camera = new StaticCamera(1f, new Vector3(-500, 1000, 0), Vector3.UnitX, Vector3.Up);
            Camera.BuildProjection(1f, 1f, 15000f, MathHelper.PiOver2);
            Camera.BuildView();

            var screenSize = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

       
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            font = FontHolder<SpriteFont>.Get("GameFont");
            CreateScene();

            TitlePosition = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2f, 60) -
                              font.MeasureString("BOUNCING BALL") / 2;


            TextPosition = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2f, 120) -
                              font.MeasureString("Use A y D para selecionar la bola y luego presione enter") / 2;

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 1f;
            MediaPlayer.Play(MusicHolder<Song>.Get("MenuMusic"));

            Sound = SoundEffectHolder<SoundEffect>.Get("NextPrev");
        }

        private void CreateScene()
        {
            //Create skybox
            CreteSkybox();

            sceneObjects.Add(
                                new CustomRoad(4000, 6000, 100, 0, 0, "Road-Type-2")
                                .SetRotation(MathHelper.PiOver2)
                                .SetTranslation(new Vector3(0, 0, 2000))
                                .Build()
                                .To3DModel()
            );

            woodBall = new WoodBall(null, new Vector3(1300, 1550, 400));
            woodBall.UpdateForMenu(Matrix.CreateTranslation(new Vector3(2500, 550, 1000)), Matrix.Identity, Matrix.CreateScale(2f));
            sceneObjects.Add(woodBall);

            metalBall = new MetalBall(null, new Vector3(1300, 1550, 400));
            metalBall.UpdateForMenu(Matrix.CreateTranslation(new Vector3(2500, 550, 0)), Matrix.Identity, Matrix.CreateScale(2f));
            sceneObjects.Add(metalBall);

            plasticBall = new PlasticBall(null, new Vector3(1300, 1550, 400));
            plasticBall.UpdateForMenu(Matrix.CreateTranslation(new Vector3(2500, 550, -1000)), Matrix.Identity, Matrix.CreateScale(2f));
            sceneObjects.Add(plasticBall);

            selectedBall = plasticBall;
        }

        private void CreteSkybox()
        {
            var skyBox = ModelsHolder.Get("SkyBoxCube");
            var skyBoxTexture = TexturesHolder<TextureCube>.Get("SkyBox");
            var skyBoxEffect = EffectsHolder.Get("SkyBox");
            SkyBox = new SkyBox(skyBox, skyBoxTexture, skyBoxEffect, 5000);
        }

        public void CollisionDetected(CollidablePair pair, Collider.CollisionInformation info)
        {

        }

        private float velocity = 1500f;
        private float currentYPosition = 1550f;
        private int movementDirection = -1;

        private TimeSpan lastPressed;
        public void Upate(GameTime gameTime)
        {
            float time = ((float)gameTime.ElapsedGameTime.TotalSeconds);

            //Exit Game
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Game.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                MediaPlayer.Stop();
                if (selectedBall == plasticBall)
                {
                    
                    this.Game.StartGameplay(Ball.Plastic);
                }
                else if (selectedBall == woodBall)
                {
                    this.Game.StartGameplay(Ball.Wood);
                }
                else if (selectedBall == metalBall)
                {
                    this.Game.StartGameplay(Ball.Metal);
                }

                return;
            }

            if ((gameTime.TotalGameTime - lastPressed).TotalMilliseconds > 300)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    Sound.Play();
                    if (selectedBall == plasticBall)
                    {
                        selectedBall.UpdateForMenu(Matrix.CreateTranslation(new Vector3(2500, 550, -1000)), Matrix.Identity, Matrix.CreateScale(2f));
                        selectedBall = metalBall;
                    }
                    else if (selectedBall == woodBall)
                    {
                        selectedBall.UpdateForMenu(Matrix.CreateTranslation(new Vector3(2500, 550, 1000)), Matrix.Identity, Matrix.CreateScale(2f));
                        selectedBall = plasticBall;
                    }
                    else if (selectedBall == metalBall)
                    {
                        selectedBall.UpdateForMenu(Matrix.CreateTranslation(new Vector3(2500, 550, 0)), Matrix.Identity, Matrix.CreateScale(2f));
                        selectedBall = woodBall;
                    }
                    lastPressed = gameTime.TotalGameTime;
                    
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    Sound.Play();
                    if (selectedBall == plasticBall)
                    {
                        selectedBall.UpdateForMenu(Matrix.CreateTranslation(new Vector3(2500, 550, -1000)), Matrix.Identity, Matrix.CreateScale(2f));
                        selectedBall = woodBall;
                    }
                    else if (selectedBall == woodBall)
                    {
                        selectedBall.UpdateForMenu(Matrix.CreateTranslation(new Vector3(2500, 550, 1000)), Matrix.Identity, Matrix.CreateScale(2f));
                        selectedBall = metalBall;
                    }
                    else if (selectedBall == metalBall)
                    {
                        selectedBall.UpdateForMenu(Matrix.CreateTranslation(new Vector3(2500, 550, 0)), Matrix.Identity, Matrix.CreateScale(2f));
                        selectedBall = plasticBall;
                    }
                    lastPressed = gameTime.TotalGameTime;
                }
            }

            currentYPosition = selectedBall.Position.Y;
            var offset = velocity * time * movementDirection;
            var newYPosition = currentYPosition + offset;


            if (newYPosition > 1600 && movementDirection == 1)
            {
                movementDirection = -1;

            }
            else if (newYPosition < 600 && movementDirection == -1)
            {
                movementDirection = 1;
            }


            selectedBall.UpdateForMenu(Matrix.CreateTranslation(new Vector3(selectedBall.Position.X, newYPosition, selectedBall.Position.Z)), Matrix.Identity, Matrix.CreateScale(2f));


            Camera.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);


            DrawSkyBox(Camera);

            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (Model3D m in sceneObjects)
            {
                EffectsHolder.Get("LightEffect").Parameters["eyePosition"].SetValue(Camera.Position);
                EffectsHolder.Get("LightEffect").Parameters["lightPosition"].SetValue(new Vector3(4000, 1000, 500));
                EffectsHolder.Get("LightEffect").Parameters["ambientColor"].SetValue(new Vector3(1, 1, 1));
                EffectsHolder.Get("LightEffect").Parameters["diffuseColor"].SetValue(new Vector3(0.5f, 0.1f, 0f));
                EffectsHolder.Get("LightEffect").Parameters["specularColor"].SetValue(new Vector3(0.5f, 0.1f, 0f));
                EffectsHolder.Get("LightEffect").Parameters["hasEnviroment"].SetValue(0f);
                m.Draw(gameTime, Camera.View, Camera.Projection, "NormalMapping");
            }


            SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateScale(1f));
            SpriteBatch.DrawString(font, "BOUNCING BALL", TitlePosition, Color.Black);
            SpriteBatch.End();

            SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullNone);
            SpriteBatch.DrawString(font, "Use A y D para selecionar la bola y luego presione enter", TextPosition, Color.Black);
            SpriteBatch.End();

            DrawCenterText("Aceleracion         : " + selectedBall.Aceleracion + "/10", 60, 0.85f);
            DrawCenterText("Salto               : " + selectedBall.Salto + "/10", 80, 0.85f);
            DrawCenterText("Freno               : " + selectedBall.Freno + "/10", 100, 0.85f);
            DrawCenterText("Control             : " + selectedBall.Control + "/10", 120, 0.85f);
        }

        public void DrawRightText(string msg, float Y, float escala)
        {
            var W = GraphicsDevice.Viewport.Width;
            var H = GraphicsDevice.Viewport.Height;
            var size = font.MeasureString(msg) * escala;
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null,
                Matrix.CreateScale(escala) * Matrix.CreateTranslation(W - size.X - 20, Y, 0));
            SpriteBatch.DrawString(font, msg, new Vector2(0, 0), Color.Blue);
            SpriteBatch.End();
        }

        public void DrawCenterText(string msg, float Y, float escala)
        {
            var W = GraphicsDevice.Viewport.Width;
            var H = GraphicsDevice.Viewport.Height;
            var size = font.MeasureString(msg) * escala;
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null,
                Matrix.CreateScale(escala) * Matrix.CreateTranslation((W - size.X) / 2, H - Y, 0));
            SpriteBatch.DrawString(font, msg, new Vector2(0, 0), Color.Black);
            SpriteBatch.End();
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

        public void Dispose()
        {
            
        }


    }
}
