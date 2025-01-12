﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene.Parts.Obstacule.Base;
using TGC.MonoGame.TP.Models.Scene.Parts.Powerups;

namespace TGC.MonoGame.TP.Models.Scene.Builder
{
    public abstract class SceneBuilder
    {
        protected List<Model3D> models;
        


        public abstract SceneBuilder StartRoad(Vector3 initialPosition);
        public abstract SceneBuilder StartRoad();
        public abstract SceneBuilder AddForward();
        public abstract SceneBuilder AddRight();
        public abstract SceneBuilder AddBackward();
        public abstract SceneBuilder AddLeft();
        public abstract SceneBuilder Up();
        public abstract SceneBuilder Down();
        public abstract SceneBuilder AddObstacule(Obstacule cubeObstacule);
        public abstract SceneBuilder AddCheckpoint(float checkpointWidth);
        public abstract SceneBuilder AddPowerup(Powerup powerup);

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

        public SceneBuilder()
        {
            models = new List<Model3D>();
        }

        protected Model3D Last()
        {
            if (models.Count == 0)
            {
                throw new Exception("You need call StartRoad First!");
            }

            return models.Last(x=> x.IsRoad);
        }

        public List<Model3D> GetScene()
        {
            return this.models;
        }

        
        
    }
}
