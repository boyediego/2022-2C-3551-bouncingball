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
    public class Road : Model3D
    {
        private Vector3 ObjectStartPosition;
        private float rotationAngle;

        private Vector3 color;
        private static Random r = new Random((int)DateTime.Now.Ticks);

        public Road(ContentManager content) : base(content, "scene/basics/calle")
        {
        }

        public override void CreateModel(ContentManager content)
        {
            Effect = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            color = new Vector3((float)r.NextDouble() , (float)r.NextDouble(), (float)r.NextDouble());
            SetEffect(Effect);
            
        }

        public void SetPositionFromOrigin(Vector3 position)
        {
            this.ObjectStartPosition = position;
            TranslationMatrix = Matrix.CreateTranslation(ObjectStartPosition);
        }

        public void Rotate(float angle)
        {
            rotationAngle = angle;
            RotationMatrix = Matrix.CreateRotationY(rotationAngle);
        }


        public override void SetCustomEffectParameters(Effect effect)
        {
            effect.Parameters["DiffuseColor"].SetValue(color);
        }
    }

}
