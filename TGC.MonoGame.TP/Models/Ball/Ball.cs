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

namespace TGC.MonoGame.TP.Models.Ball
{
    public class Ball : Model3D
    {
        
        private Vector3 color;
        private Random r = new Random((int)DateTime.Now.Ticks);

        public Ball(ContentManager content) : base(content, "balls/sphere")
        {
        }

        public override void CreateModel(ContentManager content)
        {
            Effect = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            color = new Vector3(1, 0, 0);
            SetEffect(Effect);
            base.ScaleMatrix = Matrix.CreateScale(0.55f);
            base.TranslationMatrix = Matrix.CreateTranslation(new Vector3(0, 130, 0));
        }

        public void SetPositionFromOrigin(Vector3 position)
        {
            TranslationMatrix += Matrix.CreateTranslation(position);
        }


        public override void SetCustomEffectParameters(Effect effect)
        {
            effect.Parameters["DiffuseColor"].SetValue(color);
        }

    }

}
