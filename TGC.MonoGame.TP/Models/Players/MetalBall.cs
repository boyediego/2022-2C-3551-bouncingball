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
            this.texture = TexturesHolder<Texture2D>.Get("Metal");
            this.textureNormal = TexturesHolder<Texture2D>.Get("Metal-Normal");
        }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["ModelTexture"].SetValue(texture);
            Effect.Parameters["NormalTexture"].SetValue(textureNormal);
            Effect.Parameters["Tiling"].SetValue(Vector2.One);
            Effect.Parameters["eyePosition"].SetValue(SharedObjects.CurrentCamera.Position);
            Effect.Parameters["lightPosition"].SetValue(SharedObjects.CurrentScene.LightPosition + new Vector3(this.Position.X, this.Position.Y, 0));
            Effect.Parameters["ambientColor"].SetValue(SharedObjects.CurrentScene.AmbientLightColor);
            Effect.Parameters["diffuseColor"].SetValue(SharedObjects.CurrentScene.DiffuseLightColor);
            Effect.Parameters["specularColor"].SetValue(SharedObjects.CurrentScene.SpecularLightColor);
            Effect.Parameters["KAmbient"].SetValue(0.3f);
            Effect.Parameters["KDiffuse"].SetValue(0.5f);
            Effect.Parameters["KSpecular"].SetValue(0.75f);
            Effect.Parameters["shininess"].SetValue(4.0f);
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
