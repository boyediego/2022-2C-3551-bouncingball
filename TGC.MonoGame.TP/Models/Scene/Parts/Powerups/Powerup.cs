using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Players;
using TGC.MonoGame.TP.Models.Scene.Parts.Obstacule;
using TGC.MonoGame.TP.Shared;
using TGC.MonoGame.TP.Utilities;
using TGC.MonoGame.TP.Utilities.Geometries;

namespace TGC.MonoGame.TP.Models.Scene.Parts.Powerups
{
    public abstract class Powerup : Model3D
    {
        protected Texture2D texture;
        protected Texture2D textureNormal;
        protected List<TrianglePrimitive> triangles = new List<TrianglePrimitive>();
        private float Size;

        private Boolean taked = false;
        protected Simulation simulation;
        protected BodyDescription bodyDescription;
        protected BodyHandle bodyHandle;

        private SoundEffect Sound { get; set; }
        private SoundEffectInstance Instance { get; set; }


        protected Powerup() : base(null)
        {
            base.TranslationMatrix = Matrix.Identity;
            base.RotationMatrix = Matrix.Identity;
            Sound = SoundEffectHolder<SoundEffect>.Get("Powerup-Collected");
            Instance = Sound.CreateInstance();
            Instance.IsLooped = false;
        }

        public void SetPosition(Vector3 position)
        {
            base.TranslationMatrix = Matrix.CreateTranslation(position);
            startPosition = position;
        }

        public Powerup Build(float size)
        {
            var graphicsDevice = SharedObjects.graphicsDeviceManager.GraphicsDevice;

            float h = size / 2;

            //Calculate vertex
            //Up face
            var u1 = new Vector3(-h, h, h);
            var u2 = new Vector3(h, h, h);
            var u3 = new Vector3(-h, h, -h);
            var u4 = new Vector3(h, h, -h);
            //Down face
            var d1 = new Vector3(-h, -h, h);
            var d2 = new Vector3(h, -h, h);
            var d3 = new Vector3(-h, -h, -h);
            var d4 = new Vector3(h, -h, -h);


            //Face up
            triangles.Add(new TrianglePrimitive(graphicsDevice, u1, u2, u3, Vector3.Up, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, texture, textureNormal));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u2, u4, u3, Vector3.Up, new List<Vector2>() { Vector2.UnitX, Vector2.One, Vector2.UnitY }, texture, textureNormal));

            //Face down
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, d2, d3, Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, texture, textureNormal));
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, d4, d3, Vector3.Backward, new List<Vector2>() { Vector2.UnitX, Vector2.One, Vector2.UnitY }, texture, textureNormal));

            //Front face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u1, d2, Vector3.Forward, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.UnitX }, texture, textureNormal));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u1, u2, d2, Vector3.Forward, new List<Vector2>() { Vector2.UnitY, Vector2.One, Vector2.UnitX }, texture, textureNormal));

            //Back face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d3, u3, d4, Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.UnitX }, texture, textureNormal));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u3, u4, d4, Vector3.Backward, new List<Vector2>() { Vector2.UnitY, Vector2.One, Vector2.UnitX }, texture, textureNormal));

            //Left face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u1, u3, Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.One }, texture, textureNormal));
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u3, d3, Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.One, Vector2.UnitX }, texture, textureNormal));

            // Rigth face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, u2, u4, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.One }, texture, textureNormal));
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, u4, d4, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.One, Vector2.UnitX }, texture, textureNormal));


            this.Size = size;
            return this;
        }

        public override BodyDescription GetBodyDescription(Simulation simulation)
        {
            this.simulation = simulation;
            var shape = new Box(this.Size, this.Size, this.Size);
            var collidable = new CollidableDescription(simulation.Shapes.Add(shape), 0.1f);
            this.bodyDescription = BodyDescription.CreateKinematic(new RigidPose(base.Position.ToNumericVector3()), collidable, new BodyActivityDescription(0.01f));
            this.bodyHandle = simulation.Bodies.Add(bodyDescription);
            base.SimulationHandle = bodyHandle.Value;
            return bodyDescription;
        }

        
        public abstract double DurationTime { get; }
        public override bool IsGround { get { return false; } }
        public override int PhysicsType { get { return PhysicsTypeHome.Kinematic; } }
        public override StaticDescription GetStaticDescription(Simulation simulation) { throw new NotSupportedException(); }

        public override void Collide(GameTime gameTime, Model3D sceneObject)
        {
            if (!taked)
            {
                taked = true;
                Sound.Play();
                ((Ball)sceneObject).Powerup(this, gameTime.TotalGameTime);
                simulation.Bodies.Remove(this.bodyHandle);
            }
        }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection, String techniques)
        {
            if (!taked)
                this.DrawPowerUp(gameTime, view, projection, techniques);

           
        }

        private Vector3 movementDirection = Vector3.Down;
        protected float speed=1000f;
        protected float maxMovementUnits=130f;
        protected Vector3 startPosition;
        protected Vector3 currentPosition;
        private int step = 0;

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            if (!taked)
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
                base.TranslationMatrix = Matrix.CreateTranslation(currentPosition);
            }
        }


        public abstract void DrawPowerUp(GameTime gameTime, Matrix view, Matrix projection, String techniques);
        public abstract void ApplyPowerUp(Ball ball);
        public abstract void Restore(Ball ball);

        public void Reset()
        {
            base.TranslationMatrix = Matrix.CreateTranslation(startPosition);
            GetBodyDescription(this.simulation);
            this.taked = false;
        }
    }
}
