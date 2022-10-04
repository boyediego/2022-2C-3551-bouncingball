using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Utilities;

namespace TGC.MonoGame.TP.Models.Scene.Parts
{
    public class CurveRoad : Model3D
    {
        private Vector3 color;


        public override int PhysicsType
        {
            get { return PhysicsTypeHome.Static; }
        }

        public CurveRoad(ContentManager content) : base(content, "scene/basics/curva")
        {
        }

        public override void CreateModel(ContentManager content)
        {
            Effect = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            color = new Vector3(1, 0, 0);
            SetEffect(Effect);
        }

        public void SetPositionFromOrigin(Vector3 position)
        {
            TranslationMatrix = Matrix.CreateTranslation(position);
        }

        public void Rotate(float angle)
        {
            RotationMatrix = Matrix.CreateRotationY(angle);
        }
        public override void SetCustomEffectParameters(Effect effect)
        {
            effect.Parameters["DiffuseColor"].SetValue(color);
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
