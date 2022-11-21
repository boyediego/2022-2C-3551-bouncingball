using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Shared;

namespace TGC.MonoGame.TP.Models.Players
{
    public class PlasticBall : Ball
    {
        private Texture2D texture;
        private Texture2D textureNormal;

        public override string Aceleracion { get { return "8"; } }
        public override string Salto { get { return "8"; } }
        public override string Freno { get { return "4"; } }
        public override string Control { get { return "5"; } }

        protected override float ForwardImpulse { get { return 50f; } }
        protected override float BrakeForce { get { return 20f; } }
        protected override float RotateForce { get { return 35f; } }
        protected override float JumpImpulse { get { return 1300f; } }
        protected override float TopSpeed { get { return 400f; } }
        public override Boolean HasEnviromentMap { get { return false; } }

        public PlasticBall(Simulation Simulation, Vector3 startPosition) : base(Simulation, ModelsHolder.Get("Sphere"), startPosition)
        {

        }

        public override void SetEffectAndTextures(Model model)
        {
            base.Model = model;
            base.Effect = EffectsHolder.Get("LightEffect");
            this.texture = TexturesHolder<Texture2D>.Get("Plastic");
            this.textureNormal = TexturesHolder<Texture2D>.Get("Plastic-Normal");
        }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection, String techniques)
        {
            Effect.CurrentTechnique = Effect.Techniques[techniques];
            Effect.Parameters["ModelTexture"].SetValue(texture);
            Effect.Parameters["NormalTexture"].SetValue(textureNormal);
            Effect.Parameters["Tiling"].SetValue(Vector2.One);
            Effect.Parameters["KAmbient"].SetValue(0.35199812f);
            Effect.Parameters["KDiffuse"].SetValue(0.57399464f);
            Effect.Parameters["KSpecular"].SetValue(0.5f);
            Effect.Parameters["shininess"].SetValue(64f);

            Effect.Parameters["hasEnviroment"].SetValue(HasEnviromentMap ? 1f : 0f);
            Effect.Parameters["environmentMap"].SetValue(SharedObjects.CurrentEnvironmentMapRenderTarget);
            
            

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
