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
using tileEngine.SDK.Audio;

namespace CampusCrawl.Characters
{
    public class Player : Character
    {
        private Point spawnPoint = new Point();
        private Timer attackCooldown;
        private bool canAttack = true;
        private float[] attackDirection = new float[2];
        public bool Attacking = false;
        public bool OneLife = true;
        public ProgressBar HealthBar;
        public Label HealthCount;
        public float Score = 0f;
        public Label ScoreCount;
        private RectangleButton restartButton;
        private bool paused = false;
        private Panel pausePanel;
        private Picture logo;
        public Label ScoreLabel;
        private int rightButtonHeld = 0;
        private bool rightButtonReleased = false;
        private bool isLunging = false;

        public Player()
        {
            playerModelPath = "FinalAssets/LancasterGuy.png";
            sprite = new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(playerModelPath),
                Position = new Vector2(-16, -16),
                Scale = new Vector2(1, 1)
            };
            AddComponent(sprite);
            attackCooldown = new Timer(0.1f);
            attackCooldown.OnTick += Cooldown;
            attackCooldown.Loop = true;
            speed = 110;
            health = 100;
            damage = 8;
            HealthBar = new ProgressBar(new Vector2(200, 32));
            HealthBar.ForegroundColour = Color.Green;
            HealthBar.BackgroundColour = Color.Red;
            HealthBar.Value = 1;
            HealthCount = new Label();
            HealthCount.FontSize = 32;
            HealthCount.Text = health.ToString() + " / " + 100;
            ScoreCount = new Label();
            ScoreCount.FontSize = 32;
            ScoreCount.Colour = Color.Black;
            ScoreCount.Text = "Score: " + Score.ToString();
            ScoreCount.Anchor = UIAnchor.Top | UIAnchor.Center;
            UI.AddElement(HealthBar);
            UI.AddElement(HealthCount);
            UI.AddElement(ScoreCount);
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
            attackCooldown.Start();
        }
        private float clamp(float val, float min, float max)
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public void SpawnRandomWeapon()
        {
            Fists e = new Fists();
            e.SetCharacter(this);
        }

        public void Respawn()
        {
            if(OneLife)
            {
                var enemies = Scene.GameObjects.Where(x => x is Enemy);
                foreach(var enemy in enemies.ToList())
                {
                    enemy.Scene = null;
                }
                var scene = (Scenes.BaseScene)Scene;
                scene.waveCounter = 0;
                scene.paused = true;
                paused = true;
                MakePauseMenu();
            }
            else
            {
                Position = Scene.TileToGridLocation(spawnPoint);
                health = 100;
                Attacking = false;
                pushStats.Reset();
            }
        }

        public void Restart(Point location)
        {
            SoundInstance soundInstance = TileEngine.Instance.Sound.PlaySound(TileEngine.Instance.Sound.LoadSound("Sound/click.mp3"));
            soundInstance.Volume = 0.3f;
            soundInstance.Looping = false;
            var scene = (Scenes.BaseScene)Scene;
            scene.paused = false;
            paused = false;
            UI.RemoveElement(restartButton);
            UI.RemoveElement(pausePanel);
            UI.RemoveElement(ScoreLabel);
            UI.RemoveElement(logo);
            Score = 0;
            Position = Scene.TileToGridLocation(spawnPoint);
            health = 100;
            Attacking = false;
            pushStats.Reset();
        }

        public void MakePauseMenu()
        {
            SoundInstance soundInstance = TileEngine.Instance.Sound.PlaySound(TileEngine.Instance.Sound.LoadSound("Sound/playerDeath.wav"));
            soundInstance.Volume = 0.3f;
            soundInstance.Looping = false;
            TileEngine.Instance.Sound.PlaySound(damageSound);
            pausePanel = new Panel();
            pausePanel.Colour = Color.CornflowerBlue;
            pausePanel.Size = new Vector2(500, 250);
            pausePanel.Anchor = UIAnchor.Center;
            pausePanel.OnClick += Restart;
            ScoreLabel = new Label();
            ScoreLabel.Text = "You have been defeated\n    You scored: " + Score.ToString();
            ScoreLabel.Anchor = UIAnchor.Center;
            ScoreLabel.FontSize = 32;
            ScoreLabel.Colour = Color.Black;
            ScoreLabel.Offset = new Vector2(0, -10);
            logo = new Picture();
            logo.Texture = AssetManager.AttemptLoad<Texture2D>("logo.png");
            logo.Anchor = UIAnchor.Center;
            logo.Offset = new Vector2(0, -83);
            logo.Scale = 0.3f;
            restartButton = new RectangleButton();
            restartButton.OnClick += Restart;
            restartButton.Anchor = UIAnchor.Center;
            restartButton.BorderColour = Color.Black;
            restartButton.BackgroundColour = Color.Green;
            restartButton.Offset = new Vector2(0, 50);
            Label label = new Label();
            label.Text = "Restart game";
            label.FontSize = 32;
            label.Colour = Color.Black;
            restartButton.Label = label;
            UI.AddElement(pausePanel);
            UI.AddElement(restartButton);
            UI.AddElement(logo);
            UI.AddElement(ScoreLabel);
        }

        public void Cooldown()
        {
            canAttack = true;
        }

        private void handlePrimaryAttack(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                UpdateSprite(new SpriteComponent
                {
                    Texture = AssetManager.AttemptLoad<Texture2D>(weapon.attackedModel),
                    Position = new Vector2(-16, -16),
                    Scale = new Vector2(1, 1),
                    Rotation = sprite.Rotation
                });
                weapon.Attack(false, true);
            }

            if (mouseState.LeftButton == ButtonState.Released)
            {
                UpdateSprite(new SpriteComponent
                {
                    Texture = AssetManager.AttemptLoad<Texture2D>(weapon.restingModel),
                    Position = new Vector2(-16, -16),
                    Scale = new Vector2(1, 1),
                    Rotation = sprite.Rotation
                });
            }
            canAttack = false;
        }

        private void handleSecondaryAttack(MouseState mouseState)
        {
            if (rightButtonHeld > 0 && mouseState.RightButton == ButtonState.Released)
            {
                rightButtonReleased = true;
            }
            if (mouseState.RightButton == ButtonState.Pressed && rightButtonHeld != -1)
            {
                rightButtonHeld++;
                if (weapon is Fists && rightButtonHeld % 4 == 0)
                {
                    float[] prepareDirection = mouseDirection();
                    float xMovement = prepareDirection[0];
                    float yMovement = prepareDirection[1];

                    Position = new Vector2(Position.X + xMovement, Position.Y + yMovement);
                    isLunging = true;
                }
            }
            else if (rightButtonReleased)
            {
                if (weapon is Fists)
                {
                    UpdateSprite(new SpriteComponent
                    {
                        Texture = AssetManager.AttemptLoad<Texture2D>(weapon.attackedModel),
                        Position = new Vector2(-16, -16),
                        Scale = new Vector2(1, 1),
                        Rotation = sprite.Rotation
                    });
                    Attacking = true;
                    ((Fists)weapon).Lunge(clamp(rightButtonHeld / 10, 0, 20),true);
                    isLunging = false;
                }
                rightButtonHeld = 0;
                rightButtonReleased = false;
                UpdateSprite(new SpriteComponent
                {
                    Texture = AssetManager.AttemptLoad<Texture2D>(weapon.restingModel),
                    Position = new Vector2(-16, -16),
                    Scale = new Vector2(1, 1),
                    Rotation = sprite.Rotation
                });
            }
        }

        private void handleAttack(MouseState mouseState)
        {
            if (weapon != null)
            {
                handlePrimaryAttack(mouseState);
                handleSecondaryAttack(mouseState);
            }
        }

        private Vector2 calculateLookAt()
        {
            Point pos = Scene.GridToTileLocation(Position);
            Vector2 positionNew = Position;
            positionNew.Y = Scene.TileToGridLocation(new Point(pos.X, -1)).Y;
            if (pos.X < 0)
                positionNew.X = Scene.TileToGridLocation(new Point(0, pos.Y)).X;
            if (pos.X > 59) //59 is at 750 resolution the max to the right you can go before seeing blue space
                positionNew.X = Scene.TileToGridLocation(new Point(60, pos.Y)).X;
            
            return positionNew;
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            if (!paused)
            {
                attackCooldown.Update(delta);
                var time = (float)(delta.ElapsedGameTime.TotalSeconds);
                var movement = InputHandler.GetEvent("Movement");
                var mouseState = Mouse.GetState();
                Vector2 mousePos = Scene.ToGridLocation(mouseState.Position);
                Vector2 deltaPos = new Vector2((mousePos.X - Position.X), -(mousePos.Y - Position.Y));
                float angleA = sprite.Rotation;
                if (deltaPos.Y >= 0)
                    angleA = (float)Math.Atan(deltaPos.X / deltaPos.Y);
                else
                    angleA = (float)(Math.Atan(deltaPos.X / deltaPos.Y) + Math.PI);
                
                sprite.Rotation = angleA;
                

                if (health <= 0)
                    Respawn();
                else
                    handleAttack(mouseState);
                if (!pushStats.IsPushed() && Attacking)
                    Attacking = false;

                if (!pushStats.IsPushed() && !isLunging)
                {
                    Position = new Vector2(Position.X + (movement.Value.X * time * speed), Position.Y + (movement.Value.Y * time * speed));
                    doNotPickUp = null;
                }

                HealthBar.Value = health / 100;
                HealthCount.Text = (int)health + " / " + 100;
                ScoreCount.Text = "Score: " + Score.ToString();
                Scene.LookAt(calculateLookAt());
            }
        }
    }
}
