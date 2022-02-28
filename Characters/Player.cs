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
using tileEngine.SDK.GUI.Elements;
using tileEngine.SDK.GUI;

namespace CampusCrawl.Characters
{
    public class Player : Character
    {
        private Point spawnPoint = new Point();
        private Weapon currentWeapon = null;
        private Timer attackCooldown;
        private bool canAttack = true;
        private float attackX = 0f;
        private float attackY = 0f;
        private float[] attackDirection = new float[2];
        public bool attacking = false;
        private float attackDistance = 0f;
        public bool oneLife = true;
        MouseInputHandler mouse;
        public ProgressBar healthBar;
        public Label healthCount;
        public Player()
        {
            sprite = new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(1215427970),
                Position = new Vector2(-16, -16),
                Scale = new Vector2(1, 1)
            };
            AddComponent(sprite);
            attackCooldown = new Timer(0.5f);
            attackCooldown.OnTick += cooldown;
            attackCooldown.Loop = true;
            speed = 110;
            health = 100;
            damage = 20;
            healthBar = new ProgressBar(new Vector2(200, 32));
            healthBar.ForegroundColour = Color.Green;
            healthBar.BackgroundColour = Color.Red;
            healthBar.Value = 1;
            healthCount = new Label();
            healthCount.FontSize = 32;
            healthCount.Text = health.ToString() + " / " + 100;
            UI.AddElement(healthBar);
            UI.AddElement(healthCount);
        }

        public override void Initialize()
        {
            base.Initialize();
            collider = new BoxColliderComponent()
            {
                Size = new Vector2(Scene.Map.TileTextureSize, Scene.Map.TileTextureSize),
                Location = new Vector2(-16, -16)
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
            if(oneLife)
            {
                var enemies = Scene.GameObjects.Where(x => x is Enemy);
                foreach(var enemy in enemies.ToList())
                {
                    enemy.Scene = null;
                }
                var scene = (Scenes.BaseScene)Scene;
                scene.waveCounter = 0;
            }
        }

        private float[] mouseDirection()
        {
            var direction = new float[2] { 0, 0 };
            var mousePos = InputHandler.GetEvent("MousePosition");
            var gridPos = Scene.ToGridLocation(mousePos.Value);
            var mouseTile = Scene.GridToTileLocation(gridPos);
            var currentTile = Scene.GridToTileLocation(Position);
            if (currentTile.X - mouseTile.X > 0)
            {
                direction[0] = 1;
            }
            if (currentTile.X - mouseTile.X < 0)
            {
                direction[0] = -1;
            }
            if (currentTile.Y - mouseTile.Y > 0)
            {
                direction[1] = 1;
            }
            if (currentTile.Y - mouseTile.Y < 0)
            {
                direction[1] = -1;
            }
            return direction;
        }

        public void checkAttack()
        {
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
                    if ((currentTile.X - enemyTile.X == attackDirection[0] && currentTile.Y - enemyTile.Y == attackDirection[1]) || enemyTile == currentTile)
                    {

                        if (Math.Abs(enemyPos.X - currentPos.X) < 37 && Math.Abs(enemyPos.Y - currentPos.Y) < 37) //this is as position is recorded to be top left of character so it means if its just touching tile above then its considered in the tile so this checks that its a little bit into the tile
                        {
                            currentEnemy.onDamage(damage, -attackDirection[0] * 2.5f, -attackDirection[1] * 2.5f);
                        }
                    }
                }
            }
            attacking = false;
            attackX = 0;
            attackY = 0;
            attackDistance = 0;
        }
        public void attack()
        {
            canAttack = false;
            if(currentWeapon != null)
                currentWeapon.Attack();
            attacking = true;
            attackDirection = mouseDirection();
            attackX = -attackDirection[0] * 3;
            attackY = -attackDirection[1] * 3;
            attackDistance = 60;
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
            if (click == 1 && knockBacked == false)
                attack();
            Scene.LookAt(Position);
            if(health <= 0)
            {
                respawn();
            }
            healthBar.Value = health / 100;
            healthCount.Text = health.ToString() + " / " + 100;
            if (!knockBacked)
            {
                if (attacking)
                {
                    var enemies = Scene.GameObjects.Where(x => x is Enemy).ToArray();
                    var hit = false;
                    var tempTile = Scene.GridToTileLocation(new Vector2(Position.X + (attackX * time * speed), Position.Y + (attackY * time * speed)));
                    foreach (GameObject enemy in enemies)
                    {
                        Enemy currentEnemy = (Enemy)enemy;
                        if (Math.Abs(currentEnemy.Position.X - Position.X) <37 && Math.Abs(currentEnemy.Position.Y-  Position.Y) < 37)
                        {
                            currentEnemy.onDamage(damage, -attackDirection[0] * 2.5f, -attackDirection[1] * 2.5f);
                            hit = true;
                            attacking = false;
                            attackX = 0;
                            attackY = 0;
                            attackDistance = 0;
                        }
                    }
                    Position = new Vector2(Position.X + (attackX * time * speed), Position.Y + (attackY * time * speed));
                    if (hit == false)
                    {
                        if (attackX != 0)
                        {
                            attackDistance -= Math.Abs(attackX * time * speed);
                        }
                        else
                        {
                            attackDistance -= Math.Abs(attackY * time * speed);
                        }
                        if (attackDistance <= 0)
                        {
                            checkAttack();
                        }
                    }
                }
                else
                {
                    Position = new Vector2(Position.X + (movement.Value.X * time * speed), Position.Y + (movement.Value.Y * time * speed));
                }
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
