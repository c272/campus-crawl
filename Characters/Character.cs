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
        public Character()
        {
            Components.Add(new BoxColliderComponent
            {
                Size = new Vector2(32,32),
                Location = new Vector2(0,0)
            });
            Components.Add(new SpriteComponent
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(1215427970),
                Position = new Vector2(0, 0),
                Scale = new Vector2(1,1)
            });
            TileEngine.Instance.KeyboardInput.AddAxisBinding(Keys.D, Keys.A, Keys.S, Keys.W, "Movement");

        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            var movement = InputHandler.GetEvent("Movement");
            this.Position = new Vector2(Position.X + movement.Value.X , Position.Y + movement.Value.Y );
            
            //...
        }
    }
}
