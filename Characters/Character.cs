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
using tileEngine.SDK.Components;
using tileEngine.SDK.Input;

namespace CampusCrawl.Characters
{
    public class Character : GameObject
    {
        private BoxColliderComponent collider;
        private SpriteComponent sprite;
        private float speed = 110;
        public Character()
        {
            UpdateSprite(new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(1215427970),
                Position = new Vector2(0, 0),
                Scale = new Vector2(1, 1)
            }, new BoxColliderComponent()
            {
                Size = new Vector2(31, 31),
                Location = new Vector2(0, 0)
            });
        }

        public void UpdateSprite(SpriteComponent _sprite, BoxColliderComponent boxCollider=null)
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
