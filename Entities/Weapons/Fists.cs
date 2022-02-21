using CampusCrawl.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;

namespace CampusCrawl.Entities.Weapons
{
    internal class Fists : Weapon
    {
        public Fists(Character character, string playerModel) : base(character, playerModel, "Assets/TestModel.png")
        {
            // assetPath = the model that is shown on the ground to be picked up
            // restingModel = the model of the player with 
            name = "Fists";
            string modelWithoutExtension = playerModel.Replace(".png", "");
            restingModel = modelWithoutExtension + "_" + name + "_Resting.png";
            attackedModel = modelWithoutExtension + "_" + name + "_Attacked.png";
            // Assets/TestModel.png = playerModel
            // Assets/TestModel_Fists_Resting.png = restingModel
            // Assets/TestModel_Fists_Attacked.png = attackedModel

            character.UpdateSprite(new tileEngine.SDK.Components.SpriteComponent
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(1215427970), // Texture = AssetManager.AttemptLoad<Texture2D>(restingModel),
                Position = new Vector2(-16, -16),
                Scale = new Vector2(1, 1)
            });
        }

        public override void Attack()
        {
            
        }
    }
}
