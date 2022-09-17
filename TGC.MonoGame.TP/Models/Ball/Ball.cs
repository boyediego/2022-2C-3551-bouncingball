using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;

namespace TGC.MonoGame.TP.Models.Ball
{
    public class Ball : Model3D
    {
        private Matrix World;
        private Matrix Translation;

        private Vector3 color;

        Random r = new Random((int)DateTime.Now.Ticks);

        public Ball(ContentManager content) : base(content, "balls/sphere")
        {
        }

        public override void CreateModel(ContentManager content)
        {
            Effect = content.Load<Effect>(ContentFolderEffects + "BasicShader");


            // Get the first texture we find
            // The city model only contains a single texture
            var effect = Model.Meshes.FirstOrDefault().Effects.FirstOrDefault() as BasicEffect;


            color = new Vector3(1,0,0);


            // Assign the mesh effect
            // A model contains a collection of meshes
            foreach (var mesh in Model.Meshes)
            {
                // A mesh contains a collection of parts
                foreach (var meshPart in mesh.MeshParts)
                {
                    // Assign the loaded effect to each part
                    meshPart.Effect = Effect;
                }
            }

            // Create a list of places where the city model will be drawn

            World = Matrix.Identity;
        }

        public void SetPositionFromOrigin(Vector3 position)
        {
            Translation = Matrix.CreateTranslation(position);
            World = Matrix.Identity * Translation;
        }

        public override void Draw(GameTime gameTime, Matrix view, Matrix projection)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);


            var modelMeshesBaseTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            // For each mesh in the model,
            foreach (var mesh in Model.Meshes)
            {
                // Obtain the world matrix for that mesh (relative to the parent)
                var meshWorld = modelMeshesBaseTransforms[mesh.ParentBone.Index];


                Effect.Parameters["DiffuseColor"].SetValue(color);


                // We set the main matrices for each mesh to draw
                Effect.Parameters["World"].SetValue(meshWorld * World * Matrix.CreateScale(0.2f) * Matrix.CreateTranslation(new Vector3(0,130,0)));


                // Draw the mesh
                mesh.Draw();
            }
        }
    }

}
