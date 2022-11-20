using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Models.Commons
{
    public interface IGameModel
    {
        void SetEffectAndTextures(Model model);
        void Update(GameTime gameTime, KeyboardState keyboardState);
        void Draw(GameTime gameTime, Matrix view, Matrix projection, String techniques);
    }
}
