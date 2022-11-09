using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Models.Commons
{
    public interface IScene
    {
        Vector3 LightPosition { get; }
        Vector3 AmbientLightColor { get; }
        Vector3 DiffuseLightColor { get; }
        Vector3 SpecularLightColor { get; }
    }
}
