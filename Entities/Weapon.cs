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
using tileEngine.SDK.Input;

namespace CampusCrawl.Entities
{
    public abstract class Weapon : Entity
    {
        protected string name;
        protected int knockback = 0;
        protected int range = 0;
        protected float damage = 0;
        public string playerModel;
        public string restingModel;
        public string attackedModel;
        protected bool isAttacking = false;

        protected Character character;
        public Weapon(Character character, string playerModel, string assetPath, string name) : base(assetPath)
        {
            this.character = character;
            this.playerModel = playerModel;
            string modelWithoutExtension = playerModel.Replace(".png", "");
            restingModel = modelWithoutExtension + "_" + name + "_Resting.png";
            attackedModel = modelWithoutExtension + "_" + name + "_Attacked.png";
        }

        public override void PickedUp()
        {
            base.PickedUp();

            character.UpdateSprite(new tileEngine.SDK.Components.SpriteComponent
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(restingModel),
                Position = new Vector2(-16, -16),
                Scale = new Vector2(1, 1)
            });
        }

        public bool canAttack(Point enemyTile, Point playerTile, float[] mouseDirection)
        {
            if (mouseDirection[0] == -1)
            {
                // Check X for the radius.
                if (playerTile.X > enemyTile.X || playerTile.X + range < enemyTile.X)
                {
                    return false;
                }
            } else if (mouseDirection[0] == 1)
            {
                // Check X for the radius.
                if (playerTile.X - range > enemyTile.X || playerTile.X < enemyTile.X)
                {
                    return false;
                }
            } else if (mouseDirection[0] == 0 && playerTile.X != enemyTile.X)
            {
                return false;
            }

            if (mouseDirection[1] == 1)
            {
                // Check to see if Y is within the radius for attack.
                if (playerTile.Y - range > enemyTile.Y || playerTile.Y < enemyTile.Y)
                {
                    return false;
                }
            } else if (mouseDirection[1] == -1)
            {
                // Check to see if Y is within the radius for attack.
                if (playerTile.Y > enemyTile.Y || playerTile.Y + range < enemyTile.Y)
                {
                    return false;
                }
            } else if (mouseDirection[1] == 0 && playerTile.Y != enemyTile.Y)
            {
                return false;
            }

            if (mouseDirection[0] != 0 || mouseDirection[1] != 0)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public virtual void Attack()
        {
            if (!isAttacking)
            {
                isAttacking = true;


                float[] attackDirection = character.mouseDirection();
                Point currentTile = Scene.GridToTileLocation(character.Position);

                var enemies = Scene.GameObjects.Where(x => x is Enemy).ToArray();
                foreach (Enemy enemy in enemies)
                {
                    Point enemyPos = new Point((int)enemy.Position.X, (int)enemy.Position.Y);
                    Point enemyTile = Scene.GridToTileLocation(enemy.Position);

                    if (canAttack(enemyTile, currentTile, attackDirection))
                    {
                        enemy.onDamage(damage, attackDirection, knockback);
                    }
                }

                isAttacking = false;
            }
        }
    }
}

