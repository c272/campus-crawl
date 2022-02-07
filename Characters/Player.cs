﻿using Microsoft.Xna.Framework;
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



        public override void Update(GameTime delta)
        {
            base.Update(delta);
            var time = (float)(delta.ElapsedGameTime.TotalSeconds);
            var movement = InputHandler.GetEvent("Movement");
            Position = new Vector2(Position.X + (movement.Value.X * time * speed), Position.Y + (movement.Value.Y * time * speed));

            //...
        }
    }

}
