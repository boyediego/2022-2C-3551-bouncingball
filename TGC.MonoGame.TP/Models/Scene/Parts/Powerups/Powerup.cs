using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Players;
using TGC.MonoGame.TP.Utilities;

namespace TGC.MonoGame.TP.Models.Scene.Parts.Powerups
{
    public abstract class Powerup : Model3D
    {

        private Boolean taked = false;
        protected Simulation simulation;
        protected BodyDescription bodyDescription;
        protected BodyHandle bodyHandle;

        protected Powerup(Model model) : base(model)
        {
            base.TranslationMatrix = Matrix.Identity;
            base.RotationMatrix = Matrix.Identity;
        }

        public void SetPosition(Vector3 position)
        {
            base.TranslationMatrix = Matrix.CreateTranslation(position);
        }


        public override bool IsGround { get { return false; } }
        public override int PhysicsType { get { return PhysicsTypeHome.Kinematic; } }
        public override StaticDescription GetStaticDescription(Simulation simulation) { throw new NotSupportedException(); }

        public override void Collide(Model3D sceneObject)
        {
            //Only when player collide with the objetct
            if (!taked)
            {
                taked = true;
                ((Ball)sceneObject).Powerup(this);
                simulation.Bodies.Remove(this.bodyHandle);
            }
        }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection, String techniques)
        {
            if (!taked)
                this.DrawPowerUp(gameTime, view, projection);
        }

        public abstract void DrawPowerUp(GameTime gameTime, Matrix view, Matrix projection);
        public abstract void ApplyPowerUp(Ball ball);

    }
}
