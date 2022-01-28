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

namespace CampusCrawl.Characters
{
    internal class Enemy : GameObject
    {
        private BoxColliderComponent collider;
        private SpriteComponent sprite;
        private float gridSize = 32;
        private float speed = 100;
        private float direction = -1;
        private float patrolDistance = 5;
        private float currentDistance = 0;
        private string directionName;
        public Enemy(string directionName,float distance)
        {
            if (directionName == "up" ||  directionName == "left")
                this.direction = -1;
            if (directionName == "down" || directionName == "right")
                this.direction = 1;
            this.directionName = directionName;
            this.patrolDistance = distance;
            collider = new BoxColliderComponent()
            {
                Size = new Vector2(gridSize, gridSize),
                Location = new Vector2(0, 0)
            };
            Components.Add(collider);
            sprite = new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(-1280955819),
                Position = new Vector2(0, 0),
                Scale = new Vector2(1, 1)
            };
            Components.Add(sprite);
            this.Position = new Vector2(320, 0);
        }

        private int checkPosition(Point point)
        {
            if(this.Scene.Map.Layers.Where(x => x.ID == this.Layer).FirstOrDefault().CollisionHull.ContainsKey(point))
            {
                DiagnosticsHook.DebugMessage("ab");
                return 0;
            }
            return 1;
        }

        private Vector2 newPosition(float time)
        {
            if (this.directionName == "left" || this.directionName == "right")
            {
                return new Vector2(Position.X + (this.direction * time * this.speed), Position.Y + (0 * time * this.speed));
            }
            return new Vector2(Position.X + (0 * time * this.speed), Position.Y + (this.direction * time * this.speed));
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            var time = (float)(delta.ElapsedGameTime.TotalSeconds);
            var newPos = newPosition(time);
            if (this.Scene.GridToTileLocation(newPos) != this.Scene.GridToTileLocation(this.Position))
            {
                this.currentDistance++;
                if (checkPosition(this.Scene.GridToTileLocation(newPos)) == 0)
                {
                    this.direction = -this.direction;
                    this.currentDistance = 0;
                }
            }
            if(currentDistance == patrolDistance)
            {
                direction = -direction;
                currentDistance = 0;
            }
            this.Position = newPosition(time);
        }
    }
}
