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

        public bool checkAttack(Vector2 newPos)
        {

            Point currentTile = Scene.GridToTileLocation(newPos);
            var enemies = Scene.GameObjects.Where(x => x is Enemy).ToArray();
            bool hit = false;
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

        public virtual bool Attack(bool lungeAttack)
        {
            isAttacking = true;
            character.attackDirection = character.mouseDirection();
            Point currentTile = Scene.GridToTileLocation(character.Position);

            var enemies = Scene.GameObjects.Where(x => x is Enemy).ToArray();
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
                    if ((currentTile.X - enemyTile.X <= character.attackDirection[0] && currentTile.Y - enemyTile.Y <= character.attackDirection[1]) || enemyTile == currentTile)
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

