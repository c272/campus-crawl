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
using tileEngine.SDK.Input;

namespace CampusCrawl.Characters
{
    public class Character : GameObject
    {
        protected BoxColliderComponent collider;
        protected SpriteComponent sprite;
        protected float speed;
        protected float health;
        protected float xKnockBack = 0;
        protected float yKnockBack = 0;
        protected bool knockBacked = false;
        protected float knockBackDistance = 0;
        public Character()
        {

        }

        public void onDamage(float damage, float x, float y)
        {
            health -= damage;
            xKnockBack = x;
            yKnockBack = y;
            knockBacked = true;
            knockBackDistance = 40;
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);

        }
    }
}
