using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.TP.Cameras;

namespace TGC.MonoGame.TP.Shared
{
    public static class SharedObjects
    {
        public static GraphicsDeviceManager graphicsDeviceManager { get; set; }
        public static Camera CurrentCamera { get; set; }
    }
}
