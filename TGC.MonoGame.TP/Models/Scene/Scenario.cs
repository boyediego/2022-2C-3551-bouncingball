using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Numerics;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene.Builder;
using TGC.MonoGame.TP.Models.Scene.Parts;
using TGC.MonoGame.TP.Models.Scene.Parts.Obstacule;
using TGC.MonoGame.TP.Models.Scene.Parts.Powerups;
using TGC.MonoGame.TP.Models.Scene.Parts.Roads;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace TGC.MonoGame.TP.Models.Scene
{
    public class Scenario : IGameModel
    {
        private List<Model3D> sceneObjects;

        public List<Model3D> models { get { return this.sceneObjects; } }

        public Scenario()
        {
            SetEffectAndTextures(null);
        }

        public void SetEffectAndTextures(Model model)
        {
            //TODO
            /*  SceneBuilder builder = new SimpleSceneBuilder();
             builder
                    .StartRoad()
                  .AddForward(5)
                  .AddLeft(1)
                  .AddBackward(80)
                  .AddLeft(1)
                  .AddForward(80)
                  .AddLeft(1)
                  .AddBackward(80)
                  .AddLeft(1)
                  .AddForward(80)
                  .AddLeft(1)
                  .AddBackward(80)
                  .AddLeft(1)
                  .AddForward(80)
                  .AddLeft(1)
                  .AddBackward(80)
                  .AddLeft(1)
                  .AddForward(40);
           ;
         sceneObjects = builder.GetScene();*/

            sceneObjects = new List<Model3D>();
            /*
              .AddObstacule(new CubeObstacule(content).SetMovementDirection(Vector3.Right).SetSpeed(12500).SetMaxMovement(750).SetInitialOffset(new Vector3(400, 0, 0)))
                 .AddPowerup(new ExtraJump(content))
             */

            CustomBuilder customBuilder = new CustomBuilder();
            customBuilder
                    .addTramo(new CustomRoad(2000, 5600, 100, 0, 0))
                    .addPowerup(new ExtraJump())
                    .addForwardSpace(1000)
                    .addVerticalSpace(450)
                    .addTramo(new CustomRoad(2000, 5000, 100, 0, 0))
                    .addTramo(new CustomRoad(2000, 2500, 100, 1000, 0))
                    .addTramo(new CustomRoad(2000, 3000, 100, 0, 0))
                    .addTramo(new CustomCurvedRoad(new Vector3(0, 100, 0), new Vector3(-5000, 100, 3000), 2000, 100))
                    .addSideSpace(Vector3.Right, 145)
                    .addPlataform(new CustomRoad(800, 800, 100, 0, 0))
                    .addVerticalSpace(450)
                    .addForwardSpace(300)
                    .addPlataform(new CustomRoad(800, 800, 100, 0, 0))
                    .addObstacule(new CubeObstacule().SetMovementDirection(Vector3.Right).SetSpeed(12500).SetMaxMovement(1750).SetInitialOffset(new Vector3(400, 0, 0)))
                    .addVerticalSpace(450)
                    .addForwardSpace(300)
                    .addPlataform(new CustomRoad(800, 800, 100, 0, 0))
                    .addVerticalSpace(450)
                    .addForwardSpace(300)
                    .addPlataform(new CustomRoad(800, 800, 100, 0, 0))
                    .addTramo(new CustomRoad(2000, 9000, 100, 0, 0))
                    .addCheckpoint(2000)
                    .addTramo(new CustomCurvedRoad(new Vector3(0, 100, 0), new Vector3(-5000, 100, 3000), 2000, 100));
            sceneObjects.AddRange(customBuilder.GetScene());

            /*

            //Rect path
            Vector3 position = new Vector3(0,0,0);
            CustomRoad customRoad = new CustomRoad(content, TGCGame.Graphics.GraphicsDevice, 2000, 500, 100, 0, 0);
            customRoad.SetTranslation(0, 200, 0);
            sceneObjects.Add(customRoad);
            position += new Vector3(0, 200, 500);

            customRoad = new CustomRoad(content, TGCGame.Graphics.GraphicsDevice, 2000, 5000, 100, 0, 0);
            customRoad.SetTranslation(position);
            
            sceneObjects.Add(customRoad);
            position += new Vector3(0, 0, 5000);

            customRoad = new CustomRoad(content, TGCGame.Graphics.GraphicsDevice, 2000, 2500, 100, 1000, 0);
            customRoad.SetTranslation(position);
            sceneObjects.Add(customRoad);
            position += new Vector3(0, 1000, 2500);

            customRoad = new CustomRoad(content, TGCGame.Graphics.GraphicsDevice, 2000, 3000, 100, 0, 0);
            customRoad.SetTranslation(position);
            sceneObjects.Add(customRoad);
            position += new Vector3(0, 0, 3000);


            //**********TO LEFT*************************************
            
            CustomCurvedRoad curved = new CustomCurvedRoad(content, TGCGame.Graphics.GraphicsDevice, new Vector3(0, 100, 0), new Vector3(5000, 100, 7000), 2000, 100);
            curved.SetTranslation(position);
            sceneObjects.Add(curved);
            position = Vector3.Transform(new Vector3(5000, 0, 7000), Matrix.CreateTranslation(position));

            customRoad = new CustomRoad(content, TGCGame.Graphics.GraphicsDevice, 1414.29431f, 9000, 100, 0, 0);
            customRoad.SetTranslation(position);
            customRoad.SetRotation(0.785341f);
            sceneObjects.Add(customRoad);
            position += new Vector3(0, 0, 3000);


            //**********TO RIGHT*************************************
            
            CustomCurvedRoad curved = new CustomCurvedRoad(content, TGCGame.Graphics.GraphicsDevice, new Vector3(0, 100, 0), new Vector3(-5000, 100, 7000), 2000, 100);
            curved.SetTranslation(position);
            sceneObjects.Add(curved);
            position = Vector3.Transform(new Vector3(-5000, 0, 7000), Matrix.CreateTranslation(position));

            customRoad = new CustomRoad(content, TGCGame.Graphics.GraphicsDevice, 1648.21619f, 9000, 100, 0, 0);
            customRoad.SetTranslation(position);
            customRoad.SetRotation(-0.6021707f);
            sceneObjects.Add(customRoad);
            position += new Vector3(0, 0, 3000);


            */

        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            foreach (Model3D model in sceneObjects)
            {
                model.Draw(gameTime, view, projection);
            }
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            foreach (Model3D model in sceneObjects)
            {
                model.Update(gameTime, keyboardState);
            }
        }


    }
}
