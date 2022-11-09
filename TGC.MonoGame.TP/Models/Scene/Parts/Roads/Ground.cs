using BepuPhysics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Utilities;

namespace TGC.MonoGame.TP.Models.Scene.Parts.Roads
{
    public abstract class Ground : Model3D
    {
        protected Ground(Model model) : base(model)
        {
        }

        public override bool IsGround { get { return true; } }
        public override int PhysicsType { get { return PhysicsTypeHome.Static; } }
        public override BodyDescription GetBodyDescription(Simulation simulation) { throw new NotSupportedException(); }
    }
}
