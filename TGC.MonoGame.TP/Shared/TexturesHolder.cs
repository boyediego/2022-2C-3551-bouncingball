using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TGC.MonoGame.TP.Shared
{
    public static class TexturesHolder<T> where T : Texture
    {
        private static Dictionary<String, T> list = new Dictionary<string, T>();


        public static void Load(ContentManager Content, String keyName, String texturePath)
        {
            var texture = Content.Load<T>(Folders.ContentFolderTextures + texturePath);
            list.Add(keyName, texture);
        }

        public static T Get(String keyName)
        {
            return (T)list[keyName];
        }

        public static void Add(String keyName, T texture)
        {
            list.Add(keyName, texture);
        }
    }
}
