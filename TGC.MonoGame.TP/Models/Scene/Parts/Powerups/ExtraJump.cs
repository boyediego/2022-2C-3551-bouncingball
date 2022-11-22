using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using TGC.MonoGame.TP.Models.Players;
using TGC.MonoGame.TP.Shared;
using TGC.MonoGame.TP.Utilities.Geometries;
using NumericVector3 = System.Numerics.Vector3;
using Vector3 = Microsoft.Xna.Framework.Vector3;


namespace TGC.MonoGame.TP.Models.Scene.Parts.Powerups
{
    public class ExtraJump : Powerup
    {

        public override double DurationTime { get { return 8000; } }

        public ExtraJump() : base()
        {

        }

        public override void SetEffectAndTextures(Model content)
        {
            base.Effect = EffectsHolder.Get("LightEffect");
            this.texture = TexturesHolder<Texture2D>.Get("Powerup");
            this.textureNormal = TexturesHolder<Texture2D>.Get("Powerup-Normal");
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState)
        {
           

            base.Update(gameTime, keyboardState);
        }

        private float KA = 0.54598886f;
        private float KD = 0.31298852f;
        private float KS = 0.79898363f;
        private float S = 2f;



        public override void DrawPowerUp(GameTime gameTime, Matrix view, Matrix projection, String techniques)
        {
            var graphicsDevice = SharedObjects.graphicsDeviceManager.GraphicsDevice;
            var oldRasterizerState = graphicsDevice.RasterizerState;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            foreach (TrianglePrimitive triangle in base.triangles)
            {
                Effect.CurrentTechnique = Effect.Techniques[techniques];

                Effect.Parameters["lightPosition"].SetValue(base.Position + Vector3.Transform(new Vector3(800, 1200, -1500), base.WorldMatrix));

                Effect.Parameters["diffuseColor"].SetValue(new Vector3(1f, 0.9f, 0f));
                Effect.Parameters["specularColor"].SetValue(new Vector3(1f, 0.9f, 0f));

                Effect.Parameters["ModelTexture"].SetValue(triangle.Texture);
                Effect.Parameters["NormalTexture"].SetValue(triangle.TextureNormal);
                Effect.Parameters["World"].SetValue(WorldMatrix);
                Effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Invert(Matrix.Transpose(WorldMatrix)));
                Effect.Parameters["WorldViewProjection"].SetValue(WorldMatrix * view * projection);
                Effect.Parameters["Tiling"].SetValue(new Vector2(1f, 1f));
                Effect.Parameters["KAmbient"].SetValue(KA);
                Effect.Parameters["KDiffuse"].SetValue(KD);
                Effect.Parameters["KSpecular"].SetValue(KS);
                Effect.Parameters["shininess"].SetValue(S);

                triangle.Draw(Effect);
            }
            graphicsDevice.RasterizerState = oldRasterizerState;
            EffectsHolder.Get("LightEffect").Parameters["lightPosition"].SetValue(SharedObjects.CurrentScene.LightPosition);
            EffectsHolder.Get("LightEffect").Parameters["ambientColor"].SetValue(SharedObjects.CurrentScene.AmbientLightColor);
            EffectsHolder.Get("LightEffect").Parameters["diffuseColor"].SetValue(SharedObjects.CurrentScene.DiffuseLightColor);
            EffectsHolder.Get("LightEffect").Parameters["specularColor"].SetValue(SharedObjects.CurrentScene.SpecularLightColor);
        }

        public override void ApplyPowerUp(Ball ball)
        {
            ball.IncreaseJump(50);
        }

        public override void Restore(Ball ball)
        {
            ball.RestoreJump();
        }
    }
}
