using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;

namespace TGC.MonoGame.TP.Models.Scene.Builder
{
    public abstract class SceneBuilder
    {
        protected List<Model3D> models;
        protected ContentManager contentManager;

        public abstract SceneBuilder StartRoad();
        public abstract SceneBuilder AddForward();
        public abstract SceneBuilder AddRight();
        public abstract SceneBuilder AddBackward();
        public abstract SceneBuilder AddLeft();
        public abstract SceneBuilder Up();
        public abstract SceneBuilder Down();

        public SceneBuilder AddForward(int times)
        {
            for (int i = 0; i < times; i++)
            {
                AddForward();
            }

            return this;
        }

        public SceneBuilder AddBackward(int times)
        {
            for (int i = 0; i < times; i++)
            {
                AddBackward();
            }

            return this;
        }

        public SceneBuilder AddRight(int times)
        {
            for (int i = 0; i < times; i++)
            {
                AddRight();
            }

            return this;
        }

        public SceneBuilder AddLeft(int times)
        {
            for (int i = 0; i < times; i++)
            {
                AddLeft();
            }

            return this;
        }

        public SceneBuilder(ContentManager contentManager)
        {
            this.contentManager = contentManager;
            models = new List<Model3D>();
        }

        protected Model3D Last()
        {
            if (models.Count == 0)
            {
                throw new Exception("You need call StartRoad First!");
            }

            return models[models.Count - 1];
        }

        public List<Model3D> GetScene()
        {
            return this.models;
        }
            
    }
}
