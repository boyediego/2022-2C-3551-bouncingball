using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Models.Commons;

namespace TGC.MonoGame.TP.Shared
{
    public static class SharedObjects
    {
        public static GraphicsDeviceManager graphicsDeviceManager { get; set; }
        public static Camera CurrentCamera { get; set; }
        public static IScene CurrentScene { get; set; }
        public static RenderTargetCube CurrentEnvironmentMapRenderTarget { get; set; }
    }
}
