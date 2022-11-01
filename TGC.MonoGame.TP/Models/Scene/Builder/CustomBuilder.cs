using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene.Parts;
using TGC.MonoGame.TP.Models.Scene.Parts.Powerups;

namespace TGC.MonoGame.TP.Models.Scene.Builder
{
    public class CustomBuilder
    {
        protected List<Model3D> models;
        protected ContentManager contentManager;

        private Vector3 LastPosition;

        public CustomBuilder(ContentManager contentManager)
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

            return models.Last(x => x.IsRoad);
        }

        public List<Model3D> GetScene()
        {
            return this.models;
        }

        private float rotations = 0f;
        public CustomBuilder addTramo(Tramo t)
        {
            if(models.Count == 0)
            {
                this.models.Add(
                                t
                                .SetTranslation(new Vector3(0,200,0))
                                .Build()
                                .To3DModel()
                            );

                LastPosition = Vector3.Transform(t.EndPoint, t.To3DModel().WorldMatrix);
            }
            else
            {
                Tramo last = (Tramo)Last();
                rotations += last.ActualRotation;
                this.models.Add(
                                t
                            .SetWidth(last.ActualWidth)
                            .SetRotation(rotations)
                            .SetTranslation(LastPosition)                
                            .Build()
                                .To3DModel()
                            );

                LastPosition = Vector3.Transform(t.EndPoint, t.To3DModel().WorldMatrix);

            }

            Debug.WriteLine("Position:" + LastPosition);

            return this;
        }


        public CustomBuilder addHorizontalSpace(float length)
        {
            Vector3 offset = Vector3.Transform(Vector3.Backward, Matrix.CreateRotationY(rotations));
            offset.Normalize();
            LastPosition += offset * length;
            return this;
        }

        public CustomBuilder addVerticalSpace(float h)
        {
            this.LastPosition = new Vector3(LastPosition.X, LastPosition.Y+h, LastPosition.Z);
            return this;
        }

        internal CustomBuilder addPowerup(Powerup powerup)
        {
            Tramo last = (Tramo)Last();
            powerup.SetPosition(last.Center + new Vector3(0, GameParams.ObstacleAltitudeOffset, 0));
            models.Add(powerup);
            return this;
        }
    }
}
