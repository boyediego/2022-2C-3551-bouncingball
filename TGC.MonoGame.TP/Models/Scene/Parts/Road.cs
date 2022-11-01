using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Collections;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Utilities;
using NumericVector3 = System.Numerics.Vector3;

namespace TGC.MonoGame.TP.Models.Scene.Parts
{
    public class Road : Ground
    {
        private Vector3 ObjectStartPosition;
        private float rotationAngleY=0;
        private float rotationAngleX=0;
        private static Texture2D texture;


        public Road(ContentManager content) : base(content, "scene/basics/road2")
        {
        }

        public override Boolean IsRoad { get { return true; } }

        public override void CreateModel(ContentManager content)
        {
            Effect = content.Load<Effect>(ContentFolderEffects + "TextureShader");
            var effect = Model.Meshes.FirstOrDefault().Effects.FirstOrDefault() as BasicEffect;
            if (effect != null) //TODO LOAD ALL MODELS OUTSIDE AND USE IT
            {
                texture = effect.Texture;
            }

            SetEffect(Effect);

        }

        public void SetPositionFromOrigin(Vector3 position)
        {
            this.ObjectStartPosition = position;
            TranslationMatrix = Matrix.CreateTranslation(ObjectStartPosition);
            
        }

        public void RotateY(float angle)
        {
            rotationAngleY = angle;
            RotationMatrix = Matrix.CreateRotationY(rotationAngleY)* Matrix.CreateRotationX(rotationAngleX);
        }

        public void RotateX(float angle)
        {
            rotationAngleX = angle;
            RotationMatrix = Matrix.CreateRotationY(rotationAngleY) * Matrix.CreateRotationX(rotationAngleX);
        }

        public override void SetCustomEffectParameters(Effect effect)
        {
            Effect.Parameters["ModelTexture"].SetValue(texture);
        }


        public override StaticDescription GetStaticDescription(Simulation simulation)
        {
            Vector3 size = GetModelSize();

            var halfWidth = size.X / 2;
            var halfHeight = size.Y / 2;
            var HalfLength = size.Z / 2;


            List<Vector3> puntos = new List<Vector3>();

            //UP FACE
            puntos.Add( new Vector3(Position.X - halfWidth, Position.Y + halfHeight, Position.Z - HalfLength));
            puntos.Add( new Vector3(Position.X - halfWidth, Position.Y + halfHeight, Position.Z + HalfLength));
            puntos.Add( new Vector3(Position.X + halfWidth, Position.Y + halfHeight, Position.Z - HalfLength));
            puntos.Add( new Vector3(Position.X + halfWidth, Position.Y + halfHeight, Position.Z + HalfLength));

            //DOWN FACE
            puntos.Add( new Vector3(Position.X - halfWidth, Position.Y - halfHeight, Position.Z - HalfLength));
            puntos.Add( new Vector3(Position.X - halfWidth, Position.Y - halfHeight, Position.Z + HalfLength));
            puntos.Add( new Vector3(Position.X + halfWidth, Position.Y - halfHeight, Position.Z - HalfLength));
            puntos.Add( new Vector3(Position.X + halfWidth, Position.Y - halfHeight, Position.Z + HalfLength));


            var points = new QuickList<System.Numerics.Vector3>(8, simulation.BufferPool);
            foreach (Vector3 p in puntos)
            {
                Vector3 v = Vector3.Transform(p, Quaternion.CreateFromRotationMatrix(base.RotationMatrix));
                points.AllocateUnsafely() = new System.Numerics.Vector3(v.X,v.Y,v.Z);
            }
            
            ConvexHull r =new ConvexHull(points.Span.Slice(points.Count), simulation.BufferPool, out _);

            StaticDescription sta = new StaticDescription(new NumericVector3(Position.X, Position.Y, Position.Z),
                 new CollidableDescription(simulation.Shapes.Add(r), 0.1f));

            StaticHandle handle = simulation.Statics.Add(sta);
            SimulationHandle = handle.Value;

            return sta;
        }

    }

}
