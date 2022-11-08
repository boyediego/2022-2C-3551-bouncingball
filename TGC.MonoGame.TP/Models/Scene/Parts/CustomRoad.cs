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
using TGC.MonoGame.TP.Models.Commons;
using BepuPhysics.Constraints;

namespace TGC.MonoGame.TP.Models.Scene.Parts
{
    internal class CustomRoad : Ground, Tramo
    {
        private GraphicsDevice graphicsDevice;
        private ContentManager content;

        Vector3 u1;
        Vector3 u2;
        Vector3 u3;
        Vector3 u4;

        Vector3 d1;
        Vector3 d2;
        Vector3 d3;
        Vector3 d4;


        public override Boolean IsRoad { get { return true; } }
        public float ActualWidth { get; private set; }
        public float ActualRotation { get; private set; }
        public float ActualElevation { get; private set; }
        public Vector3 StartPoint { get; private set; }
        public Vector3 EndPoint { get; private set; }
        public Vector3 Center
        {
            get
            {
                return Vector3.Transform(new Vector3(w / 2, h, l / 2), base.WorldMatrix);
            }
        }

        private float w, l, h, elevation, baseElevationOffset;


        private Texture2D boxTexture;
        private Texture2D normalTexture;

        public Tramo Build()
        {
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
            triangles.Add(new TrianglePrimitive(graphicsDevice, u1, u2, u3, Vector3.Up, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u2, u4, u3, Vector3.Up, new List<Vector2>() { Vector2.UnitX, Vector2.One, Vector2.UnitY }, boxTexture));

            //Face down
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, d2, d3, Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, d4, d3, Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));

            //Front face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u1, d2, Vector3.Forward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u1, u2, d2, Vector3.Forward, new List<Vector2>() { Vector2.One, Vector2.UnitX, Vector2.UnitY }, boxTexture));

            //Back face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d3, u3, d4, Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u3, u4, d4, Vector3.Backward, new List<Vector2>() { Vector2.UnitX, Vector2.One, Vector2.UnitY }, boxTexture));

            //Left face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u1, u3, Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u3, d3, Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.One }, boxTexture));

            // Rigth face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, u2, u4, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, boxTexture));
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, u4, d4, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.One }, boxTexture));


            this.ActualWidth = w;
            this.ActualRotation = 0f;
            this.ActualElevation = elevation;
            this.StartPoint = new Vector3(x, y, z);
            this.EndPoint = new Vector3(x, elevation, z + l);

            return this;
        }

        public Model3D To3DModel()
        {
            return (Model3D)this;
        }

        public Tramo SetWidth(float width)
        {
            this.w = width;
            return this;
        }

        public Tramo SetRotation(float radians)
        {
            base.RotationMatrix = Matrix.CreateRotationY(radians);
            return this;
        }

        public Tramo SetTranslation(Vector3 translation)
        {
            base.TranslationMatrix = Matrix.CreateTranslation(translation);
            return this;
        }

        private List<TrianglePrimitive> triangles = new List<TrianglePrimitive>();

        public CustomRoad(ContentManager content, GraphicsDevice graphicsDevice, float w, float l, float h, float elevation, float baseElevationOffset) : base(content, null)
        {
            this.content = content;
            this.graphicsDevice = graphicsDevice;
            this.w = w;
            this.l = l;
            this.h = h;
            this.elevation = elevation;
            this.baseElevationOffset = baseElevationOffset;
            boxTexture = content.Load<Texture2D>(TGCGame.ContentFolderTextures + "extras/cemento");
            normalTexture = content.Load<Texture2D>(TGCGame.ContentFolderTextures + "extras/cemento-normal-map");
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

            NumericVector3 center = new NumericVector3();
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
               /* var triangleEffect = triangle.Effect;
                triangleEffect.View = view;
                triangleEffect.Projection = projection;
                triangleEffect.World = this.WorldMatrix;
                triangleEffect.LightingEnabled = false;*/

                 var triangleEffect = TGCGame.LightEffects;

                triangleEffect.Parameters["ModelTexture"].SetValue(boxTexture);
                triangleEffect.Parameters["NormalTexture"].SetValue(normalTexture);
                triangleEffect.Parameters["World"].SetValue(this.WorldMatrix);
                triangleEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Invert(Matrix.Transpose(this.WorldMatrix)));
                triangleEffect.Parameters["WorldViewProjection"].SetValue(this.WorldMatrix * view * projection);
                triangleEffect.Parameters["Tiling"].SetValue(Vector2.One);
                triangleEffect.Parameters["eyePosition"].SetValue(TGCGame.Camera.Position);
                
                triangleEffect.Parameters["lightPosition"].SetValue(new Vector3(Center.X+16000, Center.Y+14000f, 8000));
                triangleEffect.Parameters["ambientColor"].SetValue(new Vector3(1f, 1f, 1f));
                triangleEffect.Parameters["diffuseColor"].SetValue(new Vector3(0.5f, 0.1f, 0f));
                triangleEffect.Parameters["specularColor"].SetValue(new Vector3(0.5f, 0.1f, 0f));
                triangleEffect.Parameters["KAmbient"].SetValue(0.4f);
                triangleEffect.Parameters["KDiffuse"].SetValue(0.7f);
                triangleEffect.Parameters["KSpecular"].SetValue(0.9f);
                triangleEffect.Parameters["shininess"].SetValue(16.0f);
                triangleEffect.CurrentTechnique = triangleEffect.Techniques["NormalMapping"];

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
