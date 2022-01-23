using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using tileEngine.SDK.Components;

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
                Texture = AssetManager.AttemptLoad<Texture2D>(-1280955819),
                Position = new Vector2(0, 0),
                Scale = new Vector2(1,1)
            }); 
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            //...
        }
    }
}
