﻿using System;
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
using System.Diagnostics;
using TGC.MonoGame.TP.UI;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace TGC.MonoGame.TP
{
    public class TGCGame : Game
    {


        private IRenderUI currentUI = null;
        private IRenderUI previousUI = null;

        #region Initialize GameParams & Content
        public TGCGame()
        {
            SharedObjects.graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //Set rasterizerState
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.Opaque;

            // Configuro el tamaño de la pantalla
            SharedObjects.graphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            SharedObjects.graphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            SharedObjects.graphicsDeviceManager.ApplyChanges();

            //Attach to detection collider events
            CollisionDetected += Collider_CollisionDetected;

          
            base.Initialize();
        }

        

        protected override void LoadContent()
        {
            //Load resources
            PreloadResources();

            //Load menu
            currentUI = new MenuUI(this);

            //Base calll
            base.LoadContent();
        }

        private void PreloadResources()
        {
            LoadModels();
            LoadTextures();
            LoadEffects();
            LoadFonts();
            LoadMusic();
            LoadSoundEffects();
        }

        private void LoadFonts()
        {
            //Load fonts
            FontHolder<SpriteFont>.Load(Content, "GameFont", "CascadiaCode/CascadiaCodePL");
        }

        private void LoadMusic()
        {
            MusicHolder<Song>.Load(Content, "MenuMusic", "menu-music");
            MusicHolder<Song>.Load(Content, "GamePlay", "gameplay-music");
        }

        private void LoadSoundEffects()
        {
            SoundEffectHolder<SoundEffect>.Load(Content, "Powerup-Collected", "powerup-collected");
            SoundEffectHolder<SoundEffect>.Load(Content, "Jump", "jump");
            SoundEffectHolder<SoundEffect>.Load(Content, "Checkpoint", "Checkpoint");
            SoundEffectHolder<SoundEffect>.Load(Content, "Fall", "fall");
            SoundEffectHolder<SoundEffect>.Load(Content, "NextPrev", "next-prev");
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
            TexturesHolder<Texture2D>.Load(Content, "Marble-Road", "roads/Marble-1");
            TexturesHolder<Texture2D>.Load(Content, "Marble-Road-Normal", "roads/Marble-1-normal");
            TexturesHolder<Texture2D>.Load(Content, "Metal-Finish", "roads/Metal-Finish");
            TexturesHolder<Texture2D>.Load(Content, "Metal-Finish-Normal", "roads/Metal-Finish-normal");

            //Powerups Textures
            TexturesHolder<Texture2D>.Load(Content, "Powerup", "powerups/Powerup");
            TexturesHolder<Texture2D>.Load(Content, "Powerup-Normal", "powerups/Powerup-normal");

            //Obstacule Textures
            TexturesHolder<Texture2D>.Load(Content, "Stone-Type-1", "obstacules/Stone-Type-1");
            TexturesHolder<Texture2D>.Load(Content, "Stone-Type-1-Normal", "obstacules/Stone-Type-1-normal");
            TexturesHolder<Texture2D>.Load(Content, "Marble", "obstacules/Marble");
            TexturesHolder<Texture2D>.Load(Content, "Marble-Normal", "obstacules/Marble-normal");


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

        private void Collider_CollisionDetected(CollidablePair pair, CollisionInformation info)
        {
            currentUI.CollisionDetected(pair, info);
        }
        #endregion


        #region Game flows
        internal void StartGameplay(int selectedPlayer)
        {
            currentUI = new GamePlay(this,selectedPlayer);
        }

        internal void EndGameplay(GamePlay gamePlay)
        {
            currentUI = new MenuUI(this);
        }
        #endregion


        #region Game Draw & Update
        protected override void Update(GameTime gameTime)
        {
            currentUI.Upate(gameTime);

            if (previousUI != null && previousUI != currentUI)
            {
                previousUI.Dispose();
            }

            previousUI = currentUI;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            currentUI.Draw(gameTime);
        }
        #endregion

        #region End Game
        protected override void UnloadContent()
        {
     
            // Libero los recursos.
            Content.Unload();
            base.UnloadContent();
        }


        #endregion


    }
}