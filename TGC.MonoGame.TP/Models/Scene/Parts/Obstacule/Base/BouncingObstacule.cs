using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
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
using TGC.MonoGame.TP.Utilities;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Quaternion = Microsoft.Xna.Framework.Quaternion;
using System.Diagnostics;

namespace TGC.MonoGame.TP.Models.Scene.Parts.Obstacule.Base
{
    public abstract class BouncingObstacule : Obstacule
    {

        protected Vector3 color;
        protected Vector3 movementDirection;
        protected float speed;
        protected float maxMovementUnits;

        private Simulation simulation;
        private BodyDescription bodyDescription;
        private BodyHandle bodyHandle;

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

        private int step=0;
        public override void Update(GameTime gameTime, KeyboardState keyboardState, List<IGameModel> otherInteractiveObjects)
        {
            float time = (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

            var bodyReference = simulation.Bodies.GetBodyReference(bodyHandle);

            Debug.WriteLine("D:" + Vector3.Distance(startPosition, currentPosition));
            Debug.WriteLine("Max:" + maxMovementUnits + " Step :" + step);

            if (Vector3.Distance(startPosition, currentPosition) > maxMovementUnits && step==2)
            {
                movementDirection *= -1;
                step = 1;
            }
            else if (Vector3.Distance(startPosition, currentPosition) < 50 && (step==1 || step==0))
            {
                movementDirection *= -1;
                step = 2;

            }




                var position = bodyReference.Pose.Position;
            var quaternion = bodyReference.Pose.Orientation;


            bodyReference.Velocity.Linear = movementDirection.ToNumericVector3() * speed * GameParams.ObstacleSpeedMultiplier * time;


            this.currentPosition = new Vector3(position.X, position.Y, position.Z);
            base.RotationMatrix = Matrix.CreateFromQuaternion(new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W));
            base.TranslationMatrix = Matrix.CreateTranslation(currentPosition);

            /*
            float time = (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

            if (Vector3.DistanceSquared(startPosition, currentPosition) > MathF.Pow(maxMovementUnits, 2))
            {
                movementDirection *= -1;
            }



            currentPosition += movementDirection * speed * GameParams.ObstacleSpeedMultiplier * time;
            TranslationMatrix = Matrix.CreateTranslation(currentPosition);
            */
        }

        protected abstract Boolean CheckCollision(GameTime gameTime, List<IGameModel> otherInteractiveObjects);

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
            this.simulation= simulation;
            var size = base.GetModelSize();
            var shape = new Box(size.X, size.Y, size.Z);
            var collidable = new CollidableDescription(simulation.Shapes.Add(shape), 0.1f);
            bodyDescription = BodyDescription.CreateKinematic(new RigidPose(startPosition.ToNumericVector3()), collidable, new BodyActivityDescription(0.01f));
            bodyHandle = simulation.Bodies.Add(bodyDescription);
            return bodyDescription;
        }

        public override int PhysicsType
        {
            get { return PhysicsTypeHome.Kinematic; }
        }
    }

}
