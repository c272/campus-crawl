﻿using CampusCrawl.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using tileEngine.SDK.Diagnostics;

namespace CampusCrawl.Entities.Weapons
{
    internal class Fists : Weapon
    {
        int lungeSize = 10;
        public Fists() : base("Assets/Fists.png", "Fists")
        {
            damage = 1;
            range = 1;
            knockback = 10;
        }
        
        public void Lunge(float multiplier,bool player)
        {
            range += 2;
            damage += 1 * multiplier;
            float[] attackDirection = character.mouseDirection();
            if (!player)
                attackDirection = character.attackDirection;
            character.PushSelf(-(int)attackDirection[0] * lungeSize, -(int)attackDirection[1] * lungeSize);
            Attack(true,player);
            damage -= 1 * multiplier;
            range -= 2;
        }
    }
}
