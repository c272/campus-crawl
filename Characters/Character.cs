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
        public bool IsPushed() { return !(X == 0 && Y == 0); }
        public void Reset() { X = 0; Y = 0; currX = 1; currY = 1; }
        public void SetPush(int x, int y)
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

        public void DoneXPush()
        {
            if (currX < 0) { currX--; } else { currX++; }
        }
        public void DoneYPush()
        {
            if (currY < 0) { currY--; } else { currY++; }
        }
        public void CheckPush()
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
        public SpriteComponent sprite;
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
            soundDone.OnTick += SoundCooldown;
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
            damageSound = TileEngine.Instance.Sound.LoadSound("Sound/hit.wav");
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
            if (weapon != null)
            {
                doNotPickUp = weapon;
                weapon.PutDown();
                weapon = null;
            }
        }

        public float[] mouseDirection()
        {
            var direction = new float[2] { 0, 0 };
            var mousePos = InputHandler.GetEvent("MousePosition");
            var gridPos = Scene.ToGridLocation(mousePos.Value);
            var mouseTile = Scene.GridToTileLocation(gridPos);
            var currentTile = Scene.GridToTileLocation(Position);
            if (currentTile.X - mouseTile.X > 0)
                direction[0] = 1;
            if (currentTile.X - mouseTile.X < 0)
                direction[0] = -1;
            if (currentTile.Y - mouseTile.Y > 0)
                direction[1] = 1;
            if (currentTile.Y - mouseTile.Y < 0)
                direction[1] = -1;
            return direction;
        }


        public void PushSelf(int x, int y)
        {
            pushStats.SetPush(x, y);
        }

        public void SoundCooldown()
        {
            soundPlaying = false;
        }

        public void OnDamage(float damage, float[] attackDirection, int pushAmt)
        {
            health -= damage;
            PushSelf(-(int)(attackDirection[0] * pushAmt), -(int)(attackDirection[1] * pushAmt));
            if (!soundPlaying)
            {
                SoundInstance soundInstance = TileEngine.Instance.Sound.PlaySound(damageSound);
                soundInstance.Volume = 0.3f;
                soundInstance.Looping = false;
                soundDone.Start();
                soundPlaying = true;
            }
        }

        public void ScanAndPickUpEntities()
        {
            var entities = Scene.GameObjects.Where(x => x is Entity).ToList();
            foreach (Entity entity in entities)
            {
                if (Scene.GridToTileLocation(entity.Position) == Scene.GridToTileLocation(this.Position) && entity != doNotPickUp)
                {
                    // This means, we have an entity that we can pick up.
                    entity.Initialise(Scene, Layer);
                    if (entity is Weapon)
                    {
                        weapon = (Weapon)entity;
                        weapon.SetCharacter(this);
                    }
                    entity.PickedUp();
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
                ScanAndPickUpEntities();

            // Code to move/push
            pushStats.CheckPush();
            if (pushStats.IsPushed())
            {
                PushEffect(time);
            } else
            {
                attacking = false;
            }
        }

        public void PushEffect(float time)
        {
            pushStats.CheckPush();
            if (pushStats.IsPushed() == true)
            {
                float xPushAmt = 0;
                float yPushAmt = 0;
                if (pushStats.X != 0)
                {
                    xPushAmt = pushStats.currX * time * speed;
                    pushStats.DoneXPush();
                }
                if (pushStats.Y != 0)
                {
                    yPushAmt = pushStats.currY * time * speed;
                    pushStats.DoneYPush();
                }
                if (attacking)
                {
                    if (!weapon.CheckAttack(new Vector2(Position.X + xPushAmt, Position.Y + yPushAmt),!isEnemy))
                    {
                        attacking = false;
                        pushStats.Reset();
                    }
                }
                Position = new Vector2(Position.X + xPushAmt, Position.Y + yPushAmt);
            }
        }
    }
}
