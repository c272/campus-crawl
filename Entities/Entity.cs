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
using tileEngine.SDK.Map;

namespace CampusCrawl.Entities
{
    public class Entity : GameObject
    {
        private BoxColliderComponent collider;
        private SpriteComponent sprite;
        private string assetPath;

        public Entity(string assetPath)
        {
            sprite = new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(assetPath),
                Position = new Vector2(0, 0),
                Scale = new Vector2(1, 1)
            };
            AddComponent(sprite);
        }

        public void Initialise(Scene scene, int layer)
        {
            Scene = scene;
            SetLayer(layer);
        }

        public void SetLocation(int x, int y)
        {
            Position = Scene.TileToGridLocation(new Point(x, y));
        }

        public virtual void PickedUp()
        {
            RemoveComponent(sprite);
            RemoveComponent(collider);
        }

        public virtual void PutDown()
        {
            AddComponent(sprite);
            AddComponent(collider);
        }

        public void Spawn(Vector2 playerPosition)
        {
            Point playerLocation = Scene.GridToTileLocation(playerPosition);
            Random rnd = new Random();
            int foundX = -1;
            int foundY = -1;

            while (foundX == -1 || foundY == -1)
            {
                // TODO: Set this next value to a place on the map
                foundX = rnd.Next(playerLocation.X - 3, playerLocation.X + 3); //note was 10 but made it 3 so it spawns close
                foundY = rnd.Next(playerLocation.Y - 3, playerLocation.Y + 3);
                Point foundPoint = new Point(foundX, foundY);
                foreach (GameObject obj in Scene.GameObjects.ToList())
                {
                    if (obj.Position.Equals(foundPoint))
                    {
                        foundX = -1;
                        foundY = -1;
                        continue;
                    }

                }
                TileLayer layer = Scene.Map.Layers.Where(x => x.ID == Layer).FirstOrDefault();
                if (layer.CollisionHull?.ContainsKey(foundPoint) == true)
                {
                    foundX = -1;
                    foundY = -1;
                    continue;
                }

                SetLocation(foundX, foundY);
            }
        }
    }
}
