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
using TrianglePrimitive = TGC.MonoGame.TP.Utilities.Geometries.TrianglePrimitive;
using System.Runtime.Serialization.Formatters;
using TGC.MonoGame.TP.Utilities;
using BepuUtilities.Collections;
using TGC.MonoGame.TP.Models.Commons;
using BepuPhysics.Constraints;
using TGC.MonoGame.TP.Shared;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP.Models.Scene.Parts.Roads
{
    internal class CustomRoad : Ground, Tramo
    {

        private List<TrianglePrimitive> triangles = new List<TrianglePrimitive>();
        private Vector3 u1;
        private Vector3 u2;
        private Vector3 u3;
        private Vector3 u4;

        private Vector3 d1;
        private Vector3 d2;
        private Vector3 d3;
        private Vector3 d4;


        public override bool IsRoad { get { return true; } }
        public float ActualWidth { get; private set; }
        public float ActualRotation { get; private set; }
        public float ActualElevation { get; private set; }
        public Vector3 StartPoint { get; private set; }
        public Vector3 EndPoint { get; private set; }
        public Vector3 Center { get { return Vector3.Transform(new Vector3(w / 2, h, l / 2), WorldMatrix); } }

        private float w, l, h, elevation, baseElevationOffset;


        private Texture2D boxTexture;
        private Texture2D normalTexture;

        private Texture2D boxSidesTexture;
        private Texture2D boxSidesNormalTexture;

        public Tramo Build()
        {
            var graphicsDevice = SharedObjects.graphicsDeviceManager.GraphicsDevice;
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

            //Face up
            triangles.Add(new TrianglePrimitive(graphicsDevice, u1, u2, u3, Vector3.Up, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture, normalTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u2, u4, u3, Vector3.Up, new List<Vector2>() { Vector2.UnitX, Vector2.One, Vector2.UnitY }, boxTexture, normalTexture));

            //Face down
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, d2, d3, Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxSidesTexture, boxSidesNormalTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, d4, d3, Vector3.Backward, new List<Vector2>() { Vector2.UnitX, Vector2.One, Vector2.UnitY }, boxSidesTexture, boxSidesNormalTexture));

            //Front face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u1, d2, Vector3.Forward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxSidesTexture, boxSidesNormalTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u1, u2, d2, Vector3.Forward, new List<Vector2>() { Vector2.UnitX, Vector2.One, Vector2.UnitY }, boxSidesTexture, boxSidesNormalTexture));

            //Back face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d3, u3, d4, Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxSidesTexture, boxSidesNormalTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u3, u4, d4, Vector3.Backward, new List<Vector2>() { Vector2.UnitX, Vector2.One, Vector2.UnitY }, boxSidesTexture, boxSidesNormalTexture));

            //Left face
           triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u1, u3, Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.One }, boxSidesTexture, boxSidesNormalTexture));
           triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u3, d3, Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.One }, boxSidesTexture, boxSidesNormalTexture));

            // Rigth face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, u2, u4, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxSidesTexture, boxSidesNormalTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, u4, d4, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.One }, boxSidesTexture, boxSidesNormalTexture));


            ActualWidth = w;
            ActualRotation = 0f;
            ActualElevation = elevation;
            StartPoint = new Vector3(x, y, z);
            EndPoint = new Vector3(x, elevation, z + l);

            return this;
        }

        public Model3D To3DModel()
        {
            return this;
        }

        public Tramo SetWidth(float width)
        {
            w = width;
            return this;
        }

        public Tramo SetRotation(float radians)
        {
            RotationMatrix = Matrix.CreateRotationY(radians);
            return this;
        }

        public Tramo SetTranslation(Vector3 translation)
        {
            TranslationMatrix = Matrix.CreateTranslation(translation);
            return this;
        }

        

        public CustomRoad(float w, float l, float h, float elevation, float baseElevationOffset, String KeyTexture) : base(null)
        {
            this.w = w;
            this.l = l;
            this.h = h;
            this.elevation = elevation;
            this.baseElevationOffset = baseElevationOffset;
            this.boxTexture = TexturesHolder<Texture2D>.Get(KeyTexture);
            this.normalTexture = TexturesHolder<Texture2D>.Get(KeyTexture + "-Normal");

            this.boxSidesTexture = TexturesHolder<Texture2D>.Get("Grass-Type-1");
            this.boxSidesNormalTexture = TexturesHolder<Texture2D>.Get("Grass-Type-1-Normal");
        }


        //No cargamos ningun modelo
        public override void SetEffectAndTextures(Model model)
        {
            base.Effect = EffectsHolder.Get("LightEffect");
         
        }

        public override StaticDescription GetStaticDescription(Simulation simulation)
        {

            var points = new QuickList<NumericVector3>(8, simulation.BufferPool);

            Vector3 vertex1 = Vector3.Transform(u1, WorldMatrix);
            Vector3 vertex2 = Vector3.Transform(u2, WorldMatrix);
            Vector3 vertex3 = Vector3.Transform(u3, WorldMatrix);
            Vector3 vertex4 = Vector3.Transform(u4, WorldMatrix);

            Vector3 vertex5 = Vector3.Transform(d1, WorldMatrix);
            Vector3 vertex6 = Vector3.Transform(d2, WorldMatrix);
            Vector3 vertex7 = Vector3.Transform(d3, WorldMatrix);
            Vector3 vertex8 = Vector3.Transform(d4, WorldMatrix);


            points.AllocateUnsafely() = new NumericVector3(vertex1.X, vertex1.Y, vertex1.Z);
            points.AllocateUnsafely() = new NumericVector3(vertex2.X, vertex2.Y, vertex2.Z);
            points.AllocateUnsafely() = new NumericVector3(vertex3.X, vertex3.Y, vertex3.Z);
            points.AllocateUnsafely() = new NumericVector3(vertex4.X, vertex4.Y, vertex4.Z);
            points.AllocateUnsafely() = new NumericVector3(vertex5.X, vertex5.Y, vertex5.Z);
            points.AllocateUnsafely() = new NumericVector3(vertex6.X, vertex6.Y, vertex6.Z);
            points.AllocateUnsafely() = new NumericVector3(vertex7.X, vertex7.Y, vertex7.Z);
            points.AllocateUnsafely() = new NumericVector3(vertex8.X, vertex8.Y, vertex8.Z);

            NumericVector3 center = new NumericVector3();
            ConvexHull r = new ConvexHull(points.Span.Slice(points.Count), simulation.BufferPool, out center);


            StaticDescription sta = new StaticDescription(new NumericVector3(center.X, center.Y, center.Z),
               new CollidableDescription(simulation.Shapes.Add(r), 0.0001f));

            StaticHandle handle = simulation.Statics.Add(sta);
            SimulationHandle = handle.Value;

            return new StaticDescription();
        }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection, String techniques)
        {
            var graphicsDevice = SharedObjects.graphicsDeviceManager.GraphicsDevice;
            var oldRasterizerState = graphicsDevice.RasterizerState;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            foreach (TrianglePrimitive triangle in triangles)
            {
                Effect.CurrentTechnique = Effect.Techniques[techniques];
                Effect.Parameters["ModelTexture"].SetValue(triangle.Texture);
                Effect.Parameters["NormalTexture"].SetValue(triangle.TextureNormal);
                Effect.Parameters["World"].SetValue(WorldMatrix);
                Effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Invert(Matrix.Transpose(WorldMatrix)));
                Effect.Parameters["WorldViewProjection"].SetValue(WorldMatrix * view * projection);
                Effect.Parameters["Tiling"].SetValue(new Vector2(1f, 10f));
                Effect.Parameters["KAmbient"].SetValue(0.60f);
                Effect.Parameters["KDiffuse"].SetValue(0.5f);
                Effect.Parameters["KSpecular"].SetValue(0.4f);
                Effect.Parameters["shininess"].SetValue(6.0f);
                triangle.Draw(Effect);
            }
            graphicsDevice.RasterizerState = oldRasterizerState;
        }

        internal void SetTranslation(float v1, float v2, float v3)
        {
            TranslationMatrix = Matrix.CreateTranslation(v1, v2, v3);
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {

        }

        public override void Collide(Model3D sceneObject)
        {

        }
    }
}
