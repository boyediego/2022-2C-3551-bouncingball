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
using Matrix = Microsoft.Xna.Framework.Matrix;
using System.Runtime.Serialization.Formatters;
using TGC.MonoGame.TP.Utilities;
using BepuUtilities.Collections;
using System.Threading;
using System.Security;
using static System.Net.Mime.MediaTypeNames;
using Extreme.Mathematics;
using System.Diagnostics;
using Extreme.DataAnalysis;
using System.Reflection;
using TGC.MonoGame.TP.Models.Commons;

namespace TGC.MonoGame.TP.Models.Scene.Parts
{
    internal class CustomCurvedRoad : Ground, Tramo
    {
        private GraphicsDevice graphicsDevice;
        private ContentManager content;

        private List<TrianglePrimitive> triangles = new List<TrianglePrimitive>();
        private List<TrianglePrimitive> trianglesPhysics = new List<TrianglePrimitive>();


        public override Boolean IsRoad { get { return true; } }
        public float ActualWidth { get; private set; }
        public float ActualRotation { get; private set; }
        public float ActualElevation { get; private set; }
        public Vector3 StartPoint { get; private set; }
        public Vector3 EndPoint { get; private set; }
        public Vector3 Center { get; set; }


        private Vector3 startPosition;
        private Vector3 endPosition;
        private float w;
        private float h;

        public Tramo Build()
        {
            int sign =(int) (w / MathF.Abs(w));
            BezierPath path = new BezierPath();
            Vector3 controlPoint1 = new Vector3(startPosition.X + (startPosition.Length() * 1f / 4f) - 1000 * sign, startPosition.Y, startPosition.Z + (startPosition.Length() * 1f / 4f) + 1000);
            Vector3 controlPoint3 = new Vector3(startPosition.X + (startPosition.Length() * 3f / 4f) - 1000 * sign, startPosition.Y, startPosition.Z + (startPosition.Length() * 3f / 4f) + 1000);
            path.SetControlPoints(new List<Vector3>() { startPosition, controlPoint1, controlPoint3, endPosition });

            List<Vector3> points = path.GetDrawingPoints1();

            var boxTexture = content.Load<Texture2D>(TGCGame.ContentFolderTextures + "extras/basev2");
            var boxTexture1 = content.Load<Texture2D>(TGCGame.ContentFolder3D + "scene/basics/wood");

            Vector3 lastPosition = new Vector3();
            Vector3 preLastPosition = new Vector3();
            {
                var current = points[points.Count - 2];
                var next = points[points.Count - 1];
                Vector3 p6 = next + new Vector3(w, 0, 0);
                Vector3 vectorW = new Vector3(w, 0, 0);
                Vector3 p3 = current + vectorW;

                Vector3 vd = (next - current).PerpendicularCounterClockwiseIn2D();
                vd.Normalize();

                Vector3 vd1 = p6 - p3;
                vd1.Normalize();

                var m = Extreme.Mathematics.Matrix.Create(2, 2, new double[]
                            {
                                (double)vd.X, (double)vd.Z,
                                (double)vd1.X, (double)vd1.Z,
                            }, MatrixElementOrder.ColumnMajor);

                var b = Extreme.Mathematics.Vector.Create((double)(-next.X + p6.X), (double)(-next.Z + p6.Z));
                var x = m.Solve(b);
                var x1 = x[0] * vd.X + next.X;
                var z1 = x[0] * vd.Z + next.Z;
                p6 = new Vector3((float)x1, p6.Y, (float)z1);
                lastPosition = p6;
            }

            var cut = false;
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

                trianglesPhysics.Add(triangles[triangles.Count - 2]);
                // trianglesPhysics.Add(triangles[triangles.Count - 1]);


                Vector3 p4 = current + vectorW;
                Vector3 p5 = next;
                Vector3 p6 = next + new Vector3(w, 0, 0);


                if (Math.Abs(p6.X) > Math.Abs(lastPosition.X))
                {
                    preLastPosition = p5;
                    p6 = lastPosition;
                    cut = true;
                }

                if (index == points.Count - 2 && !cut)
                {
                    //Face Right
                    triangles.Add(new TrianglePrimitive(graphicsDevice, p1, p1 - new Vector3(0, h, 0), p2 - new Vector3(0, h, 0), Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
                    triangles.Add(new TrianglePrimitive(graphicsDevice, p2 - new Vector3(0, h, 0), p1, p2, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));

                    preLastPosition = p4;
                    break;
                }


                //Face up
                triangles.Add(new TrianglePrimitive(graphicsDevice, p4, p5, p6, Vector3.Up, new List<Vector2>() { texturaP3P4, texturaP2P5, texturaP6 }, boxTexture));
                //Face down
                triangles.Add(new TrianglePrimitive(graphicsDevice, p4 - new Vector3(0, h, 0), p5 - new Vector3(0, h, 0), p6 - new Vector3(0, h, 0), Vector3.Down, new List<Vector2>() { Vector2.UnitY, Vector2.UnitX, Vector2.One }, boxTexture));
                trianglesPhysics.Add(triangles[triangles.Count - 2]);
                //  trianglesPhysics.Add(triangles[triangles.Count - 1]);

                //Face Left
                triangles.Add(new TrianglePrimitive(graphicsDevice, p3, p3 - new Vector3(0, h, 0), p6 - new Vector3(0, h, 0), Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
                triangles.Add(new TrianglePrimitive(graphicsDevice, p6 - new Vector3(0, h, 0), p3, p6, Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));

                //Face Right
                triangles.Add(new TrianglePrimitive(graphicsDevice, p1, p1 - new Vector3(0, h, 0), p2 - new Vector3(0, h, 0), Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
                triangles.Add(new TrianglePrimitive(graphicsDevice, p2 - new Vector3(0, h, 0), p1, p2, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));


                if (cut)
                {
                    break;
                }
            }

            //End up face
            triangles.Add(new TrianglePrimitive(graphicsDevice, lastPosition, endPosition, preLastPosition, Vector3.Up, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture1));
            trianglesPhysics.Add(triangles[triangles.Count - 1]);

            //End down face
            triangles.Add(new TrianglePrimitive(graphicsDevice, lastPosition - new Vector3(0, h, 0), endPosition - new Vector3(0, h, 0), preLastPosition - new Vector3(0, h, 0), Vector3.Down, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture1));
            trianglesPhysics.Add(triangles[triangles.Count - 1]);

            if (!cut)
            {
                //Face Left
                triangles.Add(new TrianglePrimitive(graphicsDevice, lastPosition, lastPosition - new Vector3(0, h, 0), preLastPosition, Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
                triangles.Add(new TrianglePrimitive(graphicsDevice, preLastPosition, preLastPosition - new Vector3(0, h, 0), lastPosition - new Vector3(0, h, 0), Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
            }
            else
            {
                //Face Right
                triangles.Add(new TrianglePrimitive(graphicsDevice, preLastPosition, endPosition - new Vector3(0, h, 0), endPosition, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
                triangles.Add(new TrianglePrimitive(graphicsDevice, preLastPosition - new Vector3(0, h, 0), preLastPosition, endPosition - new Vector3(0, h, 0), Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
            }

            //Front face
            triangles.Add(new TrianglePrimitive(graphicsDevice, startPosition, startPosition - new Vector3(0, h, 0), startPosition + new Vector3(w, 0, 0), Vector3.Forward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, startPosition - new Vector3(0, h, 0), startPosition + new Vector3(w, 0, 0), startPosition + new Vector3(w, -h, 0), Vector3.Forward, new List<Vector2>() { Vector2.One, Vector2.UnitX, Vector2.UnitY }, boxTexture));


            //Back face
            // triangles.Add(new TrianglePrimitive(graphicsDevice, endPosition, endPosition - new Vector3(0, h, 0), endPosition + new Vector3(w, 0, 0), Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
            //triangles.Add(new TrianglePrimitive(graphicsDevice, endPosition - new Vector3(0, h, 0), endPosition + new Vector3(w, 0, 0), endPosition + new Vector3(w, -h, 0), Vector3.Backward, new List<Vector2>() { Vector2.One, Vector2.UnitX, Vector2.UnitY }, boxTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, endPosition, endPosition - new Vector3(0, h, 0), lastPosition, Color.Red, Color.Green, Color.Blue, Vector3.Backward));
            triangles.Add(new TrianglePrimitive(graphicsDevice, endPosition - new Vector3(0, h, 0), lastPosition, lastPosition - new Vector3(0, h, 0), Color.Red, Color.Green, Color.Blue, Vector3.Backward));


            this.ActualWidth = Vector3.Distance(endPosition, lastPosition);
            this.ActualRotation = -MathF.Atan2((lastPosition - endPosition).Z, (lastPosition - endPosition).X);
            this.StartPoint = startPosition;
            this.EndPoint = endPosition - new Vector3(0, h, 0);
            this.ActualElevation = lastPosition.Y;

            //If turn left
            //End width=Vector3.Distance(endPosition, lastPosition) //1648.21619
            //Rotation=-MathF.Atan2((lastPosition-endPosition).Z,(lastPosition-endPosition).X//-0.6021707

            return this;
        }

        public Model3D To3DModel()
        {
            return (Model3D)this;
        }

        public Tramo SetTranslation(Vector3 position)
        {
            base.TranslationMatrix = Matrix.CreateTranslation(position);
            return this;
        }

        public Tramo SetRotation(float radians)
        {
            base.RotationMatrix = Matrix.CreateRotationY(radians);
            return this;
        }

        public Tramo SetWidth(float width)
        {
            this.w = width;
            return this;
        }

        public CustomCurvedRoad(ContentManager content, GraphicsDevice graphicsDevice, Vector3 startPosition, Vector3 endPosition, float w, float h) : base(content, null)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.w = w;
            this.h = h;
        }

        //No cargamos ningun modelo
        public override void CreateModel(ContentManager content)
        {
            throw new System.NotSupportedException();
        }

        public override StaticDescription GetStaticDescription(Simulation simulation)
        {
            var points = new QuickList<System.Numerics.Vector3>(trianglesPhysics.Count * 3, simulation.BufferPool);
            foreach (TrianglePrimitive t in trianglesPhysics)
            {
                foreach (var v in t.Vertices)
                {
                    Vector3 vertex1 = Vector3.Transform(v.Position, base.WorldMatrix);
                    points.AllocateUnsafely() = new System.Numerics.Vector3(vertex1.X, vertex1.Y, vertex1.Z);
                }
            }

            NumericVector3 center = new NumericVector3();
            ConvexHull r = new ConvexHull(points.Span.Slice(points.Count), simulation.BufferPool, out center);
            this.Center = new Vector3(center.X, center.Y, center.Z);

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


    }
}
