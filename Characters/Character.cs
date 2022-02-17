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
        protected bool knockBacked = false;
        protected float knockBackDistance = 0;
        public Character()
        {

        }

        public void onDamage(float damage, float x, float y)
        {
            health -= damage;
            xKnockBack = x;
            yKnockBack = y;
            knockBacked = true;
            knockBackDistance = 40;
            SoundReference loadedSound = TileEngine.Instance.Sound.LoadSound("Sound/testSound.mp3");
            TileEngine.Instance.Sound.PlaySound(loadedSound);
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            var time = (float)(delta.ElapsedGameTime.TotalSeconds);
            var movement = InputHandler.GetEvent("Movement");
            this.Position = new Vector2(Position.X + (movement.Value.X*time*this.speed), Position.Y + (movement.Value.Y*time*this.speed));
            
            var weapons = Scene.GameObjects.Where(x => x is Weapon).ToList();
            foreach (Weapon weapon in weapons)
            {
                if (Scene.GridToTileLocation(weapon.Position) == Scene.GridToTileLocation(this.Position))
                {
                    // This means, we have a weapon that we can pick up.
                    weapon.PickedUp();
                }
            }
        }

        public void PickUpWeapon(string weaponPath)
        {
            //...

        }
    }
}
