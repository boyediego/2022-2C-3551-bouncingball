﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene.Builder;
using TGC.MonoGame.TP.Models.Scene.Parts;
using TGC.MonoGame.TP.Models.Scene.Parts.Obstacule;

namespace TGC.MonoGame.TP.Models.Scene
{
    public class Scenario : IGameModel
    {
        private List<Model3D> sceneObjects;

        public List<Model3D> models { get { return this.sceneObjects; } }

        public Scenario(ContentManager content)
        {
            CreateModel(content);
        }

        public void CreateModel(ContentManager content)
        {
              SceneBuilder builder = new SimpleSceneBuilder(content);
             builder
                 .StartRoad()
                 .AddForward()
                 .AddObstacule(new CubeObstacule(content).SetMovementDirection(Vector3.Left).SetSpeed(2500).SetMaxMovement(750))
                 .AddForward()
                 .AddLeft(3)
                 .AddBackward(3)
                 .AddLeft(3)
                 .AddLeft()
                 .AddObstacule(new CubeObstacule(content).SetMovementDirection(Vector3.Forward).SetSpeed(2500).SetMaxMovement(450))
                 .AddLeft()
                 .AddForward(6)
                 .AddForward()
                 .AddObstacule(new SphereObstacule(content).SetMovementDirection(Vector3.Up).SetSpeed(1500).SetMaxMovement(450).Up(500))
                 .AddForward()
                 .AddForward(2)
                 .Up()
                 .AddLeft(10)
                 .AddBackward(3)
                 .AddRight(3)
                 .Up()
                 .Up()
                 .AddForward(8)
                 .Down()
                 .AddRight(3)
                 .StartRoad(new Vector3(0,590,1500))
                 .AddForward(8)
                 .AddRight(3)
                 .AddBackward(3)
                 .AddRight(5);

            /*SceneBuilder builder = new SimpleSceneBuilder(content);
            builder
                .StartRoad()
                .AddForward(4)
                .AddLeft(1)
                .AddBackward(4)
                .AddLeft(1)
                .AddForward(4)
                .AddObstacule(new CubeObstacule(content).SetMovementDirection(Vector3.Left).SetSpeed(2500).SetMaxMovement(750));*/


            sceneObjects = builder.GetScene();
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            foreach(Model3D model in sceneObjects)
            {
                model.Draw(gameTime, view, projection); 
            }
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, List<IGameModel> otherInteractiveObjects)
        {
            foreach (Model3D model in sceneObjects)
            {
                model.Update(gameTime, keyboardState, otherInteractiveObjects);
            }
        }
    }
}
