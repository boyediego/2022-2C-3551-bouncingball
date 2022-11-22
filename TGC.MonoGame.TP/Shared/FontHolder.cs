﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TGC.MonoGame.TP.Shared
{
    public static class FontHolder<T> 
    {
        private static Dictionary<String, T> list = new Dictionary<string, T>();


        public static void Load(ContentManager Content, String keyName, String path)
        {
            var texture = Content.Load<T>(Folders.ContentFolderSpriteFonts + path);
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