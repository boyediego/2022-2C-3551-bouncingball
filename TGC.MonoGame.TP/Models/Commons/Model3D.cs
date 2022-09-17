using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TGC.MonoGame.TP.Utilities;

namespace TGC.MonoGame.TP.Models.Commons
{
    public abstract class Model3D : IGameModel
    {
        protected const string ContentFolder3D = "Models/";
        protected const string ContentFolderEffects = "Effects/";

        protected Model Model { get; set; }
        protected Effect Effect { get; set; }
        protected Matrix Translation;
        protected Matrix RotationMatrix;

        public Model3D(ContentManager content, String pathModel)
        {
            Model = content.Load<Model>(ContentFolder3D + pathModel);
            CreateModel(content);
        }

        public Vector3 GetModelSize()
        {
            return this.Model.GetBoundingBox(RotationMatrix).Max - this.Model.GetBoundingBox(RotationMatrix).Min;
        }

        public Vector3 GetTranlationFromOrigin()
        {
            return Translation.Translation;
        }

        public abstract void CreateModel(ContentManager content);

        public abstract void Draw(GameTime gameTime, Matrix view, Matrix projection);

        public virtual void Update(GameTime gameTime, KeyboardState keyboardState, List<IGameModel> otherInteractiveObjects)
        {
            //TODO IMPLEMTS IN CHILD CLASESS
        }
    }
}
