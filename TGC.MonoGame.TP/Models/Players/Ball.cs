﻿using BepuPhysics;
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
using TGC.MonoGame.TP.Models.Scene.Parts;
using TGC.MonoGame.TP.Models.Scene.Parts.Powerups;

namespace TGC.MonoGame.TP.Models.Players
{
    public class Ball : Model3D
    {
        //private static Texture2D texture;//FIXME
        public GraphicsDeviceManager Graphics;
        private Simulation simulation;
        public BodyDescription BodyDescription { get; set; }
        public BodyHandle playerHanle { get; set; }
        private Vector3 PreviousVelocityDirection;
        protected Boolean OnGround = false;
        protected Vector3 ReSpawnPosition;

        protected float ForwardImpulse
        {
            get { return 30f; }
        }

        protected float BrakeForce
        {
            get { return 30f; }
        }

        protected float RotateForce
        {
            get { return 25f; }
        }

        public override int PhysicsType
        {
            get { return PhysicsTypeHome.Dynamic; }
        }

        protected virtual float JumpImpulse { get { return 1000f; } }
        private float IncreaseJumpValue=0;
        private float ExtraImpulse { get { return JumpImpulse*(IncreaseJumpValue/100f); } }

        private Texture2D boxTexture;
        private Texture2D normalTexture;


        public Ball(ContentManager content, Vector3 startPosition, Simulation Simulation) : base(content, "balls/sphere1")
        {
            this.simulation = Simulation;
            var position = new NumericVector3(startPosition.X, startPosition.Y, startPosition.Z);
            CreatePhysics(position);
            this.ReSpawnPosition = startPosition;
            base.ScaleMatrix = Matrix.CreateScale(0.5f);
        }

        private void CreatePhysics(NumericVector3 position)
        {
            var boundingPlayer = this.GetBoundingSphere();
            var simulationPlayer = new Sphere(boundingPlayer.Radius - 95);
            this.BodyDescription = BodyDescription.CreateConvexDynamic(position, 1f, simulation.Shapes, simulationPlayer);
            this.playerHanle = simulation.Bodies.Add(this.BodyDescription);
            base.CurrentMovementDirection = new Vector3(0, 0, 0);
        }

        public override StaticDescription GetStaticDescription(Simulation simulation) { throw new NotSupportedException(); }
        public override BodyDescription GetBodyDescription(Simulation simulation) { throw new NotSupportedException(); }
        public override bool IsGround => throw new NotSupportedException();

        public BoundingSphere GetBoundingSphere()
        {
            return this.Model.GetSphereFrom();
        }

        public override void CreateModel(ContentManager content)
        {
            /*   Effect = content.Load<Effect>(ContentFolderEffects + "TextureShader");
               var effect = Model.Meshes.FirstOrDefault().Effects.FirstOrDefault() as BasicEffect;
               if (effect != null)
               {
                   texture = effect.Texture;
               }*/

            boxTexture = content.Load<Texture2D>(TGCGame.ContentFolder3D + "balls/metal");
            normalTexture = content.Load<Texture2D>(TGCGame.ContentFolder3D + "balls/metal-normal");

            Effect = TGCGame.LightEffects;
            SetEffect(Effect);
        }

        private Boolean init = false;
        private TimeSpan lastJump=TimeSpan.Zero;
        private Vector3 applyImpulse;
        public override void Update(GameTime gameTime, KeyboardState keyboardState, List<IGameModel> otherInteractiveObjects)
        {
            
            var bodyReference = simulation.Bodies.GetBodyReference(playerHanle);
            var position = bodyReference.Pose.Position;
            var quaternion = bodyReference.Pose.Orientation;

            var velocityVector = new Vector3(bodyReference.Velocity.Linear.X, 0, bodyReference.Velocity.Linear.Z);

            base.RotationMatrix = Matrix.CreateFromQuaternion(new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W));
            base.TranslationMatrix = Matrix.CreateTranslation(new Vector3(position.X, position.Y, position.Z));


            if (position.Y < -400)
            {
                Respawn();
                return;
            }

            if (!init)
            {
                bodyReference.ApplyLinearImpulse(Vector3.Backward.ToNumericVector3() * ForwardImpulse);
                init = true;
            }


            if (velocityVector.LengthSquared() > 0)
            {
                PreviousVelocityDirection = velocityVector;
                PreviousVelocityDirection.Normalize();
            }


            base.CurrentMovementDirection = Vector3.Lerp(base.CurrentMovementDirection, -PreviousVelocityDirection, 0.05f);

            var velocityDirection = base.CurrentMovementDirection;
            velocityDirection.Normalize();

            var nVelocityVector = PreviousVelocityDirection;

            nVelocityVector.Normalize();


            var dv = velocityVector.Length() / 500;
            if (dv <= 1)
            {
                dv = 1;
            }


            if (keyboardState.IsKeyDown(Keys.W))
            {
                bodyReference.Awake = true;
                applyImpulse = nVelocityVector;
                bodyReference.ApplyLinearImpulse(applyImpulse.ToNumericVector3() * ForwardImpulse / dv);
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                bodyReference.Awake = true;
                applyImpulse = velocityDirection;
                bodyReference.ApplyLinearImpulse(applyImpulse.ToNumericVector3() * BrakeForce);
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                bodyReference.Awake = true;
                applyImpulse = (velocityDirection.PerpendicularCounterClockwiseIn2D());
                bodyReference.ApplyLinearImpulse(applyImpulse.ToNumericVector3() * (RotateForce / velocityDirection.Length()));

            }

            if (keyboardState.IsKeyDown(Keys.X))
            {
                bodyReference.Awake = true;
                bodyReference.ApplyLinearImpulse(velocityDirection.PerpendicularClockwiseIn2D().ToNumericVector3() * RotateForce / velocityDirection.Length());
            }

           

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                TryJump(bodyReference, gameTime.TotalGameTime);
            }

        }

        private void Respawn()
        {
            simulation.Bodies.Remove(this.playerHanle);
            base.TranslationMatrix = Matrix.CreateTranslation(ReSpawnPosition);
            base.TranslationMatrix = Matrix.Identity;
            CreatePhysics(ReSpawnPosition.ToNumericVector3());
        }

        
        private void TryJump(BodyReference bodyReference, TimeSpan current)
        {
            if (lastJump == TimeSpan.Zero || current.Subtract(lastJump).Milliseconds > 300)
            {
                if (OnGround)
                {
                    lastJump = current;
                    bodyReference.Awake = true;
                    bodyReference.ApplyLinearImpulse(Vector3.Up.ToNumericVector3() * (JumpImpulse + ExtraImpulse));
                    OnGround = false;
                }
            }
        }


        public override void Collide(Model3D sceneObject)
        {
            if (sceneObject.PhysicsType == PhysicsTypeHome.Static)
            {
                if(!OnGround)
                    Debug.WriteLine("OnGround detected with " + sceneObject);
                OnGround = sceneObject.IsGround;
                
            }
            else
            {
                sceneObject.Collide(this);
            }

        }



        public override void SetCustomEffectParameters(Effect effect)
        {
            //Effect.Parameters["ModelTexture"].SetValue(texture);
            effect.Parameters["ModelTexture"].SetValue(boxTexture);
            effect.Parameters["NormalTexture"].SetValue(normalTexture);
            effect.Parameters["Tiling"].SetValue(Microsoft.Xna.Framework.Vector2.One);
            effect.Parameters["eyePosition"].SetValue(TGCGame.Camera.Position);
            effect.Parameters["World"].SetValue(this.WorldMatrix);
            effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Invert(Matrix.Transpose(this.WorldMatrix)));
            effect.Parameters["lightPosition"].SetValue(new Vector3(this.Position.X + 16000, this.Position.Y + 14000f, this.Position.Z));
            effect.Parameters["ambientColor"].SetValue(new Vector3(1f, 1f, 1f));
            effect.Parameters["diffuseColor"].SetValue(new Vector3(0.5f, 0.1f, 0f));
            effect.Parameters["specularColor"].SetValue(new Vector3(0.5f, 0.1f, 0f));
            effect.Parameters["KAmbient"].SetValue(0.4f);
            effect.Parameters["KDiffuse"].SetValue(0.7f);
            effect.Parameters["KSpecular"].SetValue(0.4f);
            effect.Parameters["shininess"].SetValue(4.0f);
            effect.CurrentTechnique = effect.Techniques["NormalMapping"];
        }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["WorldViewProjection"].SetValue(this.WorldMatrix * view * projection);
            base.Draw(gameTime, view, projection);    
        }


        public void CheckpointReached(Checkpoint checkpoint)
        {
            Debug.WriteLine("Checkpoint");
            this.ReSpawnPosition = checkpoint.Position + Vector3.Up * 550;
        }

        public void Powerup(Powerup powerup)
        {
            powerup.ApplyPowerUp(this);
        }

        internal void IncreaseJump(float percent)
        {
            IncreaseJumpValue = percent;
        }
    }

}
