using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Players;
using TGC.MonoGame.TP.Utilities;
using TGC.MonoGame.TP.Utilities.Geometries;
using NumericVector3 = System.Numerics.Vector3;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace TGC.MonoGame.TP.Models.Scene.Parts
{
    public class Checkpoint : Model3D
    {

        private Simulation simulation;
        private Boolean playerPassCheckPoint = false;
        private Vector3 checkpointPosition;
        private float checkpointWidth;
        private BodyDescription bodyDescription;
        private BodyHandle bodyHandle;
        
        
        private CubePrimitive cubePrimitive;
        


        public Boolean CheckpointPassed { get { return playerPassCheckPoint; } }


        public Checkpoint(ContentManager content, string pathModel) : base(content, pathModel)
        {
            throw new NotSupportedException();
        }

        public Checkpoint(ContentManager content, Vector3 checkpointPosition, float checkpointWidth) : base(content, null)
        {
            base.TranslationMatrix = Matrix.CreateTranslation(checkpointPosition);
            this.checkpointPosition = checkpointPosition;
            this.checkpointWidth = checkpointWidth;
            cubePrimitive = new CubePrimitive(TGCGame.Graphics.GraphicsDevice, checkpointWidth, Color.Fuchsia);
        }

        public override bool IsGround { get { return false; } }
        public override int PhysicsType { get { return PhysicsTypeHome.Kinematic; } }
        public override StaticDescription GetStaticDescription(Simulation simulation) { throw new NotSupportedException(); }

        public override void CreateModel(ContentManager content)
        {
            throw new NotSupportedException();
        }

        public override BodyDescription GetBodyDescription(Simulation simulation)
        {
            this.simulation = simulation;
            var shape = new Box(checkpointWidth, checkpointWidth, checkpointWidth);
            bodyDescription = BodyDescription.CreateConvexDynamic(new NumericVector3(checkpointPosition.X, checkpointPosition.Y + (checkpointWidth/2), checkpointPosition.Z), 0.1f, simulation.Shapes, shape);
            bodyHandle = simulation.Bodies.Add(bodyDescription);
            SimulationHandle = bodyHandle.Value;
            return bodyDescription;
        }

        public override void SetCustomEffectParameters(Effect effect)
        {
            throw new NotSupportedException();
        }
        
        public override void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            //Only for debug 
          //  var world = Matrix.CreateTranslation(new Vector3(checkpointPosition.X, checkpointPosition.Y + (checkpointWidth / 2), checkpointPosition.Z));
          //  cubePrimitive.Draw(world, view, projection);
        }

        public override void Collide(Model3D sceneObject)
        {
            //Only when player collide with the objetct
            if (!playerPassCheckPoint)
            {
                playerPassCheckPoint = true;
                ((Ball)sceneObject).CheckpointReached(this);
                simulation.Bodies.Remove(this.bodyHandle);
            }
            

        }
    }
}
