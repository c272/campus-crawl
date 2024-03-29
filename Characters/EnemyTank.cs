﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;

namespace CampusCrawl.Characters
{
    internal class EnemyTank : Enemy
    {
        public EnemyTank(string directionName, float distance, Vector2 location) : base(directionName, distance, location)
        {
            playerModelPath = "FinalAssets/YorkGuy.png";
            sprite.Texture = AssetManager.AttemptLoad<Texture2D>(playerModelPath);
            speed = 50;
            damage = 20;
            scoreValue = 150;
        }
    }
}
