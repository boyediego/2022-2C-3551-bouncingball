using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene.Parts;
using TGC.MonoGame.TP.Models.Scene.Parts.Checkpoints;
using TGC.MonoGame.TP.Models.Scene.Parts.Obstacule.Base;
using TGC.MonoGame.TP.Models.Scene.Parts.Powerups;
using TGC.MonoGame.TP.Models.Scene.Parts.Roads;

namespace TGC.MonoGame.TP.Models.Scene.Builder
{
    public class CustomBuilder
    {
        protected List<Model3D> models;

        private Vector3 LastPosition;

        public CustomBuilder()
        {
            models = new List<Model3D>();
        }

        protected Model3D Last()
        {
            if (models.Count == 0)
            {
                throw new Exception("You need call StartRoad First!");
            }

            return models.Last(x => x.IsRoad);
        }

        public List<Model3D> GetScene()
        {
            return this.models;
        }

        private float rotations = 0f;
        private Boolean lastPlataform = false;

        public CustomBuilder addTramo(Tramo t)
        {

            return addTramo(t, false);
        }
        public CustomBuilder addPlataform(Tramo t)
        {
            return addTramo(t, true);
        }
        private CustomBuilder addTramo(Tramo t, Boolean platform)
        {
            if (models.Count == 0)
            {
                this.models.Add(
                                t
                                .SetTranslation(new Vector3(0, 200, 0))
                                .Build()
                                .To3DModel()
                            );

                LastPosition = Vector3.Transform(t.EndPoint, t.To3DModel().WorldMatrix);
            }
            else
            {
                Tramo last = (Tramo)Last();

                if (!platform && !lastPlataform)
                {
                    t.SetWidth(last.ActualWidth);
                }
                else if (platform)
                {
                    this.addForwardSpace(200);
                }
                this.models.Add(
                                t.SetRotation(rotations)
                                 .SetTranslation(LastPosition)
                                 .Build()
                                 .To3DModel()
                            );

                LastPosition = Vector3.Transform(t.EndPoint, t.To3DModel().WorldMatrix);

            }
            rotations += t.ActualRotation;
            Debug.WriteLine("Position:" + LastPosition);

            lastPlataform = platform;
            return this;
        }

        public CustomBuilder Rotate90Degress()
        {
            this.rotations += MathHelper.PiOver2;
            return this;
        }

        public CustomBuilder Rotate90DegressClockwise()
        {
            this.rotations -= MathHelper.PiOver2;
            return this;
        }


        public CustomBuilder addForwardSpace(float length)
        {
            Vector3 offset = Vector3.Transform(Vector3.Backward, Matrix.CreateRotationY(rotations));
            offset.Normalize();
            LastPosition += offset * length;
            return this;
        }

        public CustomBuilder addVerticalSpace(float h)
        {
            this.LastPosition = new Vector3(LastPosition.X, LastPosition.Y + h, LastPosition.Z);
            return this;
        }

        public CustomBuilder addSideSpace(Vector3 side, float length)
        {
            Vector3 offset = Vector3.Transform(side, Matrix.CreateRotationY(rotations));
            offset.Normalize();
            LastPosition += offset * length;
            return this;
        }

        public CustomBuilder addPowerup(Powerup powerup, Vector3 offset)
        {
            Tramo last = (Tramo)Last();
            powerup.SetPosition(last.Center + new Vector3(0, GameParams.PowerupYOffset, 0) + offset);
            models.Add(powerup);
            return this;
        }

        public CustomBuilder addObstacule(Obstacule obstacule)
        {
            Tramo last = (Tramo)Last();
            obstacule.ExternalTransformation = Matrix.CreateRotationY(rotations);
            obstacule.SetPositionFromOrigin(last.Center + obstacule.InitialOffset);
            models.Add(obstacule);
            return this;
        }

        public CustomBuilder addCheckpoint(float checkpointWidth)
        {
            Tramo last = (Tramo)Last();
            models.Add(new Checkpoint(last.Center, checkpointWidth));
            return this;
        }

        internal CustomBuilder End(float checkpointWidth)
        {
            Tramo last = (Tramo)Last();
            models.Add(new EndRoad(last.Center, checkpointWidth));
            return this;
        }
    }
}
