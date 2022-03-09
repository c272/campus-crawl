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
        public float score = 0f;
        public Label scoreCount;
        private RectangleButton restartButton;
        private Boolean paused = false;
        private Panel pausePanel;
        public Label scoreLabel;
        public Player()
        {
            playerModelPath = "Assets/TestModel.png";
            sprite = new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(playerModelPath),
                Position = new Vector2(-16, -16),
                Scale = new Vector2(1, 1)
            };
            AddComponent(sprite);
            attackCooldown = new Timer(0.5f);
            attackCooldown.OnTick += cooldown;
            attackCooldown.Loop = true;
            speed = 110;
            health = 100;
            damage = 5;
            healthBar = new ProgressBar(new Vector2(200, 32));
            healthBar.ForegroundColour = Color.Green;
            healthBar.BackgroundColour = Color.Red;
            healthBar.Value = 1;
            healthCount = new Label();
            healthCount.FontSize = 32;
            healthCount.Text = health.ToString() + " / " + 100;
            scoreCount = new Label();
            scoreCount.FontSize = 32;
            scoreCount.Colour = Color.Black;
            scoreCount.Text = "Score: " + score.ToString();
            scoreCount.Anchor = UIAnchor.Top | UIAnchor.Center;
            UI.AddElement(healthBar);
            UI.AddElement(healthCount);
            UI.AddElement(scoreCount);
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
        private float clamp(float val, float min, float max)
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public void spawnRandomWeapon()
        {
            Fists e = new Fists();
            e.SetCharacter(this);
        }

        public void respawn()
        {
            if(oneLife)
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
                makePauseMenu();
            }
            else
            {
                Position = Scene.TileToGridLocation(spawnPoint);
                health = 100;
                attacking = false;
                pushStats.reset();
            }
        }

        public void restart(Point location)
        {
            var scene = (Scenes.BaseScene)Scene;
            scene.paused = false;
            paused = false;
            UI.RemoveElement(restartButton);
            UI.RemoveElement(pausePanel);
            UI.RemoveElement(scoreLabel);
            score = 0;
            Position = Scene.TileToGridLocation(spawnPoint);
            health = 100;
            attacking = false;
            pushStats.reset();
        }

        public void makePauseMenu()
        {
            pausePanel = new Panel();
            pausePanel.Colour = Color.PaleVioletRed;
            pausePanel.Size = new Vector2(500, 250);
            pausePanel.Anchor = UIAnchor.Center;
            pausePanel.OnClick += restart;
            scoreLabel = new Label();
            scoreLabel.Text = "You have been defeated\nYou scored: " + score.ToString();
            scoreLabel.Anchor = UIAnchor.Center;
            scoreLabel.FontSize = 32;
            scoreLabel.Colour = Color.Black;
            scoreLabel.Offset = new Vector2(0, -70);
            restartButton = new RectangleButton();
            restartButton.OnClick += restart;
            restartButton.Anchor = UIAnchor.Center;
            restartButton.BorderColour = Color.Black;
            restartButton.BackgroundColour = Color.Green;
            Label label = new Label();
            label.Text = "Restart game";
            label.FontSize = 32;
            label.Colour = Color.Black;
            restartButton.Label = label;
            UI.AddElement(pausePanel);
            UI.AddElement(restartButton);
            UI.AddElement(scoreLabel);
        }

        public void cooldown()
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
                    Scale = new Vector2(1, 1)
                });
                weapon.Attack(false,true);
            }

            if (mouseState.LeftButton == ButtonState.Released)
            {
                UpdateSprite(new SpriteComponent
                {
                    Texture = AssetManager.AttemptLoad<Texture2D>(weapon.restingModel),
                    Position = new Vector2(-16, -16),
                    Scale = new Vector2(1, 1)
                });
            }
        }

        private int rightButtonHeld = 0;
        private bool rightButtonReleased = false;
        private bool isLunging = false;
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
                        Scale = new Vector2(1, 1)
                    });
                    attacking = true;
                    ((Fists)weapon).Lunge(clamp(rightButtonHeld / 10, 0, 20),true);
                    isLunging = false;
                }
                rightButtonHeld = 0;
                rightButtonReleased = false;
                UpdateSprite(new SpriteComponent
                {
                    Texture = AssetManager.AttemptLoad<Texture2D>(weapon.restingModel),
                    Position = new Vector2(-16, -16),
                    Scale = new Vector2(1, 1)
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
            if(positionNew.Y > 1)
            {
                positionNew.Y = Scene.TileToGridLocation(new Point(pos.X, 1)).Y;
            }
            if (positionNew.Y < -2)
            {
                positionNew.Y = Scene.TileToGridLocation(new Point(pos.X, -2)).Y;
            }
            if (pos.X < 0)
            {
                positionNew.X = Scene.TileToGridLocation(new Point(0, pos.Y)).X;
            }
            if (pos.X > 59)
            {
                positionNew.X = Scene.TileToGridLocation(new Point(60, pos.Y)).X;
            }
            
            return positionNew;
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            if (!paused)
            {
                var time = (float)(delta.ElapsedGameTime.TotalSeconds);
                var movement = InputHandler.GetEvent("Movement");
                var mouseState = Mouse.GetState();
                Vector2 mousePos = Scene.ToGridLocation(mouseState.Position);
                Vector2 deltaPos = new Vector2((mousePos.X - Position.X), -(mousePos.Y - Position.Y));

                //DiagnosticsHook.DebugMessage(deltaPos.ToString());
                float angleA = (float)Math.Atan(deltaPos.X/deltaPos.Y);

                //double personAngle = Math.Atan2(deltaPos.Y, deltaPos.X);
                //double personAngle = Math.Atan(tanAngle);
                //DiagnosticsHook.DebugMessage("Tan angle = " + tanAngle);
                //DiagnosticsHook.DebugMessage("Person angle = " + personAngle);
                //float radianAngle = (float)((Math.PI / 180) * personAngle);
                DiagnosticsHook.DebugMessage("angle = " + angleA);
                sprite.Rotation = (float)3.14;
                

                if (health <= 0)
                {
                    respawn();
                }
                else
                {
                    handleAttack(mouseState);
                }
                if (!pushStats.isPushed() && attacking)
                {
                    attacking = false;
                }

                if (!pushStats.isPushed() && !isLunging)
                {
                    Position = new Vector2(Position.X + (movement.Value.X * time * speed), Position.Y + (movement.Value.Y * time * speed));
                    doNotPickUp = null;
                }

                healthBar.Value = health / 100;
                healthCount.Text = health.ToString() + " / " + 100;
                scoreCount.Text = "Score: " + score.ToString();
                Scene.LookAt(calculateLookAt());
            }
        }
    }
}
