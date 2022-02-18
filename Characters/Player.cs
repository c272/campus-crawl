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
        private Point spawnPoint = new Point();

        public Player()
        {
            sprite = new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(1215427970),
                Position = new Vector2(0, 0),
                Scale = new Vector2(1, 1)
            };
            AddComponent(sprite);
            speed = 110;
            health = 100;
        }

        public override void Initialize()
        {
            base.Initialize();
            collider = new BoxColliderComponent()
            {
                Size = new Vector2(Scene.Map.TileTextureSize - 1, Scene.Map.TileTextureSize - 1),
                Location = new Vector2(0, 0)
            };
            spawnPoint = new Point(0,0);
            AddComponent(collider);
        }


        public void respawn()
        {
            Position = Scene.TileToGridLocation(spawnPoint);
            health = 100;
            knockBacked = false;
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            var time = (float)(delta.ElapsedGameTime.TotalSeconds);
            var movement = InputHandler.GetEvent("Movement");
            if(health <= 0)
            {
                respawn();
            }

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
