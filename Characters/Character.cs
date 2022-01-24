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
using tileEngine.SDK.Input;

namespace CampusCrawl.Characters
{
    internal class Character : GameObject
    {
        private BoxColliderComponent collider;
        private SpriteComponent sprite;
        private float speed = 100;
        public Character()
        {
            collider = new BoxColliderComponent()
            {
                Size = new Vector2(32, 32),
                Location = new Vector2(0, 0)
            };
            Components.Add(collider);
            sprite = new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(1215427970),
                Position = new Vector2(0, 0),
                Scale = new Vector2(1, 1)
            };
            Components.Add(sprite);
            TileEngine.Instance.KeyboardInput.AddAxisBinding(Keys.D, Keys.A, Keys.S, Keys.W, "Movement");
        }

    

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            var time = (float)(delta.ElapsedGameTime.TotalSeconds);
            var movement = InputHandler.GetEvent("Movement");
            this.Position = new Vector2(Position.X + (movement.Value.X*time*this.speed), Position.Y + (movement.Value.Y*time*this.speed));
            
            //...
        }
    }
}
