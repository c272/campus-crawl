using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using tileEngine.SDK.Components;
using tileEngine.SDK.Diagnostics;
using tileEngine.SDK.Input;
using CampusCrawl.Entities;
using CampusCrawl.Entities.Weapons;
using tileEngine.SDK.Utility;

namespace CampusCrawl.Characters
{
    public class Player : Character
    {
        private Point spawnPoint = new Point();
        private Weapon currentWeapon = null;
        private Timer attackCooldown;
        private bool canAttack = true;
        MouseInputHandler mouse;

        public Player()
        {
            sprite = new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(1215427970),
                Position = new Vector2(0, 0),
                Scale = new Vector2(1, 1)
            };
            AddComponent(sprite);
            attackCooldown = new Timer(0.5f);
            attackCooldown.OnTick += cooldown;
            attackCooldown.Loop = true;
            speed = 110;
            health = 100;
            damage = 20;
        }

        public override void Initialize()
        {
            base.Initialize();
            collider = new BoxColliderComponent()
            {
                Size = new Vector2(Scene.Map.TileTextureSize, Scene.Map.TileTextureSize),
                Location = new Vector2(0, 0)
            };
            spawnPoint = new Point(0,0);
            AddComponent(collider);
            mouse = new MouseInputHandler();
            attackCooldown.Start();
        }

        public void spawnRandomWeapon()
        {
            Fists e = new Fists(this, "Assets/TestModel.png");
            e.Scene = Scene;
            e.SetLayer(Layer);
            e.Spawn(Position);
        }

        public void respawn()
        {
            Position = Scene.TileToGridLocation(spawnPoint);
            health = 100;
            knockBacked = false;
        }

        private float[] attackDirection()
        {
            var direction = new float[2] { 0, 0 };
            var mousePos = InputHandler.GetEvent("MousePosition");
            var gridPos = Scene.ToGridLocation(mousePos.Value);
            var mouseTile = Scene.GridToTileLocation(gridPos);
            var currentTile = Scene.GridToTileLocation(Position);
            if (currentTile.X - mouseTile.X >= 0)
            {
                direction[0] = 1;
            }
            if (currentTile.X - mouseTile.X < 0)
            {
                direction[0] = -1;
            }
            if (currentTile.Y - mouseTile.Y >= 0)
            {
                direction[1] = 1;
            }
            if (currentTile.Y - mouseTile.Y < 0)
            {
                direction[1] = -1;
            }
            return direction;
        }


        public void attack()
        {
            canAttack = false;
            if(currentWeapon != null)
                currentWeapon.Attack();
            var mousePos = InputHandler.GetEvent("MousePosition");
            var gridPos = Scene.ToGridLocation(mousePos.Value);
            var direction = attackDirection();
            Point currentTile = Scene.GridToTileLocation(Position);
            var enemies = Scene.GameObjects.Where(x => x is Enemy).ToArray();
            foreach (GameObject enemy in enemies)
            {
                Enemy currentEnemy = (Enemy)enemy;
                if (currentEnemy.knockBacked == false)
                {
                    Point enemyPos = new Point((int)currentEnemy.Position.X, (int)currentEnemy.Position.Y);
                    Point enemyTile = Scene.GridToTileLocation(enemy.Position);
                    Point currentPos = new Point((int)Position.X, (int)Position.Y);
                    if ((currentTile.X - enemyTile.X == direction[0] && currentTile.Y - enemyTile.Y == direction[1]) || enemyTile == currentTile)
                    {
                        
                        if (Math.Abs(enemyPos.X - currentPos.X) < 50 && Math.Abs(enemyPos.Y - currentPos.Y) < 50)
                        {
                            DiagnosticsHook.DebugMessage((enemyTile.X.ToString() + " " + enemyTile.Y.ToString() + " P: " + currentTile.X.ToString() + " " + currentTile.Y.ToString() + "  K: " + direction[0].ToString() + " " + direction[1].ToString()));
                            currentEnemy.onDamage(damage, -direction[0] * 2.5f, -direction[1] * 2.5f);
                        }
                    }
                }
            }

        }

        public void cooldown()
        {
            canAttack = true;
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            var time = (float)(delta.ElapsedGameTime.TotalSeconds);
            var movement = InputHandler.GetEvent("Movement");
            var mouseState = Mouse.GetState();
            var click = 0;
            if (mouseState.LeftButton == ButtonState.Pressed)
                click = 1;
            if (click == 1)
                attack();
            Scene.LookAt(Position);
            if(health <= 0)
            {
                respawn();
            }

            if (!knockBacked)
            {
                Position = new Vector2(Position.X + (movement.Value.X * time * speed), Position.Y + (movement.Value.Y * time * speed));
            }
            if (knockBacked)
            {
                Position = new Vector2(Position.X + (xKnockBack * time * speed), Position.Y + (yKnockBack * time * speed));
                if (xKnockBack != 0)
                {
                    knockBackDistance -= Math.Abs(xKnockBack * time * speed);
                }
                else
                {
                    knockBackDistance -= Math.Abs(yKnockBack * time * speed);
                }
                if (knockBackDistance <= 0)
                {
                    knockBacked = false;
                }
            }
            //...
        }
    }

}
