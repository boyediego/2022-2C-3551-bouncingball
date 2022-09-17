using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP.Models.Commons
{
    public interface IGameModel
    {
        void CreateModel(ContentManager content);
        void Draw(GameTime gameTime, Matrix view, Matrix projection);
        void Update(GameTime gameTime, KeyboardState keyboardState, List<IGameModel> otherInteractiveObjects);
    }
}
