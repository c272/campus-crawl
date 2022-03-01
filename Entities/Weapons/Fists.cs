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
    internal class Fists : Weapon
    {
        int lungeSize = 10;
        public Fists(Character character) : base(character, "Assets/Fists.png", "Fists")
        {
            damage = 1;
            range = 1;
            knockback = 10;
            // assetPath = the model that is shown on the ground to be picked up
            // restingModel = the model of the player with 

            // Assets/TestModel.png = playerModel
            // Assets/TestModel_Fists_Resting.png = restingModel
            // Assets/TestModel_Fists_Attacked.png = attackedModel
        }
        
        public void Lunge(float multiplier,bool player)
        {
            range += 2;
            damage += 1 * multiplier;
            float[] attackDirection = character.mouseDirection();
            character.PushSelf(-(int)attackDirection[0] * lungeSize, -(int)attackDirection[1] * lungeSize);
            Attack(true,player);
            range -= 2;
        }
    }
}
