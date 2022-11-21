using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene.Builder;
using TGC.MonoGame.TP.Models.Scene.Parts;
using TGC.MonoGame.TP.Models.Scene.Parts.Checkpoints;
using TGC.MonoGame.TP.Models.Scene.Parts.Obstacule;
using TGC.MonoGame.TP.Models.Scene.Parts.Powerups;
using TGC.MonoGame.TP.Models.Scene.Parts.Roads;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace TGC.MonoGame.TP.Models.Scene
{
    public class Scenario : IGameModel, IScene
    {
        private List<Model3D> sceneObjects;

        public event EventHandler EndGame;

        public List<Model3D> models { get { return this.sceneObjects; } }
        public Vector3 LightPosition { get; set; }
        public Vector3 AmbientLightColor { get { return new Vector3(1, 1, 1); } }
        public Vector3 DiffuseLightColor { get { return new Vector3(0.5f, 0.1f, 0f); } }
        public Vector3 SpecularLightColor { get { return new Vector3(0.5f, 0.1f, 0f); } }

        public Scenario()
        {
            SetEffectAndTextures(null);
        }

        public void SetEffectAndTextures(Model model)
        {
            CustomBuilder customBuilder = new CustomBuilder();
            customBuilder
                    .addTramo(new CustomRoad(2000, 7600, 100, 0, 0, "Road-Type-2"))
                    .addObstacule(new CubeObstacule().Build(500).SetMovementDirection(Vector3.Left).SetSpeed(7000).SetMaxMovement(1500).SetInitialOffset(new Vector3(-1500 / 2f, 250, -1650)))
                    .addObstacule(new CubeObstacule().Build(500).SetMovementDirection(Vector3.Right).SetSpeed(5000).SetMaxMovement(1500).SetInitialOffset(new Vector3(1500 / 2f, 250, 0)))
                    .addObstacule(new CubeObstacule().Build(500).SetMovementDirection(Vector3.Left).SetSpeed(3000).SetMaxMovement(1500).SetInitialOffset(new Vector3(-1500 / 2f, 250, 650)))
                    .addPlataform(new CustomRoad(3000, 2000, 100, 0, 0, "Plataform-Type-1"))
                    .addPowerup(new ExtraJump().Build(GameParams.PowerupSize), Vector3.Zero)
                    .addPowerup(new IncreaseSpeed().Build(GameParams.PowerupSize), new Vector3(-1000, 0, -500))
                    .addForwardSpace(1000)
                    .addVerticalSpace(300)
                    .addPlataform(new CustomRoad(2000, 3000, 100, 0, 0, "Plataform-Type-1"))
                    .addCheckpoint(3000)
                    .addTramo(new CustomRoad(2000, 1600, 100, 0, 0, "Road-Type-2"))
                    .addPowerup(new ExtraJump().Build(GameParams.PowerupSize), Vector3.Zero)
                    .addTramo(new CustomCurvedRoad(new Vector3(0, 100, 0), new Vector3(5000, 100, 3000), 2000, 100, "Road-Type-2"))
                    .addTramo(new CustomCurvedRoad(new Vector3(0, 100, 0), new Vector3(5000, 100, 3000), 2000, 100, "Road-Type-2"))
                    .addObstacule(new CubeFixedObstacule().Build(100).SetInitialOffset(new Vector3(0, 100, 0)).Rotate(0.2f))
                    .addForwardSpace(1000)
                    .addPlataform(new CustomRoad(3000, 2000, 100, 0, 0, "Plataform-Type-1"))
                    .End(2000);
            sceneObjects = customBuilder.GetScene();


        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection, String technique)
        {
            foreach (Model3D model in sceneObjects)
            {
                model.Draw(gameTime, view, projection, technique);
            }
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            foreach (Model3D model in sceneObjects)
            {
                model.Update(gameTime, keyboardState);
            }
        }

        public void RestorePowerups(Checkpoint current)
        {
            Boolean startRestore = current == null;
            foreach (Model3D model in sceneObjects)
            {
                if(!startRestore && current == model)
                {
                    startRestore = true;
                }

                if(startRestore && model is Powerup)
                {
                    ((Powerup)model).Reset();
                }

            }
        }

        public void End()
        {
            EndGame?.Invoke(this, new EventArgs());
        }

    }
}
