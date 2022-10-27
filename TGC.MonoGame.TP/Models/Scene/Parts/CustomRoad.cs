using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Utilities.Geometries;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework.Content;
using BepuPhysics;
using TGC.MonoGame.TP.Cameras;
using System.Linq;
using System.Data;
using System;
using NumericVector3 = System.Numerics.Vector3;
using System.Runtime.Serialization.Formatters;
using TGC.MonoGame.TP.Utilities;
using BepuUtilities.Collections;


namespace TGC.MonoGame.TP.Models.Scene.Parts
{
    internal class CustomRoad : Ground
    {
        private GraphicsDevice graphicsDevice;

        Vector3 u1;
        Vector3 u2;
        Vector3 u3;
        Vector3 u4;

        Vector3 d1;
        Vector3 d2;
        Vector3 d3;
        Vector3 d4;

        private List<TrianglePrimitive> triangles = new List<TrianglePrimitive>();
        public CustomRoad(ContentManager content, GraphicsDevice graphicsDevice, float w, float l, float h, float elevation, float baseElevationOffset) : base(content, null)
        {
            this.graphicsDevice = graphicsDevice;
            float x = 0;
            float y = h;
            float z = 0;


            //Calculate vertex
            //Up face
            u1 = new Vector3(x, y, z);
            u2 = new Vector3(x + w, y, z);
            u3 = new Vector3(x, y + elevation, z + l);
            u4 = new Vector3(x + w, y + elevation, z + l);
            //Down face
            d1 = new Vector3(x, 0, z);
            d2 = new Vector3(x + w, 0, z);
            d3 = new Vector3(x, 0 + baseElevationOffset, z + l);
            d4 = new Vector3(x + w, 0 + baseElevationOffset, z + l);


            //2 Triangles per face
            //Face up
            triangles.Add(new TrianglePrimitive(graphicsDevice, u1, u2, u3, Color.White, Color.White, Color.White, Vector3.Up));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u2, u4, u3, Color.White, Color.White, Color.White, Vector3.Up));

            //Face down
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, d2, d3, Color.Fuchsia, Color.Fuchsia, Color.Fuchsia, Vector3.Backward));
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, d4, d3, Color.Fuchsia, Color.Fuchsia, Color.Fuchsia, Vector3.Backward));

            //Front face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u1, d2, Color.Blue, Color.Blue, Color.Blue, Vector3.Forward));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u1, u2, d2, Color.Blue, Color.Blue, Color.Blue, Vector3.Forward));

            //Back face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d3, u3, d4, Color.Green, Color.Green, Color.Green, Vector3.Backward));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u3, u4, d4, Color.Green, Color.Green, Color.Green, Vector3.Backward));

            //Left face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u1, u3, Color.Red, Color.Red, Color.Red, Vector3.Left));
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u3, d3, Color.Red, Color.Red, Color.Red, Vector3.Left));

            // Rigth face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, u2, u4, Color.Yellow, Color.Yellow, Color.Yellow, Vector3.Right));
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, u4, d4, Color.Yellow, Color.Yellow, Color.Yellow, Vector3.Right));

        }

        //No cargamos ningun modelo
        public override void CreateModel(ContentManager content)
        {
            throw new System.NotSupportedException();
        }

        public override StaticDescription GetStaticDescription(Simulation simulation)
        {

            var points = new QuickList<System.Numerics.Vector3>(8, simulation.BufferPool);

            Vector3 vertex1 = Vector3.Transform(u1, base.WorldMatrix);
            Vector3 vertex2 = Vector3.Transform(u2, base.WorldMatrix);
            Vector3 vertex3 = Vector3.Transform(u3, base.WorldMatrix);
            Vector3 vertex4 = Vector3.Transform(u4, base.WorldMatrix);

            Vector3 vertex5 = Vector3.Transform(d1, base.WorldMatrix);
            Vector3 vertex6 = Vector3.Transform(d2, base.WorldMatrix);
            Vector3 vertex7 = Vector3.Transform(d3, base.WorldMatrix);
            Vector3 vertex8 = Vector3.Transform(d4, base.WorldMatrix);


            points.AllocateUnsafely() = new System.Numerics.Vector3(vertex1.X, vertex1.Y, vertex1.Z);
            points.AllocateUnsafely() = new System.Numerics.Vector3(vertex2.X, vertex2.Y, vertex2.Z);
            points.AllocateUnsafely() = new System.Numerics.Vector3(vertex3.X, vertex3.Y, vertex3.Z);
            points.AllocateUnsafely() = new System.Numerics.Vector3(vertex4.X, vertex4.Y, vertex4.Z);
            points.AllocateUnsafely() = new System.Numerics.Vector3(vertex5.X, vertex5.Y, vertex5.Z);
            points.AllocateUnsafely() = new System.Numerics.Vector3(vertex6.X, vertex6.Y, vertex6.Z);
            points.AllocateUnsafely() = new System.Numerics.Vector3(vertex7.X, vertex7.Y, vertex7.Z);
            points.AllocateUnsafely() = new System.Numerics.Vector3(vertex8.X, vertex8.Y, vertex8.Z);

            NumericVector3 center=new NumericVector3();
            ConvexHull r = new ConvexHull(points.Span.Slice(points.Count), simulation.BufferPool, out center);


            StaticDescription sta = new StaticDescription(new NumericVector3(center.X, center.Y, center.Z),
               new CollidableDescription(simulation.Shapes.Add(r), 0.0001f));

            StaticHandle handle = simulation.Statics.Add(sta);
            SimulationHandle = handle.Value;

            return new StaticDescription();
        }

        public override void SetCustomEffectParameters(Effect effect)
        {

        }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            var oldRasterizerState = graphicsDevice.RasterizerState;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            foreach (TrianglePrimitive triangle in triangles)
            {
                var triangleEffect = triangle.Effect;
                triangleEffect.View = view;
                triangleEffect.Projection = projection;
                triangleEffect.World = this.WorldMatrix;
                triangleEffect.LightingEnabled = false;
                triangle.Draw(triangleEffect);
            }
            graphicsDevice.RasterizerState = oldRasterizerState;
        }

        internal void SetTranslation(float v1, float v2, float v3)
        {
            base.TranslationMatrix = Matrix.CreateTranslation(v1, v2, v3);
        }
    }
}
