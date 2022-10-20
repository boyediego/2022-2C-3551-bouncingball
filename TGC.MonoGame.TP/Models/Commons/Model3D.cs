using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using TGC.MonoGame.TP.Utilities;

namespace TGC.MonoGame.TP.Models.Commons
{
    public abstract class Model3D : IGameModel
    {
        protected const string ContentFolder3D = "Models/";
        protected const string ContentFolderEffects = "Effects/";

        public abstract int PhysicsType { get; }


        public int SimulationHandle { get; set; }

        protected Model Model { get; set; }
        protected Effect Effect { get; set; }
        public Matrix WorldMatrix
        {
            get
            {
                return ScaleMatrix * RotationMatrix * TranslationMatrix;
            }
        }
        protected Matrix ScaleMatrix;
        protected Matrix TranslationMatrix;
        protected Matrix RotationMatrix;

        public Vector3 CurrentMovementDirection { get; set; }
        public Vector3 Position
        {
            get
            {
                
                return TranslationMatrix.Translation;
            }
        }

        public Matrix Rotation
        {
            get
            {
                return RotationMatrix;
            }
        }

        public Model3D(ContentManager content, String pathModel)
        {
            Model = content.Load<Model>(ContentFolder3D + pathModel);
            ScaleMatrix = Matrix.Identity;
            TranslationMatrix = Matrix.Identity;
            RotationMatrix = Matrix.Identity;
            CreateModel(content);
        }

        protected void SetEffect(Effect effect)
        {
            foreach (var mesh in Model.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }
        }

        public Vector3 GetModelSize()
        {
            return this.Model.GetBoundingBox(RotationMatrix).Max - this.Model.GetBoundingBox(RotationMatrix).Min;
        }

        public abstract void CreateModel(ContentManager content);

        public virtual void Draw(GameTime gameTime, Matrix view, Matrix projection)
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
                SetCustomEffectParameters(Effect);


                // We set the main matrices for each mesh to draw
                Effect.Parameters["World"].SetValue(meshWorld * WorldMatrix);


                // Draw the mesh
                mesh.Draw();
            }
        }

        public abstract void SetCustomEffectParameters(Effect effect);

        public virtual void Update(GameTime gameTime, KeyboardState keyboardState, List<IGameModel> otherInteractiveObjects)
        {
            //TODO IMPLEMTS IN CHILD CLASESS
        }

        public abstract StaticDescription GetStaticDescription(Simulation simulation);
        public abstract BodyDescription GetBodyDescription(Simulation simulation);

        public virtual void Collide(Model3D sceneObject)
        {
            Debug.WriteLine("YES " + sceneObject.GetType());
        }
    }
}
