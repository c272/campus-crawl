using CampusCrawl.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using tileEngine.SDK.Audio;
using tileEngine.SDK.Components;
using tileEngine.SDK.Input;

namespace CampusCrawl.Characters
{
    public class Character : GameObject
    {
        protected BoxColliderComponent collider;
        protected SpriteComponent sprite;
        protected float speed;
        protected float health;
        protected float xKnockBack = 0;
        protected float yKnockBack = 0;
        public bool knockBacked = false;
        protected float knockBackDistance = 0;
        SoundReference damageSound;

        Weapon weapon;

        public Character()
        {

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
            damageSound = TileEngine.Instance.Sound.LoadSound("Sound/testSound.mp3");
        }

        public void onDamage(float damage, float x, float y)
        {
            health -= damage;
            xKnockBack = x;
            yKnockBack = y;
            knockBacked = true;
            knockBackDistance = 40;

            TileEngine.Instance.Sound.PlaySound(damageSound);
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);

            var entities = Scene.GameObjects.Where(x => x is Entity).ToList();
            foreach (Entity entity in entities)
            {
                if (Scene.GridToTileLocation(entity.Position) == Scene.GridToTileLocation(this.Position))
                {
                    // This means, we have a weapon that we can pick up.
                    entity.PickedUp();
                    if (entity is Weapon)
                    {
                        weapon = (Weapon)entity;
                    }
                }
            }
        }
    }
}
