using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using tileEngine.SDK.Components;
using tileEngine.SDK.Diagnostics;

namespace CampusCrawl.Entities
{
    internal class Weapon : GameObject
    {
        private BoxColliderComponent collider;
        private SpriteComponent sprite;
        private string assetPath;
        public Weapon(string assetPath)
        {
            collider = new BoxColliderComponent()
            {
                Size = new Vector2(31, 31),
                Location = new Vector2(0, 0)
            };
            AddComponent(collider);
            sprite = new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(assetPath),
                Position = new Vector2(0, 0),
                Scale = new Vector2(1, 1)
            };
            AddComponent(sprite);
        }

        public void SetLocation(int x, int y)
        {
            Position = Scene.TileToGridLocation(new Point(x, y));
        }

        public string PickedUp()
        {
            RemoveComponent(sprite);
            RemoveComponent(collider);

            return assetPath;
        }

        public void SetRandomLocation()
        {
            Random rnd = new Random();
            var everything = Scene.GameObjects.ToList();
            int foundX = -1;
            int foundY = -1;

            while (foundX == -1 || foundY == -1)
            {
                // TODO: Set this next value to a place on the map
                foundX = rnd.Next(1, 100);
                foundY = rnd.Next(1, 100);

                Vector2 foundVector = new Vector2(foundX, foundY);
                foreach (GameObject obj in everything)
                {
                    if (obj.Position.Equals(Position))
                    {
                        foundX = -1;
                        foundY = -1;
                        continue;
                    }
                }

                DiagnosticsHook.DebugMessage($"({foundX}, {foundY})");
                //SetLocation(foundX, foundY);
            }
        }

        public void Initialize()
        {
            //SetRandomLocation();
            
        }
    }
}
