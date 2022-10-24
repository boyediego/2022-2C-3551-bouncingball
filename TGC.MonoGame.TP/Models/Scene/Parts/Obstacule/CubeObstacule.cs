﻿using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using TGC.MonoGame.TP.Models.Commons;
using TGC.MonoGame.TP.Models.Scene.Parts.Obstacule.Base;
using TGC.MonoGame.TP.Utilities;

namespace TGC.MonoGame.TP.Models.Scene.Parts.Obstacule
{
    public class CubeObstacule : BouncingObstacule
    {        
        public CubeObstacule(ContentManager content) : base(content, "scene/basics/cubo")
        {
        }

        protected override void LoadEffectAndParameters(ContentManager content)
        {
            Effect = content.Load<Effect>(ContentFolderEffects + "BasicShader");
            color = new Vector3(0, 1, 0);
        }

        protected override bool CheckCollision(GameTime gameTime, List<IGameModel> otherInteractiveObjects)
        {
            return false;
        }

        
    }

}
