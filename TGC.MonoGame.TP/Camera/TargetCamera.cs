﻿using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Models.Commons;

namespace TGC.MonoGame.TP.Cameras
{
    /// <summary>
    ///     Camera looking at a particular point, assumes the up vector is in y.
    /// </summary>
    public class TargetCamera : Camera
    {
        /// <summary>
        ///     The direction that is "up" from the camera's point of view.
        /// </summary>
        public readonly Vector3 DefaultWorldUpVector = Vector3.Up;
        public Model3D Target { get; set; }

        /// <summary>
        ///     Camera looking at a particular direction, which has the up vector (0,1,0).
        /// </summary>
        /// <param name="aspectRatio">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="position">The position of the camera.</param>
        /// <param name="targetPosition">The target towards which the camera is pointing.</param>
        public TargetCamera(float aspectRatio, Vector3 position, Vector3 targetPosition) : base(aspectRatio)
        {
            BuildView(position, targetPosition);
        }

        /// <summary>
        ///     Camera looking at a particular direction, which has the up vector (0,1,0).
        /// </summary>
        /// <param name="aspectRatio">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="position">The position of the camera.</param>
        /// <param name="targetPosition">The target towards which the camera is pointing.</param>
        /// <param name="nearPlaneDistance">Distance to the near view plane.</param>
        /// <param name="farPlaneDistance">Distance to the far view plane.</param>
        public TargetCamera(float aspectRatio, Vector3 position, Vector3 targetPosition, float nearPlaneDistance,
            float farPlaneDistance) : base(aspectRatio, nearPlaneDistance, farPlaneDistance)
        {
            BuildView(position, targetPosition);
        }

        /// <summary>
        ///     The target towards which the camera is pointing.
        /// </summary>
        public Vector3 TargetPosition { get; set; }

        /// <summary>
        ///     Build view matrix and update the internal directions.
        /// </summary>
        /// <param name="position">The position of the camera.</param>
        /// <param name="targetPosition">The target towards which the camera is pointing.</param>
        private void BuildView(Vector3 position, Vector3 targetPosition)
        {
            Position = position;
            TargetPosition = targetPosition;
            BuildView();
        }

        private const float CameraFollowRadius = 1200f;
        private const float CameraUpDistance = 350f;

        public void UpdateCamera(Vector3 position, Matrix rotation)
        {
            // Create a position that orbits the Robot by its direction (Rotation)

            // Create a normalized vector that points to the back of the Robot
            var robotBackDirection = Vector3.Transform(Vector3.Backward, rotation);
            // Then scale the vector by a radius, to set an horizontal distance between the Camera and the Robot
            var orbitalPosition = robotBackDirection * CameraFollowRadius;


            // We will move the Camera in the Y axis by a given distance, relative to the Robot
            var upDistance = Vector3.Up * CameraUpDistance;

            // Calculate the new Camera Position by using the Robot Position, then adding the vector orbitalPosition that sends 
            // the camera further in the back of the Robot, and then we move it up by a given distance
            var newCameraPosition = position + orbitalPosition + upDistance;

            // Check if the Camera collided with the scene
          /*  var collisionDistance = CameraCollided(newCameraPosition);

            // If the Camera collided with the scene
            if (collisionDistance.HasValue)
            {
                // Limit our Horizontal Radius by subtracting the distance from the collision
                // Then clamp that value to be in between the near plane and the original Horizontal Radius
                var clampedDistance = MathHelper.Clamp(CameraFollowRadius - collisionDistance.Value, 0.1f, CameraFollowRadius);

                // Calculate our new Horizontal Position by using the Robot Back Direction multiplied by our new range
                // (a range we know doesn't collide with the scene)
                var recalculatedPosition = robotBackDirection * clampedDistance;

                // Set our new position. Up is unaffected
                Camera.Position = RobotPosition + recalculatedPosition + upDistance;
            }
            // If the Camera didn't collide with the scene
            else*/
                this.Position = newCameraPosition;

            // Set our Target as the Robot, the Camera needs to be always pointing to it
            this.TargetPosition = position;

            // Build our View matrix from the Position and TargetPosition
            this.BuildView();
        }

        /// <summary>
        ///     Build view matrix and update the internal directions.
        /// </summary>
        public void BuildView()
        {
            FrontDirection = Vector3.Normalize(TargetPosition - Position);
            RightDirection = Vector3.Normalize(Vector3.Cross(DefaultWorldUpVector, FrontDirection));
            UpDirection = Vector3.Cross(FrontDirection, RightDirection);
            View = Matrix.CreateLookAt(Position, Position + FrontDirection, UpDirection);
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            UpdateCamera(Target.Position, Target.RotationWithDirection);
            // This camera has no movement, once initialized with position and lookAt it is no longer updated automatically.
        }
    }
}