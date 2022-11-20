using BepuPhysics;
using BepuPhysics.Collidables;
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
using TGC.MonoGame.TP.Models.Scene.Parts.Obstacule.Base;
using TGC.MonoGame.TP.Shared;
using TGC.MonoGame.TP.Utilities;

namespace TGC.MonoGame.TP.Models.Scene.Parts.Obstacule
{
    public class SphereObstacule : BouncingObstacule
    {
        protected Vector3 color;
        public SphereObstacule() : base(ModelsHolder.Get("SphereObstacule"))
        {
            base.ScaleMatrix = Matrix.CreateScale(1.5f);
            base.Effect = EffectsHolder.Get("BasicShader");
            color = new Vector3(0, 1, 0);
        }

        public override void SetEffectAndTextures(Model model)
        {
            base.Effect = EffectsHolder.Get("BasicShader");
            color = new Vector3(0, 1, 0);
        }

        public override BodyDescription GetBodyDescription(Simulation simulation)
        {
            base.simulation = simulation;
            var size = base.GetModelSize();
            var shape = new Box(size.X, size.Y, size.Z);
            var collidable = new CollidableDescription(simulation.Shapes.Add(shape), 0.1f);

            base.bodyDescription = BodyDescription.CreateKinematic(new RigidPose(startPosition.ToNumericVector3()), collidable, new BodyActivityDescription(0.01f));
            base.bodyHandle = simulation.Bodies.Add(bodyDescription);
            base.SimulationHandle = bodyHandle.Value;
            return bodyDescription;
        }


        public override void Draw(GameTime gameTime, Matrix view, Matrix projection, String t)
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

    }

}
