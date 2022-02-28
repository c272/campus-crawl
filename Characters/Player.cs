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
        private Timer attackCooldown;
        private bool canAttack = true;
        private float attackX = 0f;
        private float attackY = 0f;
        private float[] attackDirection = new float[2];
        public bool attacking = false;
        private float attackDistance = 0f;
        MouseInputHandler mouse;

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
            Fists e = new Fists(this, "Assets/TestModel.png");
            e.Scene = Scene;
            e.SetLayer(Layer);
            e.Spawn(Position);
        }

        public void respawn()
        {
            Position = Scene.TileToGridLocation(spawnPoint);
            health = 100;
            //knockBacked = false;
            pushStats.reset();
        }


        public void cooldown()
        {
            canAttack = true;
        }

        private void handlePrimaryAttack(MouseState mouseState)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
                weapon.Attack();
        }

        private int rightButtonHeld = 0;
        private bool rightButtonReleased = false;
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
                }
            }
            else if (rightButtonReleased)
            {
                if (weapon is Fists)
                {
                    ((Fists)weapon).Lunge(clamp(rightButtonHeld / 10, 0, 20));
                }
                rightButtonHeld = 0;
                rightButtonReleased = false;
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

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            var time = (float)(delta.ElapsedGameTime.TotalSeconds);
            var movement = InputHandler.GetEvent("Movement");
            var mouseState = Mouse.GetState();


            if(health <= 0)
            {
                respawn();
            } else
            {
                handleAttack(mouseState);
            }

            Scene.LookAt(Position);
        }
    }

}
