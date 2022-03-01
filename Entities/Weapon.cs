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
            }
            else if (mouseDirection[0] == 1)
            {
                // Check X for the radius.
                if (playerTile.X - range > enemyTile.X || playerTile.X < enemyTile.X)
                {
                    return false;
                }
            }
            else if (mouseDirection[0] == 0 && playerTile.X != enemyTile.X)
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
            }
            else if (mouseDirection[1] == -1)
            {
                // Check to see if Y is within the radius for attack.
                if (playerTile.Y > enemyTile.Y || playerTile.Y + range < enemyTile.Y)
                {
                    return false;
                }
            }
            else if (mouseDirection[1] == 0 && playerTile.Y != enemyTile.Y)
            {
                return false;
            }

            if (mouseDirection[0] != 0 || mouseDirection[1] != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkAttack(Vector2 newPos,bool player)
        {

            Point currentTile = Scene.GridToTileLocation(newPos);
            bool hit = false;
            var enemies = Scene.GameObjects.Where(x => x is Character).ToArray();
            if (player)
            {
                enemies = Scene.GameObjects.Where(x => x is Enemy).ToArray();
            }
            foreach (GameObject enemy in enemies)
            {
                Enemy currentEnemy = (Enemy)enemy;
                Point enemyPos = new Point((int)currentEnemy.Position.X, (int)currentEnemy.Position.Y);
                Point enemyTile = Scene.GridToTileLocation(enemy.Position);
                Point currentPos = new Point((int)newPos.X, (int)newPos.Y);
                if ((currentTile.X - enemyTile.X <= character.attackDirection[0] && currentTile.Y - enemyTile.Y <= character.attackDirection[1]) || enemyTile == currentTile)
                {
                    currentEnemy.onDamage(damage, character.attackDirection, (int)(knockback * 1.5));
                    hit = true;
                }
                else if (Math.Abs(enemyPos.X - currentPos.X) < 40 && Math.Abs(enemyPos.Y - currentPos.Y) < 40)
                {
                    currentEnemy.onDamage(damage, character.attackDirection, (int)(knockback * 1.5));
                    hit = true;
                }
            }
            if (hit) { return true; }
            return false;
        }

        public virtual bool Attack(bool lungeAttack,bool player)
        {
            isAttacking = true;
            if (player)
            {
                character.attackDirection = character.mouseDirection();
            }
            Point currentTile = Scene.GridToTileLocation(character.Position);
            var enemies = Scene.GameObjects.Where(x => x is Character).ToArray();
            if (player)
            {
                enemies = Scene.GameObjects.Where(x => x is Enemy).ToArray();
            }
            foreach (Enemy enemy in enemies)
            {
                Point enemyTile = Scene.GridToTileLocation(enemy.Position);
                if (lungeAttack)
                {
                    if (Math.Abs(enemy.Position.X - character.Position.X) < 40 && Math.Abs(enemy.Position.Y - character.Position.Y) < 40)
                    {
                        enemy.onDamage(damage, character.attackDirection, (int)(knockback * 1.5));
                        isAttacking = false;
                    }
                }
                else
                {
                    if (canAttack(enemyTile, currentTile, character.attackDirection))
                    {
                        enemy.onDamage(damage, character.attackDirection, knockback);
                        isAttacking = false;
                    }
                }
            }

            if(isAttacking)
                return false;
                
            return true;
        }
    }
}

