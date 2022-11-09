using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Collections;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene.Parts.Roads;
using TGC.MonoGame.TP.Shared;
using TGC.MonoGame.TP.Utilities;
using NumericVector3 = System.Numerics.Vector3;

namespace TGC.MonoGame.TP.Models.Scene.Parts.Deprecated
{
    [Obsolete("Use CustomRoad")]
    public class Road : Ground
    {
        private Vector3 ObjectStartPosition;
        private float rotationAngleY = 0;
        private float rotationAngleX = 0;
        private Texture2D texture;


        public Road() : base(ModelsHolder.Get("Platform"))
        {
        }

        public override bool IsRoad { get { return true; } }

        public override void SetEffectAndTextures(Model model)
        {
            base.Effect = EffectsHolder.Get("TextureShader");
            this.texture = TexturesHolder<Texture2D>.Get("Road-type-1");
        }

        public void SetPositionFromOrigin(Vector3 position)
        {
            ObjectStartPosition = position;
            TranslationMatrix = Matrix.CreateTranslation(ObjectStartPosition);
        }

        public void RotateY(float angle)
        {
            rotationAngleY = angle;
            RotationMatrix = Matrix.CreateRotationY(rotationAngleY) * Matrix.CreateRotationX(rotationAngleX);
        }

        public void RotateX(float angle)
        {
            rotationAngleX = angle;
            RotationMatrix = Matrix.CreateRotationY(rotationAngleY) * Matrix.CreateRotationX(rotationAngleX);
        }

        public override StaticDescription GetStaticDescription(Simulation simulation)
        {
            Vector3 size = GetModelSize();

            var halfWidth = size.X / 2;
            var halfHeight = size.Y / 2;
            var HalfLength = size.Z / 2;


            List<Vector3> puntos = new List<Vector3>();

            //UP FACE
            puntos.Add(new Vector3(Position.X - halfWidth, Position.Y + halfHeight, Position.Z - HalfLength));
            puntos.Add(new Vector3(Position.X - halfWidth, Position.Y + halfHeight, Position.Z + HalfLength));
            puntos.Add(new Vector3(Position.X + halfWidth, Position.Y + halfHeight, Position.Z - HalfLength));
            puntos.Add(new Vector3(Position.X + halfWidth, Position.Y + halfHeight, Position.Z + HalfLength));

            //DOWN FACE
            puntos.Add(new Vector3(Position.X - halfWidth, Position.Y - halfHeight, Position.Z - HalfLength));
            puntos.Add(new Vector3(Position.X - halfWidth, Position.Y - halfHeight, Position.Z + HalfLength));
            puntos.Add(new Vector3(Position.X + halfWidth, Position.Y - halfHeight, Position.Z - HalfLength));
            puntos.Add(new Vector3(Position.X + halfWidth, Position.Y - halfHeight, Position.Z + HalfLength));


            var points = new QuickList<NumericVector3>(8, simulation.BufferPool);
            foreach (Vector3 p in puntos)
            {
                Vector3 v = Vector3.Transform(p, Quaternion.CreateFromRotationMatrix(RotationMatrix));
                points.AllocateUnsafely() = new NumericVector3(v.X, v.Y, v.Z);
            }

            ConvexHull r = new ConvexHull(points.Span.Slice(points.Count), simulation.BufferPool, out _);

            StaticDescription sta = new StaticDescription(new NumericVector3(Position.X, Position.Y, Position.Z),
                 new CollidableDescription(simulation.Shapes.Add(r), 0.1f));

            StaticHandle handle = simulation.Statics.Add(sta);
            SimulationHandle = handle.Value;

            return sta;
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            //Nothing
        }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["ModelTexture"].SetValue(texture);
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

        public override void Collide(Model3D sceneObject)
        {
            //Nothing
        }
    }

}
