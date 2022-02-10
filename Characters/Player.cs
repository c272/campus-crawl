using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using tileEngine.SDK.Components;
using tileEngine.SDK.Diagnostics;
using tileEngine.SDK.Input;

namespace CampusCrawl.Characters
{
    public class Player : Character
    {
        private BoxColliderComponent collider;
        private SpriteComponent sprite;
        private float speed = 110;
        private float health = 100;
        private float xKnockBack = 0;
        private float yKnockBack = 0;
        private bool knockBacked = false;
        private float knockBackDistance = 0;
        public Player()
        {
            collider = new BoxColliderComponent()
            {
                Size = new Vector2(31, 31),
                Location = new Vector2(0, 0)
            };
            AddComponent(collider);
            sprite = new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(1215427970),
                Position = new Vector2(0, 0),
                Scale = new Vector2(1, 1)
            };
            AddComponent(sprite);
        }

        public void onDamage(float damage,float x, float y)
        {
            health -= damage;
            xKnockBack = x;
            yKnockBack = y;
            knockBacked = true;
            knockBackDistance = 33;
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            var time = (float)(delta.ElapsedGameTime.TotalSeconds);
            var movement = InputHandler.GetEvent("Movement");
            if (!knockBacked)
            {
                Position = new Vector2(Position.X + (movement.Value.X * time * speed), Position.Y + (movement.Value.Y * time * speed));
            }
            if (knockBacked)
            {
                Position = new Vector2(Position.X + (xKnockBack * time * speed), Position.Y + (yKnockBack * time * speed));
                if (xKnockBack != 0)
                {
                    knockBackDistance -= Math.Abs(xKnockBack * time * speed);
                }
                else
                {
                    knockBackDistance -= Math.Abs(yKnockBack * time * speed);
                }
                if (knockBackDistance <= 0)
                {
                    knockBacked = false;
                }

            }
            //...
        }
    }

}
