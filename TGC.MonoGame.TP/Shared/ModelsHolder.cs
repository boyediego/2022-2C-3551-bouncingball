using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Linq;

namespace TGC.MonoGame.TP.Shared
{
    public static class ModelsHolder
    {
        private static Dictionary<String, Model> list = new Dictionary<string, Model>();


        public static void Load(ContentManager Content, String keyName, String modelPath)
        {
            var model = Content.Load<Model>(Folders.ContentFolder3D + modelPath);
            list.Add(keyName, model);

            var effect = model.Meshes.FirstOrDefault().Effects.FirstOrDefault() as BasicEffect;
            if (effect != null && effect.Texture !=null)
            {
                TexturesHolder<Texture2D>.Add(keyName + "-Texture", effect.Texture);
            }
        }

        public static Model Get(String keyName)
        {
            return list[keyName];
        }

    }
}
