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
using TGC.MonoGame.TP.Models.Scene.Parts.Obstacule.Base;


namespace TGC.MonoGame.TP.Models.Scene.Parts.Obstacule
{
    public class CubeFixedObstacule : TGC.MonoGame.TP.Models.Scene.Parts.Obstacule.Base.Obstacule
    {
        private Texture2D texture;
        private Texture2D textureNormal;

        private List<TrianglePrimitive> triangles = new List<TrianglePrimitive>();

        private float Size;

        public override int PhysicsType { get { return PhysicsTypeHome.Static; } }

        public CubeFixedObstacule() : base(null)
        {

        }

        public override void SetEffectAndTextures(Model model)
        {
            base.Effect = EffectsHolder.Get("LightEffect");
            this.texture = TexturesHolder<Texture2D>.Get("Stone-Type-1");
            this.textureNormal = TexturesHolder<Texture2D>.Get("Stone-Type-1-Normal");
        }

        public CubeFixedObstacule Build(float size)
        {
            var graphicsDevice = SharedObjects.graphicsDeviceManager.GraphicsDevice;

            float h = size / 2;

            //Calculate vertex
            //Up face
            var u1 = new Vector3(-h, h, h);
            var u2 = new Vector3(h, h, h);
            var u3 = new Vector3(-h, h, -h);
            var u4 = new Vector3(h, h, -h);
            //Down face
            var d1 = new Vector3(-h, -h, h);
            var d2 = new Vector3(h, -h, h);
            var d3 = new Vector3(-h, -h, -h);
            var d4 = new Vector3(h, -h, -h);


            //Face up
            triangles.Add(new TrianglePrimitive(graphicsDevice, u1, u2, u3, Vector3.Up, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, texture, textureNormal));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u2, u4, u3, Vector3.Up, new List<Vector2>() { Vector2.UnitX, Vector2.One, Vector2.UnitY }, texture, textureNormal));

            //Face down
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, d2, d3, Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitX, Vector2.UnitY }, texture, textureNormal));
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, d4, d3, Vector3.Backward, new List<Vector2>() { Vector2.UnitX, Vector2.One, Vector2.UnitY }, texture, textureNormal));

            //Front face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u1, d2, Vector3.Forward, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.UnitX }, texture, textureNormal));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u1, u2, d2, Vector3.Forward, new List<Vector2>() { Vector2.UnitY, Vector2.One, Vector2.UnitX }, texture, textureNormal));

            //Back face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d3, u3, d4, Vector3.Backward, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.UnitX }, texture, textureNormal));
            triangles.Add(new TrianglePrimitive(graphicsDevice, u3, u4, d4, Vector3.Backward, new List<Vector2>() { Vector2.UnitY, Vector2.One, Vector2.UnitX }, texture, textureNormal));

            //Left face
           triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u1, u3, Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.One }, texture, textureNormal));
           triangles.Add(new TrianglePrimitive(graphicsDevice, d1, u3, d3, Vector3.Left, new List<Vector2>() { Vector2.Zero, Vector2.One, Vector2.UnitX }, texture, textureNormal));

            // Rigth face
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, u2, u4, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.UnitY, Vector2.One }, texture, textureNormal));
            triangles.Add(new TrianglePrimitive(graphicsDevice, d2, u4, d4, Vector3.Right, new List<Vector2>() { Vector2.Zero, Vector2.One, Vector2.UnitX }, texture, textureNormal));


            this.Size = size;
            return this;
        }


        public override BodyDescription GetBodyDescription(Simulation simulation){ throw new NotSupportedException(); }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            var graphicsDevice = SharedObjects.graphicsDeviceManager.GraphicsDevice;
            var oldRasterizerState = graphicsDevice.RasterizerState;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            foreach (TrianglePrimitive triangle in triangles)
            {
                Effect.Parameters["ModelTexture"].SetValue(triangle.Texture);
                Effect.Parameters["NormalTexture"].SetValue(triangle.TextureNormal);
                Effect.Parameters["World"].SetValue(WorldMatrix);
                Effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Invert(Matrix.Transpose(WorldMatrix)));
                Effect.Parameters["WorldViewProjection"].SetValue(WorldMatrix * view * projection);
                Effect.Parameters["Tiling"].SetValue(new Vector2(1f, 4f));
                Effect.Parameters["eyePosition"].SetValue(SharedObjects.CurrentCamera.Position);
                Effect.Parameters["lightPosition"].SetValue(SharedObjects.CurrentScene.LightPosition + new Vector3(this.Position.X, 0, this.Position.Z));
                Effect.Parameters["ambientColor"].SetValue(SharedObjects.CurrentScene.AmbientLightColor);
                Effect.Parameters["diffuseColor"].SetValue(SharedObjects.CurrentScene.DiffuseLightColor);
                Effect.Parameters["specularColor"].SetValue(SharedObjects.CurrentScene.SpecularLightColor);
                Effect.Parameters["KAmbient"].SetValue(0.2f);
                Effect.Parameters["KDiffuse"].SetValue(0.75f);
                Effect.Parameters["KSpecular"].SetValue(0.2f);
                Effect.Parameters["shininess"].SetValue(12.0f);
                Effect.CurrentTechnique = Effect.Techniques["NormalMapping"];

                triangle.Draw(Effect);
            }
            graphicsDevice.RasterizerState = oldRasterizerState;
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            
        }

        public override void Collide(Model3D sceneObject)
        {
            
        }

        public override StaticDescription GetStaticDescription(Simulation simulation)
        {
            Vector3 center = Vector3.Transform(new Vector3(0, 0, 0), base.WorldMatrix);
            var shape = new Box(this.Size, this.Size, this.Size);
            StaticDescription sta = new StaticDescription(new NumericVector3(center.X, center.Y, center.Z),
               new CollidableDescription(simulation.Shapes.Add(shape), 0.01f));

            StaticHandle handle = simulation.Statics.Add(sta);
            SimulationHandle = handle.Value;

            return new StaticDescription();

        }
    }

}
