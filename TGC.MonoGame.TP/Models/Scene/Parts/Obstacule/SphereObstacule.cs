using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene.Parts.Obstacule.Base;
using TGC.MonoGame.TP.Utilities;

namespace TGC.MonoGame.TP.Models.Scene.Parts.Obstacule
{
    public class SphereObstacule : BouncingObstacule
    {        
        public SphereObstacule(ContentManager content) : base(content, "balls/sphere")
        {
            base.ScaleMatrix = Matrix.CreateScale(1.5f);
        }

        public override bool IsGround { get { return false; } }

        protected override void LoadEffectAndParameters(ContentManager content)
        {
            Effect = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            color = new Vector3(0, 0, 1);
        }

        protected override bool CheckCollision(GameTime gameTime, List<IGameModel> otherInteractiveObjects)
        {
            //TODO IMPLEMENT
            return false;
        }

        public override int PhysicsType
        {
            get { return PhysicsTypeHome.Kinematic; }
        }

        public override StaticDescription GetStaticDescription(Simulation simulation)
        {
            throw new NotSupportedException();
        }

        public override BodyDescription GetBodyDescription(Simulation simulation)
        {
            throw new NotSupportedException();
        }

    }

}
