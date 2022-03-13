using CampusCrawl.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using tileEngine.SDK;
using tileEngine.SDK.Audio;
using tileEngine.SDK.Components;
using tileEngine.SDK.Diagnostics;
using tileEngine.SDK.Input;
using tileEngine.SDK.Utility;

namespace CampusCrawl.Characters
{
    public struct Push
    {
        public Push(int x, int y)
        {
            X = x;
            Y = y;
            currX = 1;
            currY = 1;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int currX { get; set; }
        public int currY { get; set; }
        public bool isPushed() { return !(X == 0 && Y == 0); }
        public void reset() { X = 0; Y = 0; currX = 1; currY = 1; }
        public void setPush(int x, int y)
        {
            X = x;
            Y = y;
            if (x < 0)
            {
                currX = -1;
            }
            if (y < 0)
            {
                currY = -1;
            }
        }

        public void doneXPush()
        {
            if (currX < 0) { currX--; } else { currX++; }
        }
        public void doneYPush()
        {
            if (currY < 0) { currY--; } else { currY++; }
        }
        public void checkPush()
        {
            if (Math.Abs(currX) == Math.Abs(X))
            {
                currX = 1;
                X = 0;
            }
            if (Math.Abs(currY) == Math.Abs(Y))
            {
                currY = 1;
                Y = 0;
            }
        }
    }
    public class Character : GameObject
    {
        protected bool followingPath = false;
        protected BoxColliderComponent collider;
        protected SpriteComponent sprite;
        protected float speed;
        public float health;
        public bool attacking;
        public Push pushStats = new Push(0, 0);
        public float[] attackDirection;
        public float damage;
        protected SoundReference damageSound;
        protected bool isEnemy = false;
        protected Weapon weapon;
        protected Entity doNotPickUp;
        public string playerModelPath;
        protected bool soundPlaying = false;
        protected Timer soundDone;

        public Character()
        {
            soundDone = new Timer(0.25f);
            soundDone.OnTick += soundCooldown;
            soundDone.Loop = false;
        }

        public void UpdateSprite(SpriteComponent _sprite, BoxColliderComponent boxCollider = null)
        {
            if (sprite != null)
                RemoveComponent(sprite);

            if (boxCollider != null)
            {
                if (collider != null)
                    RemoveComponent(collider);
                collider = boxCollider;
                AddComponent(boxCollider);
            }

            sprite = _sprite;
            AddComponent(_sprite);
        }

        public override void Initialize()
        {
            base.Initialize();
            attacking = false;
            soundDone.Start();
            damageSound = TileEngine.Instance.Sound.LoadSound("Sound/testSound.mp3");
        }

        public void CreateAndSetWeapon(Weapon weapon)
        {
            weapon.SetCharacter(this);
            weapon.Spawn(Position);

            weapon.PickedUp();
            this.weapon = weapon;
        }

        public void DropCurrentWeapon()
        {
            doNotPickUp = weapon;
            weapon.PutDown();
        }

        public float[] mouseDirection()
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


        public void PushSelf(int x, int y)
        {
            pushStats.setPush(x, y);
        }

        public void soundCooldown()
        {
            soundPlaying = false;
        }

        public void onDamage(float damage, float[] attackDirection, int pushAmt)
        {
            health -= damage;
            PushSelf(-(int)(attackDirection[0] * pushAmt), -(int)(attackDirection[1] * pushAmt));
            if (!soundPlaying)
            {
                TileEngine.Instance.Sound.PlaySound(damageSound);
                soundDone.Start();
                soundPlaying = true;
            }
        }

        public void scanAndPickUpEntities()
        {
            var entities = Scene.GameObjects.Where(x => x is Entity).ToList();
            foreach (Entity entity in entities)
            {
                if (Scene.GridToTileLocation(entity.Position) == Scene.GridToTileLocation(this.Position) && entity != doNotPickUp)
                {
                    // This means, we have an entity that we can pick up.
                    entity.PickedUp();
                    entity.Initialise(Scene, Layer);

                    if (entity is Weapon)
                    {
                        weapon = (Weapon)entity;
                        weapon.SetCharacter(this);
                    }
                }
            }
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            var time = (float)(delta.ElapsedGameTime.TotalSeconds);
            var movement = InputHandler.GetEvent("Movement");
            soundDone.Update(delta);
            // Code to pick up entities
            if (weapon == null)
                scanAndPickUpEntities();

            // Code to move/push
            pushStats.checkPush();
            if (pushStats.isPushed())
            {
                pushEffect(time);
            } else
            {
                attacking = false;
            }
        }

        public void pushEffect(float time)
        {
            pushStats.checkPush();
            if (pushStats.isPushed() == true)
            {
                float xPushAmt = 0;
                float yPushAmt = 0;
                if (pushStats.X != 0)
                {
                    xPushAmt = pushStats.currX * time * speed;
                    pushStats.doneXPush();
                }
                if (pushStats.Y != 0)
                {
                    yPushAmt = pushStats.currY * time * speed;
                    pushStats.doneYPush();
                }
                if (attacking)
                {
                    if (!weapon.checkAttack(new Vector2(Position.X + xPushAmt, Position.Y + yPushAmt),!isEnemy))
                    {
                        attacking = false;
                        pushStats.reset();
                    }
                }
                Position = new Vector2(Position.X + xPushAmt, Position.Y + yPushAmt);
            }
        }
    }
}
