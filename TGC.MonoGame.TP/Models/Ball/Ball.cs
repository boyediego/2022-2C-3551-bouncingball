using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Utilities;
using NumericVector3 = System.Numerics.Vector3;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Quaternion = Microsoft.Xna.Framework.Quaternion;
using TGC.MonoGame.TP.Cameras;

namespace TGC.MonoGame.TP.Models.Ball
{
    public class Ball : Model3D
    {
        private static Texture2D texture;//FIXME

        private Simulation simulation;
        public BodyDescription BodyDescription { get; set; }
        public BodyHandle playerHanle { get; set; }
        private Vector3 PreviousVelocityDirection;
        
        protected float ForwardImpulse
        {
            get { return 30f;}
        }

        protected float BrakeForce
        {
            get { return 60f; }
        }

        protected float RotateForce
        {
            get { return 25f; }
        }


        public Ball(ContentManager content, Vector3 startPosition, Simulation Simulation) : base(content, "balls/sphere1")
        {
            this.simulation = Simulation;
            var position = new NumericVector3(startPosition.X, startPosition.Y, startPosition.Z);
            var boundingPlayer = this.GetBoundingSphere();
            var simulationPlayer = new Sphere(boundingPlayer.Radius - 60);
            this.BodyDescription = BodyDescription.CreateConvexDynamic(position, 1f, Simulation.Shapes, simulationPlayer);
            this.playerHanle = Simulation.Bodies.Add(this.BodyDescription);
            base.CurrentMovementDirection = new Vector3(0 , 0,  0);
            
        }

        public BoundingSphere GetBoundingSphere()
        {
            return this.Model.GetSphereFrom();
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
//            base.TranslationMatrix = Matrix.CreateTranslation(new Vector3(0, 1430, 0));
        }

        public override void SetCustomEffectParameters(Effect effect)
        {
            Effect.Parameters["ModelTexture"].SetValue(texture);
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, List<IGameModel> otherInteractiveObjects)
        {
            var bodyReference = simulation.Bodies.GetBodyReference(playerHanle);
            var position = bodyReference.Pose.Position;
            var quaternion = bodyReference.Pose.Orientation;

            var velocityVector = new Vector3(bodyReference.Velocity.Linear.X, 0, bodyReference.Velocity.Linear.Z);
            var velocityDirection = velocityVector;//.Abs();
            velocityDirection.Normalize();
            velocityDirection=velocityDirection.Abs();


            int fSign = -1; //velocityVector.Z < 0.01 ? -1 : 1;
            Debug.WriteLine(velocityVector);

            if (keyboardState.IsKeyDown(Keys.W))
            {
                bodyReference.Awake = true;
                bodyReference.ApplyLinearImpulse(velocityDirection.ToNumericVector3() * fSign*ForwardImpulse);
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                bodyReference.Awake = true;
                bodyReference.ApplyLinearImpulse(velocityDirection.ToNumericVector3() *-fSign* BrakeForce);
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                bodyReference.Awake = true;
                bodyReference.ApplyLinearImpulse(velocityDirection.PerpendicularCounterClockwiseIn2D().ToNumericVector3() * RotateForce);
            }

            if (keyboardState.IsKeyDown(Keys.X))
            {
                bodyReference.Awake = true;
                bodyReference.ApplyLinearImpulse(velocityDirection.PerpendicularClockwiseIn2D().ToNumericVector3() * RotateForce);
            }

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                bodyReference.Awake = true;
                bodyReference.ApplyLinearImpulse(Vector3.Up.ToNumericVector3() * 300);
            }

            base.RotationMatrix = Matrix.CreateFromQuaternion(new Quaternion(quaternion.X, quaternion.Y, quaternion.Z,quaternion.W));
            base.TranslationMatrix = Matrix.CreateTranslation(new Vector3(position.X, position.Y, position.Z));

            

            if (velocityVector.LengthSquared() > 0)
            {
                PreviousVelocityDirection = velocityVector;
                PreviousVelocityDirection.Normalize();
            }


            base.CurrentMovementDirection = Vector3.Lerp(base.CurrentMovementDirection,- PreviousVelocityDirection, 0.05f);
            
        }






    }

}
