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
        public abstract int PhysicsType { get; }
        public abstract Boolean IsGround { get; }
        public virtual Boolean IsRoad { get { return false; } }
        public int SimulationHandle { get; set; }

        protected Model Model { get; set; }

        protected Matrix ScaleMatrix;
        protected Matrix TranslationMatrix;
        protected Matrix RotationMatrix;
        protected Effect Effect { get; set; }

        public Matrix WorldMatrix
        {
            get
            {
                return ScaleMatrix * RotationMatrix * TranslationMatrix;
            }
        }

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

        public Vector3 GetModelSize()
        {
            return this.Model.GetBoundingBox(RotationMatrix).Max - this.Model.GetBoundingBox(RotationMatrix).Min;
        }

        public Model3D(Model model)
        {
            this.Model = model;
            ScaleMatrix = Matrix.Identity;
            TranslationMatrix = Matrix.Identity;
            RotationMatrix = Matrix.Identity;
            SetEffectAndTextures(model);
            if(model!=null && Effect != null)
            {
                foreach (var mesh in Model.Meshes)
                {
                    foreach (var meshPart in mesh.MeshParts)
                    {
                        meshPart.Effect = this.Effect;
                    }
                }
            }
        }


        public abstract void SetEffectAndTextures(Model content);
        public abstract void Update(GameTime gameTime, KeyboardState keyboardState);
        public abstract void Draw(GameTime gameTime, Matrix view, Matrix projection);
        public abstract void Collide(Model3D sceneObject);

        public abstract StaticDescription GetStaticDescription(Simulation simulation);
        public abstract BodyDescription GetBodyDescription(Simulation simulation);
    }
}
