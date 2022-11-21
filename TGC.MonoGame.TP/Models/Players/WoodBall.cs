using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.TP.Shared;

namespace TGC.MonoGame.TP.Models.Players
{
    public class WoodBall : Ball
    {
        private Texture2D texture;
        private Texture2D textureNormal;

        protected override float ForwardImpulse { get { return 25f; } }
        protected override float BrakeForce { get { return 40f; } }
        protected override float RotateForce { get { return 45f; } }
        protected override float JumpImpulse { get { return 1200f; } }
        protected override float TopSpeed { get { return 500f; } }
        public override Boolean HasEnviromentMap { get { return false; } }

        public WoodBall(Simulation Simulation, Vector3 startPosition) : base(Simulation, ModelsHolder.Get("Sphere"), startPosition)
        {

        }

        public override void SetEffectAndTextures(Model model)
        {
            base.Model = model;
            base.Effect = EffectsHolder.Get("LightEffect");
            this.texture = TexturesHolder<Texture2D>.Get("OldWood");
            this.textureNormal = TexturesHolder<Texture2D>.Get("OldWood-Normal");
        }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection, String techniques)
        {
            Effect.CurrentTechnique = Effect.Techniques[techniques];
            Effect.Parameters["ModelTexture"].SetValue(texture);
            Effect.Parameters["NormalTexture"].SetValue(textureNormal);
            Effect.Parameters["Tiling"].SetValue(Vector2.One);
            Effect.Parameters["KAmbient"].SetValue(0.60999167f);
            Effect.Parameters["KDiffuse"].SetValue(0.66199124f);
            Effect.Parameters["KSpecular"].SetValue(0.7279942f);
            Effect.Parameters["shininess"].SetValue(46f);


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
