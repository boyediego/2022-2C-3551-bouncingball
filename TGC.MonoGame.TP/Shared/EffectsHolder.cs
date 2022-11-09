using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace TGC.MonoGame.TP.Shared
{
    public static class EffectsHolder
    {
        private static Dictionary<String, Effect> list = new Dictionary<string, Effect>();


        public static void Load(ContentManager Content, String keyName, String effectPath)
        {
            var effect = Content.Load<Effect>(Folders.ContentFolderEffects + effectPath);
            list.Add(keyName, effect);
        }

        public static Effect Get(String keyName)
        {
            return list[keyName];
        }

    }
}
