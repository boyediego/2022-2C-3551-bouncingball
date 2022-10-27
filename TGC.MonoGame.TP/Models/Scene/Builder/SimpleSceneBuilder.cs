using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene.Parts;
using TGC.MonoGame.TP.Models.Scene.Parts.Obstacule.Base;
using TGC.MonoGame.TP.Models.Scene.Parts.Powerups;

namespace TGC.MonoGame.TP.Models.Scene.Builder
{
    public class SimpleSceneBuilder : SceneBuilder
    {

        private Vector3 CurrentRoadDirection;
        private Vector3 CurrentCenter;
        

        public SimpleSceneBuilder(ContentManager contentManager) : base(contentManager)
        {
            
        }

        public override SceneBuilder Up()
        {
            CurrentCenter += new Vector3(0, 150, 0);
            return this;
        }

        public override SceneBuilder Down()
        {
            CurrentCenter += new Vector3(0, -150, 0);
            return this;
        }

        public override SceneBuilder StartRoad()
        {
            Road road = new Road(this.contentManager);
            this.CurrentCenter = new Vector3(0, 0, 0);
            this.CurrentRoadDirection = Vector3.Forward;
            models.Add(road);
            return this;
        }


        public override SceneBuilder StartRoad(Vector3 initialPosition)
        {
            Road road = new Road(this.contentManager);
            this.CurrentCenter = initialPosition;
            this.CurrentRoadDirection = Vector3.Forward;
            models.Add(road);
            return this;
        }

        public override SceneBuilder AddForward()
        {
            Vector3 newDirection = Vector3.Forward;
            Road road = new Road(this.contentManager);

            if (newDirection != CurrentRoadDirection)
            {
                this.CurrentCenter.Z+= ((Last().GetModelSize().Z - road.GetModelSize().X) / 2) ;
                this.CurrentCenter.X-= ((Last().GetModelSize().X - road.GetModelSize().Z) / 2)*CurrentRoadDirection.X;
            }

            this.CurrentCenter += (newDirection * road.GetModelSize());
            road.SetPositionFromOrigin(this.CurrentCenter + newDirection );
            this.CurrentRoadDirection = newDirection;
            models.Add(road);
            return this;
        }


        public override SceneBuilder AddRight()
        {
            Vector3 newDirection = Vector3.Right;
            Road road = new Road(this.contentManager);
            road.RotateY(-MathHelper.PiOver2); //ROTATE 90° DEGREES TO RIGHT
            if (newDirection != CurrentRoadDirection)
            {
               this.CurrentCenter.Z -= (Last().GetModelSize().Z - road.GetModelSize().X) / 2 * -CurrentRoadDirection.Z;
               this.CurrentCenter.X += (Last().GetModelSize().X - road.GetModelSize().Z) / 2;
            }

            Vector3 size = road.GetModelSize();
            this.CurrentCenter += (newDirection * new Vector3(size.Z, size.Y, size.X)) ;
            road.SetPositionFromOrigin(this.CurrentCenter);
            this.CurrentRoadDirection = newDirection;
            
            models.Add(road);
            return this;
        }

        public override SceneBuilder AddBackward()
        {
            Vector3 newDirection = Vector3.Backward;
            Road road = new Road(this.contentManager);

            if (newDirection != CurrentRoadDirection)
            {
                this.CurrentCenter.Z -= (Last().GetModelSize().Z - road.GetModelSize().X) / 2;
                this.CurrentCenter.X += (Last().GetModelSize().X - road.GetModelSize().Z) / 2 * -CurrentRoadDirection.X;
            }

            this.CurrentCenter += (newDirection * road.GetModelSize()) ;
            road.SetPositionFromOrigin(this.CurrentCenter);
            this.CurrentRoadDirection = newDirection;
            
            models.Add(road);
            return this;
        }

        public override SceneBuilder AddLeft()
        {
            Vector3 newDirection = Vector3.Left;
            Road road = new Road(this.contentManager);
            road.RotateY(-MathHelper.PiOver2); //ROTATE 90° DEGREES TO RIGHT
            if (newDirection != CurrentRoadDirection)
            {
                this.CurrentCenter.Z += (Last().GetModelSize().Z - road.GetModelSize().X) / 2 * CurrentRoadDirection.Z;
                this.CurrentCenter.X -= (Last().GetModelSize().X - road.GetModelSize().Z) / 2 ;
            }

            Vector3 size = road.GetModelSize();
            this.CurrentCenter += (newDirection * new Vector3(size.Z, size.Y, size.X)) ;
            road.SetPositionFromOrigin(this.CurrentCenter);
            this.CurrentRoadDirection = newDirection;
            
            models.Add(road);
            return this;
        }

        public override SceneBuilder AddObstacule(Obstacule obstacule)
        {
            obstacule.SetPositionFromOrigin(CurrentCenter + new Vector3(0, GameParams.ObstacleAltitudeOffset, 0) + obstacule.InitialOffset);
            models.Add(obstacule);
            return this;
        }

        public override SceneBuilder AddCheckpoint(float checkpointWidth)
        {
            models.Add(new Checkpoint(this.contentManager, CurrentCenter, checkpointWidth));
            return this;
        }

        public override SceneBuilder AddPowerup(Powerup powerup)
        {
            powerup.SetPosition(CurrentCenter + new Vector3(0, GameParams.ObstacleAltitudeOffset, 0));
            models.Add(powerup);
            return this;
        }


    }
}
