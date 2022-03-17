using CampusCrawl.Characters;
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
    internal class Knife : Weapon
    {
        public Knife() : base("Assets/Knife.png", "Knife")
        {
            damage = 2;
            range = 1;
            knockback = 15;
        }
    }
}
