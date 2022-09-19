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
        private static Texture2D texture;

        
        public Road(ContentManager content) : base(content, "scene/basics/road2")
        {
        }

        public override void CreateModel(ContentManager content)
        {
            Effect = content.Load<Effect>(ContentFolderEffects + "TextureShader");
            var effect = Model.Meshes.FirstOrDefault().Effects.FirstOrDefault() as BasicEffect;
            if (effect != null) //TODO LOAD ALL MODELS OUTSIDE AND USE IT
            {
                texture = effect.Texture;
            }

            //base.ScaleMatrix = Matrix.CreateScale(0.55f);
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
            Effect.Parameters["ModelTexture"].SetValue(texture);
        }
    }

}
