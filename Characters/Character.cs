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
    public class Character : GameObject
    {
        private BoxColliderComponent collider;
        private SpriteComponent sprite;
        private float speed = 110;
        public Character()
        {

        }

    

        public override void Update(GameTime delta)
        {
            base.Update(delta);

        }
    }
}
