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

namespace TGC.MonoGame.TP.Models.Ball
{
    public class Ball : Model3D
    {


        private Random r = new Random((int)DateTime.Now.Ticks);
        private static Texture2D texture;//FIXME
        private float currentSpinAngle;

        public Ball(ContentManager content) : base(content, "balls/sphere1")
        {
        }

        public override void CreateModel(ContentManager content)
        {
            Effect = content.Load<Effect>(ContentFolderEffects + "TextureShader");
            var effect = Model.Meshes.FirstOrDefault().Effects.FirstOrDefault() as BasicEffect;
            if (effect != null) 
            {
                texture = effect.Texture;
            }

            SetEffect(Effect);
            base.ScaleMatrix = Matrix.CreateScale(0.55f);
            base.TranslationMatrix = Matrix.CreateTranslation(new Vector3(0, 130, 0));
        }

        public void SetPositionFromOrigin(Vector3 position)
        {
            TranslationMatrix = Matrix.CreateTranslation(position);
            this.position = position;
        }


        public override void SetCustomEffectParameters(Effect effect)
        {
            Effect.Parameters["ModelTexture"].SetValue(texture);
        }

        private float angle;
        public Vector3 position;
        private Vector3 yPosition = new Vector3(0, 130, 0);
        private float speed = 0;

        public override void Update(GameTime gameTime, KeyboardState keyboardState, List<IGameModel> otherInteractiveObjects)
        {
            


            var da = 0.01f;
            if (keyboardState.IsKeyDown(Keys.Left)) { angle -= da; }
            if (keyboardState.IsKeyDown(Keys.P)) { angle += da; }

            
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                speed = 10;
            }

            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                speed = -10;
            }
            else
            {
                speed = 0;
            }

            float dirX = (float)Math.Sin(-angle);
            float dirZ = (float)Math.Cos(-angle);

            position += new Vector3(dirX, 0, dirZ) * -speed;
            position.Y = 130;

            Spin(gameTime, speed);

            RotationMatrix *= Matrix.CreateFromAxisAngle(Vector3.Down, angle);
            TranslationMatrix = Matrix.CreateTranslation(position);
        }


        private void Spin(GameTime gameTime, float speed)
        {
            float time = ((float)gameTime.ElapsedGameTime.Milliseconds) / 1000;
            currentSpinAngle += time * speed;
            this.RotationMatrix = Matrix.CreateRotationX(-currentSpinAngle);
        }


    }

}
