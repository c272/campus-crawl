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
        public Fists(Character character, string playerModel) : base(character, playerModel, "Assets/Fists.png", "Fists")
        {
            damage = 1;
            range = 2;
            knockback = 20;
            // assetPath = the model that is shown on the ground to be picked up
            // restingModel = the model of the player with 

            // Assets/TestModel.png = playerModel
            // Assets/TestModel_Fists_Resting.png = restingModel
            // Assets/TestModel_Fists_Attacked.png = attackedModel
        }
        
        public void Lunge(float multiplier)
        {
            range += 2;
            damage += 1 * multiplier;
            DiagnosticsHook.DebugMessage("Doing " + damage + " damage (multiplier of: " + multiplier + ")");
            float[] attackDirection = character.mouseDirection();
            character.PushSelf(-(int)attackDirection[0] * lungeSize, -(int)attackDirection[1] * lungeSize);
            Attack();
            range -= 2;
        }
    }
}
