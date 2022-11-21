using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
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
    public class EndRoad : Checkpoint
    {
        public EndRoad(Vector3 checkpointPosition, float checkpointWidth) : base(checkpointPosition, checkpointWidth)
        {
        }

        public override void Collide(GameTime gameTime, Model3D sceneObject)
        {
            if (!playerPassCheckPoint)
            {
                CheckpointReached((Ball)sceneObject);
                SharedObjects.CurrentScene.End();
            }
        }

     
    }
}
