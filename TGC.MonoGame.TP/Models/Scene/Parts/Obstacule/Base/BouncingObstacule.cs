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
        protected Vector3 movementDirection;
        protected float speed;
        protected float maxMovementUnits;

        protected Simulation simulation;
        protected BodyDescription bodyDescription;
        protected BodyHandle bodyHandle;

        protected BouncingObstacule(Model model) : base(model)
        {
            movementDirection = Vector3.Forward;
            speed = 0;
            maxMovementUnits = 0;
        }

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

        private Matrix _ExternalTransformation;
        public override Matrix ExternalTransformation
        {
            get { return _ExternalTransformation; }
            set
            {
                _ExternalTransformation = value;
                movementDirection = Vector3.Transform(movementDirection, ExternalTransformation);
                base.RotationMatrix = _ExternalTransformation;
            }
        }


        public BouncingObstacule SetMovementDirection(Vector3 direction)
        {
            movementDirection = direction;
            return this;
        }

        private int step = 0;
        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            float time = (float)gameTime.ElapsedGameTime.Milliseconds / 1000;

            var bodyReference = simulation.Bodies.GetBodyReference(bodyHandle);

            if (Vector3.Distance(startPosition, currentPosition) > maxMovementUnits && step == 2)
            {
                movementDirection *= -1;
                step = 1;
            }
            else if (Vector3.Distance(startPosition, currentPosition) < 50 && (step == 1 || step == 0))
            {
                movementDirection *= -1;
                step = 2;

            }

            var position = bodyReference.Pose.Position;
            var quaternion = bodyReference.Pose.Orientation;


            bodyReference.Velocity.Linear = movementDirection.ToNumericVector3() * speed * GameParams.ObstacleSpeedMultiplier * time;
            

            this.currentPosition = new Vector3(position.X, position.Y, position.Z);
            //base.RotationMatrix = Matrix.CreateFromQuaternion(new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W));
            base.TranslationMatrix = Matrix.CreateTranslation(currentPosition);
        }

        

        public override StaticDescription GetStaticDescription(Simulation simulation) { throw new NotSupportedException(); }
        public override int PhysicsType { get { return PhysicsTypeHome.Kinematic; } }
      

        public override void Collide(Model3D sceneObject)
        {
            //DO NNOTHING
        }

    }

}
