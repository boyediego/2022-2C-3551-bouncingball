using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene.Builder;

namespace TGC.MonoGame.TP.Models.Scene
{
    public class Scenario : IGameModel
    {
        private List<Model3D> sceneObjects;

        public void CreateModel(ContentManager content)
        {
            SceneBuilder builder = new SimpleSceneBuilder(content);
            builder
                .StartRoad()
                .AddForward(2)
                .AddLeft(3)
                .AddBackward(3)
                .AddLeft(3)
                .AddForward(8)
                .Up()
                .AddLeft(10)
                .AddBackward(3)
                .AddRight(3)
                .Up()
                .Up()
                .AddForward(8)
                .Down()
                .AddRight(3)
                ;


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
           //UPDATE OBJECT WITH MOVENTS
        }
    }
}
