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
    internal class Entity : GameObject
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

        public void SetLocation(int x, int y)
        {
            Position = Scene.TileToGridLocation(new Point(x, y));
        }

        public void PickedUp()
        {
            RemoveComponent(sprite);
            RemoveComponent(collider);
        }

        public void Spawn(Vector2 playerPosition)
        {
            DiagnosticsHook.DebugMessage("Starting spawn");
            DiagnosticsHook.DebugMessage(Scene.ToString());
            Point playerLocation = Scene.GridToTileLocation(playerPosition);
            Random rnd = new Random();
            int foundX = -1;
            int foundY = -1;

            DiagnosticsHook.DebugMessage("Passed first set of variables");

            while (foundX == -1 || foundY == -1)
            {
                DiagnosticsHook.DebugMessage("In while loop");
                // TODO: Set this next value to a place on the map
                foundX = rnd.Next(playerLocation.X - 10, playerLocation.X + 10);
                foundY = rnd.Next(playerLocation.Y - 10, playerLocation.Y + 10);
                DiagnosticsHook.DebugMessage("Got random");
                Point foundPoint = new Point(foundX, foundY);
                DiagnosticsHook.DebugMessage("Got point");
                foreach (GameObject obj in Scene.GameObjects.ToList())
                {
                    DiagnosticsHook.DebugMessage("Looking at each object");
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

                DiagnosticsHook.DebugMessage($"({foundX}, {foundY})");
                SetLocation(foundX, foundY);
            }
        }
    }
}
