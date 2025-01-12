﻿using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Players;
using TGC.MonoGame.TP.Shared;
using TGC.MonoGame.TP.Utilities;
using TGC.MonoGame.TP.Utilities.Geometries;
using NumericVector3 = System.Numerics.Vector3;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace TGC.MonoGame.TP.Models.Scene.Parts.Checkpoints
{
    public class Checkpoint : Model3D
    {
        private Simulation simulation;
        protected bool playerPassCheckPoint = false;
        private Vector3 checkpointPosition;
        private float checkpointWidth;
        private BodyDescription bodyDescription;
        private BodyHandle bodyHandle;
        private CubePrimitive cubePrimitive;
        public bool CheckpointPassed { get { return playerPassCheckPoint; } }

        private SoundEffect Sound { get; set; }

        public Checkpoint(Vector3 checkpointPosition, float checkpointWidth) : base(null)
        {
            TranslationMatrix = Matrix.CreateTranslation(checkpointPosition);
            this.checkpointPosition = checkpointPosition;
            this.checkpointWidth = checkpointWidth;
            cubePrimitive = new CubePrimitive(SharedObjects.graphicsDeviceManager.GraphicsDevice, checkpointWidth, Color.Fuchsia);
            Sound = SoundEffectHolder<SoundEffect>.Get("Checkpoint");
        }

        public override bool IsGround { get { return false; } }
        public override int PhysicsType { get { return PhysicsTypeHome.Kinematic; } }
        public override StaticDescription GetStaticDescription(Simulation simulation) { throw new NotSupportedException(); }
        public override void SetEffectAndTextures(Model model) { }

        public override BodyDescription GetBodyDescription(Simulation simulation)
        {
            this.simulation = simulation;
            var shape = new Box(checkpointWidth, checkpointWidth, checkpointWidth);
            bodyDescription = BodyDescription.CreateConvexDynamic(new NumericVector3(checkpointPosition.X, checkpointPosition.Y + checkpointWidth / 2, checkpointPosition.Z), 0.1f, simulation.Shapes, shape);
            bodyHandle = simulation.Bodies.Add(bodyDescription);
            SimulationHandle = bodyHandle.Value;
            return bodyDescription;
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            //Nothing
        }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection, String techniques)
        {
            //Only for debug 
            //var world = Matrix.CreateTranslation(new Vector3(checkpointPosition.X, checkpointPosition.Y + (checkpointWidth / 2), checkpointPosition.Z));
            //cubePrimitive.Draw(world, view, projection);
        }

        public override void Collide(GameTime gameTime, Model3D sceneObject)
        {
            //Only when player collide with the objetct
            if (!playerPassCheckPoint)
            {
                Sound.Play();
                CheckpointReached((Ball)sceneObject);
            }
        }


        protected void CheckpointReached(Ball byPlayer)
        {
            playerPassCheckPoint = true;
            byPlayer.CheckpointReached(this);
            simulation.Bodies.Remove(bodyHandle);
        }
     
    }
}
