using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.TP.Shared;

namespace TGC.MonoGame.TP.Models.Players
{
    public class MetalBall : Ball
    {
        private Texture2D texture;
        private Texture2D textureNormal;

        protected override float ForwardImpulse { get { return 30f; } }
        protected override float BrakeForce { get { return 30f; } }
        protected override float RotateForce { get { return 25f; } }
        protected override float JumpImpulse { get { return 1000f; } }

        public MetalBall(Simulation Simulation, Vector3 startPosition) : base(Simulation, ModelsHolder.Get("Sphere"), startPosition)
        {

        }

        public override void SetEffectAndTextures(Model model)
        {
            base.Model = model;
            base.Effect = EffectsHolder.Get("LightEffect");
            this.texture = TexturesHolder<Texture2D>.Get("Test");
            this.textureNormal = TexturesHolder<Texture2D>.Get("Test-Normal");
        }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["ModelTexture"].SetValue(texture);
            Effect.Parameters["NormalTexture"].SetValue(textureNormal);
            Effect.Parameters["Tiling"].SetValue(Vector2.One);
            Effect.Parameters["eyePosition"].SetValue(SharedObjects.CurrentCamera.Position);
            Effect.Parameters["lightPosition"].SetValue(new Vector3(this.Position.X + 16000, this.Position.Y + 14000f, 8000));
            Effect.Parameters["ambientColor"].SetValue(new Vector3(1f, 1f, 1f));
            Effect.Parameters["diffuseColor"].SetValue(new Vector3(0.5f, 0.1f, 0f));
            Effect.Parameters["specularColor"].SetValue(new Vector3(0.5f, 0.1f, 0f));
            Effect.Parameters["KAmbient"].SetValue(0.7f);
            Effect.Parameters["KDiffuse"].SetValue(0.7f);
            Effect.Parameters["KSpecular"].SetValue(0.1f);
            Effect.Parameters["shininess"].SetValue(16.0f);
            Effect.CurrentTechnique = Effect.Techniques["NormalMapping"];

            var modelMeshesBaseTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            foreach (var mesh in Model.Meshes)
            {
                var meshWorld = modelMeshesBaseTransforms[mesh.ParentBone.Index];
                Effect.Parameters["World"].SetValue(this.WorldMatrix);
                Effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Invert(Matrix.Transpose(meshWorld * this.WorldMatrix)));
                Effect.Parameters["WorldViewProjection"].SetValue(meshWorld * this.WorldMatrix * view * projection);
                mesh.Draw();
            }

        }
    }
}
