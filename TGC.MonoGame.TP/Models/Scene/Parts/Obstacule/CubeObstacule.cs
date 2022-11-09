using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene.Parts.Obstacule.Base;
using TGC.MonoGame.TP.Shared;
using TGC.MonoGame.TP.Utilities;

namespace TGC.MonoGame.TP.Models.Scene.Parts.Obstacule
{
    public class CubeObstacule : BouncingObstacule
    {
        private Texture2D texture;
        private Texture2D textureNormal;

        public CubeObstacule() : base(ModelsHolder.Get("CubeModel"))
        {
           
        }


        public override void SetEffectAndTextures(Model model)
        {
            base.Effect = EffectsHolder.Get("LightEffect");
            this.texture = TexturesHolder<Texture2D>.Get("Stone-Type-1");
            this.textureNormal = TexturesHolder<Texture2D>.Get("Stone-Type-1-Normal");
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

            /* Effect.Parameters["DiffuseColor"].SetValue(color);
             Effect.Parameters["View"].SetValue(view);
             Effect.Parameters?["Projection"].SetValue(projection);

             var modelMeshesBaseTransforms = new Matrix[Model.Bones.Count];
             Model.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

             foreach (var mesh in Model.Meshes)
             {
                 var meshWorld = modelMeshesBaseTransforms[mesh.ParentBone.Index];
                 Effect.Parameters?["World"].SetValue(meshWorld * WorldMatrix);
                 mesh.Draw();
             }*/
        }

        
    }

}
