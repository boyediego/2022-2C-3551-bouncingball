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
        public Vector3 InitialOffset;

        protected Obstacule(ContentManager content, string pathModel) : base(content, pathModel)
        {
            InitialOffset = new Vector3(0, 0, 0);
            startPosition = new Vector3(0, 0, 0);
            currentPosition = startPosition;
        }

        public override bool IsGround { get { return false; } }

        public Obstacule SetInitialOffset(Vector3 v)
        {
            this.InitialOffset = v;
            return this;
        }

        public Obstacule Up(float amount)
        {
            this.InitialOffset += Vector3.Up * amount;
            return this;
        }

        public Obstacule Down(float amount)
        {
            this.InitialOffset += Vector3.Down * amount;
            return this;
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
