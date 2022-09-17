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
    public abstract class BouncingObstacule : Obstacule
    {

        protected Vector3 color;
        protected Vector3 movementDirection;
        protected float speed;
        protected float maxMovementUnits;

        private static Random r = new Random((int)DateTime.Now.Ticks);

        protected BouncingObstacule(ContentManager content, string pathModel) : base(content, pathModel)
        {
        }


        public override void CreateModel(ContentManager content)
        {
            LoadEffectAndParameters(content);
            SetEffect(Effect);
            movementDirection = Vector3.Forward;
            speed = 0;
            maxMovementUnits = 0;
        }

        protected abstract void LoadEffectAndParameters(ContentManager content);

        public BouncingObstacule SetMaxMovement(float maxUnits)
        {
            maxMovementUnits = maxUnits;
            return this;
        }

        public BouncingObstacule SetSpeed(float speed)
        {
            this.speed = speed;
            return this;
        }

      

        public BouncingObstacule SetMovementDirection(Vector3 direction)
        {
            movementDirection = direction;
            return this;
        }


        public override void Update(GameTime gameTime, KeyboardState keyboardState, List<IGameModel> otherInteractiveObjects)
        {
            float time = (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
            //TODO check collition to another object and change direction
            if (Vector3.DistanceSquared(startPosition, currentPosition) > MathF.Pow(maxMovementUnits, 2) || CheckCollision(gameTime, otherInteractiveObjects))
            {
                movementDirection *= -1;
            }

            currentPosition += movementDirection * speed * GameParams.ObstacleSpeedMultiplier * time;
            TranslationMatrix = Matrix.CreateTranslation(currentPosition);
        }

        protected abstract Boolean CheckCollision(GameTime gameTime, List<IGameModel> otherInteractiveObjects);

        public override void SetCustomEffectParameters(Effect effect)
        {
            effect.Parameters["DiffuseColor"].SetValue(color);
        }
    }

}
