using BepuPhysics;
using BepuPhysics.CollisionDetection;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.TP.Models.Players;
using TGC.MonoGame.TP.Physics;
using static TGC.MonoGame.TP.Physics.Collider;

namespace TGC.MonoGame.TP.UI
{
    public interface IRenderUI
    {
        void Initialize(TGCGame game);
        void CollisionDetected(CollidablePair pair, CollisionInformation info);
        void Upate(GameTime gameTime);
        void Draw(GameTime gameTime);
        void Dispose();
    }
}
