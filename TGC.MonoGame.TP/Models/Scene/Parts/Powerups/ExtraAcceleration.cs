using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using TGC.MonoGame.TP.Models.Players;
using TGC.MonoGame.TP.Shared;
using TGC.MonoGame.TP.Utilities.Geometries;
using NumericVector3 = System.Numerics.Vector3;
using Vector3 = Microsoft.Xna.Framework.Vector3;


namespace TGC.MonoGame.TP.Models.Scene.Parts.Powerups
{
    public class ExtraAcceleration : Powerup
    {
        private const float HEIGTH = 300;
        protected Vector3 color;

        public ExtraAcceleration() : base(ModelsHolder.Get("Powerup"))
        {
         
        }

        public override void SetEffectAndTextures(Model content)
        {
            base.Effect = EffectsHolder.Get("BasicShader");
            color = new Vector3(0, 1, 0);
        }

        public override BodyDescription GetBodyDescription(Simulation simulation)
        {
            base.simulation = simulation;
            var shape = new Box(300, HEIGTH, 300);
            bodyDescription = BodyDescription.CreateConvexDynamic(new NumericVector3(base.Position.X, base.Position.Y + (HEIGTH / 2) + 50, base.Position.Z), 0.1f, simulation.Shapes, shape);
            bodyHandle = simulation.Bodies.Add(bodyDescription);
            SimulationHandle = bodyHandle.Value;
            return bodyDescription;
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {

        }

        public override void DrawPowerUp(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["DiffuseColor"].SetValue(color);
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters?["Projection"].SetValue(projection);

            var modelMeshesBaseTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            foreach (var mesh in Model.Meshes)
            {
                var meshWorld = modelMeshesBaseTransforms[mesh.ParentBone.Index];
                Effect.Parameters?["World"].SetValue(meshWorld * WorldMatrix);
                mesh.Draw();
            }
        }
    
        public override void ApplyPowerUp(Ball ball)
        {
            //TODO CALL INCREASE ACCELERATION
            ball.IncreaseJump(150);
        }

    }
}
