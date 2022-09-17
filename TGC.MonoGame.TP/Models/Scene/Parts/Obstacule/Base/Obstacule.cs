using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Utilities;

namespace TGC.MonoGame.TP.Models.Scene.Parts.Obstacule.Base
{
    public abstract class Obstacule : Model3D
    {
        protected Vector3 startPosition;
        protected Vector3 currentPosition;

        protected Obstacule(ContentManager content, string pathModel) : base(content, pathModel)
        {
            startPosition = new Vector3(0, 0, 0);
            currentPosition = startPosition;
        }

        public Obstacule SetPositionFromOrigin(Vector3 position)
        {
            startPosition = position;
            currentPosition = startPosition;
            TranslationMatrix = Matrix.CreateTranslation(position);
            return this;
        }

        public Obstacule Rotate(float angle)
        {
            RotationMatrix = Matrix.CreateRotationY(angle);
            return this;
        }

    }

}
