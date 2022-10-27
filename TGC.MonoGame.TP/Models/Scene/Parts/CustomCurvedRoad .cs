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
using System.Threading;
using System.Security;
using static System.Net.Mime.MediaTypeNames;

namespace TGC.MonoGame.TP.Models.Scene.Parts
{
    internal class CustomCurvedRoad : Ground
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
        public CustomCurvedRoad(ContentManager content, GraphicsDevice graphicsDevice, Vector3 startPosition, Vector3 endPosition, float w, float h) : base(content, null)
        {
            BezierPath path = new BezierPath();
            Vector3 controlPoint1 = new Vector3(startPosition.X + (startPosition.Length() * 1f / 4f) + 1000, startPosition.Y, startPosition.Z + (startPosition.Length() * 1f / 4f) + 1000);
            Vector3 controlPoint3 = new Vector3(startPosition.X + (startPosition.Length() * 3f / 4f) + 1000, startPosition.Y, startPosition.Z + (startPosition.Length() * 3f / 4f) + 1000);
            path.SetControlPoints(new List<Vector3>() { startPosition, controlPoint1, controlPoint3, endPosition });

            List<Vector3> points = path.GetDrawingPoints1();

            var boxTexture = content.Load<Texture2D>(TGCGame.ContentFolderTextures + "extras/basev2");


            this.graphicsDevice = graphicsDevice;
            for (int index = 0; index < points.Count - 1; index++)
            {



                var current = points[index];
                var next = points[index + 1];



                Vector2 texturaP1 = Vector2.Zero;
                Vector2 texturaP2P5 = Vector2.UnitX;
                Vector2 texturaP3P4 = Vector2.UnitY;
                Vector2 texturaP6 = Vector2.One;

                Vector3 vectorW = new Vector3(w, 0, 0);
                Vector3 p1 = current;
                Vector3 p2 = next;
                Vector3 p3 = current + vectorW;
                //Face up
                triangles.Add(new TrianglePrimitive(graphicsDevice, p1, p2, p3, Vector3.Up, new List<Vector2>() { texturaP1, texturaP2P5, texturaP3P4 }, boxTexture));
                //Face down
                triangles.Add(new TrianglePrimitive(graphicsDevice, p1 - new Vector3(0, h, 0), p2 - new Vector3(0, h, 0), p3 - new Vector3(0, h, 0), Vector3.Down, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));

                Vector3 p4 = current + vectorW;
                Vector3 p5 = next;
                Vector3 p6 = next + new Vector3(w, 0, 0);
                //Face up
                triangles.Add(new TrianglePrimitive(graphicsDevice, p4, p5, p6, Vector3.Up, new List<Vector2>() { texturaP3P4, texturaP2P5, texturaP6 }, boxTexture));
                //Face down
                triangles.Add(new TrianglePrimitive(graphicsDevice, p4 - new Vector3(0, h, 0), p5 - new Vector3(0, h, 0), p6 - new Vector3(0, h, 0), Vector3.Down, new List<Vector2>() { Vector2.UnitY, Vector2.UnitX, Vector2.One }, boxTexture));

                //Face Left
                triangles.Add(new TrianglePrimitive(graphicsDevice, p3, p3 - new Vector3(0, h, 0), p6 - new Vector3(0, h, 0), Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
                triangles.Add(new TrianglePrimitive(graphicsDevice, p6 - new Vector3(0, h, 0), p3, p6, Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));

                //Face Right
                triangles.Add(new TrianglePrimitive(graphicsDevice, p1, p1 - new Vector3(0, h, 0), p2 - new Vector3(0, h, 0), Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
                triangles.Add(new TrianglePrimitive(graphicsDevice, p2- new Vector3(0, h, 0), p1, p2, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));


            }

            //Front face
            triangles.Add(new TrianglePrimitive(graphicsDevice, startPosition, startPosition - new Vector3(0, h, 0), startPosition + new Vector3(w, 0, 0), Vector3.Forward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, startPosition - new Vector3(0, h, 0), startPosition + new Vector3(w, 0, 0), startPosition + new Vector3(w, -h, 0), Vector3.Forward, new List<Vector2>() { Vector2.One, Vector2.UnitX, Vector2.UnitY }, boxTexture));

            //Back face
            triangles.Add(new TrianglePrimitive(graphicsDevice, endPosition, endPosition - new Vector3(0, h, 0), endPosition + new Vector3(w, 0, 0), Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, endPosition - new Vector3(0, h, 0), endPosition + new Vector3(w, 0, 0), endPosition + new Vector3(w, -h, 0), Vector3.Backward, new List<Vector2>() { Vector2.One, Vector2.UnitX, Vector2.UnitY }, boxTexture));

            /* float x = 0;
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

             var boxTexture = content.Load<Texture2D>(TGCGame.ContentFolderTextures + "extras/basev2");

             //Face up
             triangles.Add(new TrianglePrimitive(graphicsDevice, u1, u2, u3, Vector3.Up, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
             triangles.Add(new TrianglePrimitive(graphicsDevice, u2, u4, u3, Vector3.Up, new List<Vector2>() { Vector2.UnitX, Vector2.One, Vector2.UnitY }, boxTexture));

             //Face down
             triangles.Add(new TrianglePrimitive(graphicsDevice, d1, d2, d3, Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
             triangles.Add(new TrianglePrimitive(graphicsDevice, d2, d4, d3, Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));

             //Front face
             triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u1, d2,Vector3.Forward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
             triangles.Add(new TrianglePrimitive(graphicsDevice, u1, u2, d2,  Vector3.Forward, new List<Vector2>() { Vector2.One, Vector2.UnitX, Vector2.UnitY }, boxTexture));

             //Back face
             triangles.Add(new TrianglePrimitive(graphicsDevice, d3, u3, d4, Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
             triangles.Add(new TrianglePrimitive(graphicsDevice, u3, u4, d4, Vector3.Backward, new List<Vector2>() { Vector2.UnitX, Vector2.One, Vector2.UnitY }, boxTexture));

             //Left face
             triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u1, u3, Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
             triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u3, d3,  Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.One }, boxTexture));

             // Rigth face
             triangles.Add(new TrianglePrimitive(graphicsDevice, d2, u2, u4, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
             triangles.Add(new TrianglePrimitive(graphicsDevice, d2, u4, d4, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.One }, boxTexture));
            */
        }


        //No cargamos ningun modelo
        public override void CreateModel(ContentManager content)
        {
            throw new System.NotSupportedException();
        }

        public override StaticDescription GetStaticDescription(Simulation simulation)
        {
            var points = new QuickList<System.Numerics.Vector3>(triangles.Count * 3, simulation.BufferPool);
            foreach (TrianglePrimitive t in triangles)
            {
                foreach(var v in t.Vertices)
                {
                    Vector3 vertex1 = Vector3.Transform(v.Position, base.WorldMatrix);
                    points.AllocateUnsafely() = new System.Numerics.Vector3(vertex1.X, vertex1.Y, vertex1.Z);
                }
            }

             NumericVector3 center = new NumericVector3();
             ConvexHull r = new ConvexHull(points.Span.Slice(points.Count), simulation.BufferPool, out center);


             StaticDescription sta = new StaticDescription(new NumericVector3(center.X, center.Y, center.Z),
                new CollidableDescription(simulation.Shapes.Add(r), 0.001f));

             StaticHandle handle = simulation.Statics.Add(sta);
             SimulationHandle = handle.Value;

            return sta;
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
