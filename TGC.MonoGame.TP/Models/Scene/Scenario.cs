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
                    .addSideSpace(Vector3.Left, 500)
                    .addPlataform(new CustomRoad(3000, 2000, 100, 0, 0, "Plataform-Type-1"))
                    .addForwardSpace(500)
                    .addVerticalSpace(200)
                    .addPlataform(new CustomRoad(3000, 2000, 100, 0, 0, "Plataform-Type-1"))
                    .addForwardSpace(500)
                    .addVerticalSpace(200)
                    .addPlataform(new CustomRoad(3000, 2000, 100, 0, 0, "Plataform-Type-1"))
                    .addForwardSpace(500)
                    .addVerticalSpace(200)
                    .addPlataform(new CustomRoad(3000, 2000, 100, 0, 0, "Plataform-Type-1"))
                    .addPowerup(new IncreaseSpeed().Build(GameParams.PowerupSize), Vector3.Zero)
                    .addTramo(new CustomRoad(3000, 10000, 100, 0, 0, "Road-Type-2"));

            for (int x = 0; x < 2500; x += 100)
            {
                customBuilder.addObstacule(new CubeFixedObstacule().Build(100).SetInitialOffset(new Vector3(1450, 50, x)));
            }

            for (int x = 0; x < 2500; x += 100)
            {
                customBuilder.addObstacule(new CubeFixedObstacule().Build(100).SetInitialOffset(new Vector3(-1450, 50, x)));
            }

            for (int y = 100; y < 1000; y += 200)
            {
                for (int x = -1300; x < 1300; x += 200)
                {
                    if ((x <= -200 || x > 400) || (y <= 300 || y > 700))
                    {
                        customBuilder.addObstacule(new CubeFixedObstacule().Build(200).SetInitialOffset(new Vector3(x, y, 1250)));
                    }
                }
            }
            customBuilder.addForwardSpace(1000)
                .addVerticalSpace(300)
                .addPlataform(new CustomRoad(2000, 2000, 100, 0, 0, "Plataform-Type-1"))
                .addVerticalSpace(300)
                .addPlataform(new CustomRoad(2000, 2000, 100, 0, 0, "Plataform-Type-1"))
                .addForwardSpace(1000)
                .Rotate90Degress()
                .addForwardSpace(3000)
                .addPlataform(new CustomRoad(5000, 6000, 100, 0, 0, "Road-Type-2"));

            for (int x = 0; x < 2500; x += 100)
            {
                customBuilder.addObstacule(new CubeFixedObstacule().Build(100).SetInitialOffset(new Vector3(x, 50, 2450)));
            }

            for (int x = 0; x < 2500; x += 100)
            {
                customBuilder.addObstacule(new CubeFixedObstacule().Build(100).SetInitialOffset(new Vector3(x, 50, -2450)));
            }

            customBuilder.addObstacule(
                                new CubeObstacule()
                                .ChangeTexture("Marble", 0.4f, 0.8f, 0.87f, 32f)
                                .Build(90)
                                .SetMovementDirection(Vector3.Left)
                                .SetSpeed(12500)
                                .SetMaxMovement(2500)
                                .SetInitialOffset(new Vector3(-1500 / 2f, 150, 650)))
                .addObstacule(
                                new CubeObstacule()
                                .ChangeTexture("Marble", 0.4f, 0.8f, 0.87f, 32f)
                                .Build(90)
                                .SetMovementDirection(Vector3.Right)
                                .SetSpeed(14500)
                                .SetMaxMovement(1500)
                                .SetInitialOffset(new Vector3(-1000, 180, 0)))
                .addObstacule(
                                new CubeObstacule()
                                .ChangeTexture("Marble", 0.4f, 0.8f, 0.87f, 32f)
                                .Build(90)
                                .SetMovementDirection(Vector3.Right)
                                .SetSpeed(12500)
                                .SetMaxMovement(1900)
                                .SetInitialOffset(new Vector3(-1000, 380, 0)))
                .addObstacule(
                                new CubeObstacule()
                                .ChangeTexture("Marble", 0.4f, 0.8f, 0.87f, 32f)
                                .Build(90)
                                .SetMovementDirection(Vector3.Right)
                                .SetSpeed(16500)
                                .SetMaxMovement(2600)
                                .SetInitialOffset(new Vector3(1000, 380, 0)))
                    .addObstacule(
                                new CubeObstacule()
                                .ChangeTexture("Marble", 0.4f, 0.8f, 0.87f, 32f)
                                .Build(90)
                                .SetMovementDirection(Vector3.Right)
                                .SetSpeed(18500)
                                .SetMaxMovement(4600)
                                .SetInitialOffset(new Vector3(1000, 380, 150)))
                    .addObstacule(
                                new CubeObstacule()
                                .ChangeTexture("Marble", 0.4f, 0.8f, 0.87f, 32f)
                                .Build(90)
                                .SetMovementDirection(Vector3.Right)
                                .SetSpeed(9500)
                                .SetMaxMovement(4600)
                                .SetInitialOffset(new Vector3(500, 120, -150)))
                    .addObstacule(
                                new CubeObstacule()
                                .ChangeTexture("Marble", 0.4f, 0.8f, 0.87f, 32f)
                                .Build(90)
                                .SetMovementDirection(Vector3.Left)
                                .SetSpeed(11200)
                                .SetMaxMovement(3200)
                                .SetInitialOffset(new Vector3(700, 120, -350)))
                    .addObstacule(
                                new CubeObstacule()
                                .ChangeTexture("Marble", 0.4f, 0.8f, 0.87f, 32f)
                                .Build(90)
                                .SetMovementDirection(Vector3.Left)
                                .SetSpeed(9200)
                                .SetMaxMovement(4200)
                                .SetInitialOffset(new Vector3(1100, 150, -50)))
                .addTramo(new CustomRoad(5000, 3000, 100, 0, 0, "Road-Type-2"))
                .addPowerup(new ExtraJump().Build(GameParams.PowerupSize), Vector3.Zero)
                .addTramo(new CustomRoad(5000, 3000, 100, 900, 0, "Road-Type-2"))
                .addTramo(new CustomRoad(5000, 3000, 100, 0, 0, "Road-Type-2"))
                .addCheckpoint(5000)
                .addPlataform(new CustomRoad(3000, 2000, 100, 0, 0, "Plataform-Type-1"))
                .Rotate90DegressClockwise()
                .addSideSpace(Vector3.Left, 1500)
                .addForwardSpace(1000)
                .addVerticalSpace(200)
                .addPlataform(new CustomRoad(3000, 2000, 100, 0, 0, "Plataform-Type-1"))
                .addVerticalSpace(200)
                .addPlataform(new CustomRoad(3000, 2000, 100, 0, 0, "Plataform-Type-1"))
                .addVerticalSpace(-200)
                .addPlataform(new CustomRoad(3000, 2000, 100, 0, 0, "Plataform-Type-1"))
                .addVerticalSpace(-200)
                .addPlataform(new CustomRoad(3000, 2000, 100, 0, 0, "Plataform-Type-1"))
                .addForwardSpace(250)
                .addSideSpace(Vector3.Left, 750)
                .addTramo(new CustomRoad(2000, 15000, 100, 0, 0, "Road-Type-2"));

            Random rnd = new Random((int)DateTime.Now.Ticks);
            for (int z = -5000; z < 6000; z += 400)
            {
                int offsetZ = rnd.Next(10, 80);
                int obstacleSize = rnd.Next(100, 200);
                float offsetY = obstacleSize / 2;
                

                int obstacleInLine = rnd.Next(1, 4);

                for (int countLine = 0; countLine < obstacleInLine; countLine++)
                {
                    float inclinacion = Convert.ToSingle((Math.PI * 2) * rnd.NextDouble());
                    int offsetX = rnd.Next(0, 2000) - 1000;
                    customBuilder.addObstacule(new CubeFixedObstacule().Build(obstacleSize).Rotate(inclinacion).SetInitialOffset(new Vector3(offsetX, offsetY, z + offsetZ)));
                }
            }

            customBuilder.addTramo(new CustomCurvedRoad(new Vector3(0, 100, 0), new Vector3(5000, 0, 5000), 3000, 100, "Road-Type-2"))
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
                if (!startRestore && current == model)
                {
                    startRestore = true;
                }

                if (startRestore && model is Powerup)
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
