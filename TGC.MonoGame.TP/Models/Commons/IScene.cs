using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Models.Scene.Parts.Checkpoints;

namespace TGC.MonoGame.TP.Models.Commons
{
    public interface IScene : IGameModel
    {
        Vector3 LightPosition { get; set; }
        Vector3 AmbientLightColor { get; }
        Vector3 DiffuseLightColor { get; }
        Vector3 SpecularLightColor { get; }
        List<Model3D> models { get; }

        void RestorePowerups(Checkpoint current);
        void End();

        event EventHandler EndGame;

    }
}
