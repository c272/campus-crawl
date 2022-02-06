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
using tileEngine.SDK.Utility;

namespace CampusCrawl.Characters
{
    public class Enemy : GameObject
    {
        private BoxColliderComponent collider;
        private SpriteComponent sprite;
        private float gridSize = 32;
        private float speed = 100;
        private float direction = -1;
        private float patrolDistance = 5;
        private float currentDistance = 0;
        private string directionName;
        private Dictionary<Point, node> map;
        private List<node> openNodes;
        private List<node> closedNodes;
        private List<node> completedPath;
        private TimeSpan timePath;
        private int followingPath = 0;
        private Timer timerPath;


        public struct node
        {
            public node(int distanceFromStart, int distanceFromEnd, int target, Point relativeLocation,List<node> path)
            {
                DistanceFromStart = distanceFromStart;
                DistanceFromEnd = distanceFromEnd;
                OverallCost = DistanceFromStart + DistanceFromEnd;
                Path = path;
                Target = target;
                RelativeLocation = relativeLocation;
            }

            public void newStartDistance(int distanceFromStart, List<node> path)
            {
                DistanceFromStart = distanceFromStart;
                OverallCost = DistanceFromStart + DistanceFromEnd;
                Path = path;
            }
            public int DistanceFromStart { get; set; }
            public int DistanceFromEnd { get; }
            public int OverallCost { get; set; }
            public List<node> Path { get; set; }
            public int Target { get; }
            public Point RelativeLocation { get; }
        }




        public Enemy(string directionName, float distance)
        {
            if (directionName == "up" || directionName == "left")
                this.direction = -1;
            if (directionName == "down" || directionName == "right")
                this.direction = 1;
            this.directionName = directionName;
            this.patrolDistance = distance;
            collider = new BoxColliderComponent()
            {
                Size = new Vector2(gridSize-1, gridSize-1),
                Location = new Vector2(0, 0)
            };
            AddComponent(collider);
            sprite = new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(-1280955819),
                Position = new Vector2(0, 0),
                Scale = new Vector2(1, 1)
            };
            AddComponent(sprite);
            this.Position = new Vector2(320, 0);
            timerPath = new Timer((float)0.5);
            timerPath.OnTick += pathCreation;
            timerPath.Loop = true;
        }
        /*
         * Checks if a tile has collision at set point
         */
        private int checkPosition(Point point)
        {
            if (this.Scene.Map.Layers.Where(x => x.ID == this.Layer).FirstOrDefault().CollisionHull.ContainsKey(point))
            {
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
        /*
         * calculates the relative distance between two points
         */
        private int calculateLength(Point destination, Point start)
        {
            var total = 0;
            while (start.X != destination.X || start.Y != destination.Y)
            {
                if (start.X < destination.X && start.Y < destination.Y)
                {
                    total += 14;
                    start.X += 1;
                    start.Y += 1;
                }
                else if (start.X > destination.X && start.Y > destination.Y)
                {
                    total += 14;
                    start.X -= 1;
                    start.Y -= 1;
                }
                else if (start.X < destination.X)
                {
                    start.X += 1;
                    total += 10;
                }
                else if (start.X > destination.X)
                {
                    start.X -= 1;
                    total += 10;
                }
                else if (start.Y < destination.Y)
                {
                    start.Y += 1;
                    total += 10;
                }
                else if (start.Y > destination.Y)
                {
                    start.Y -= 1;
                    total += 10;
                }
            }
            return total;
        }
        /*
         * finds the node in open nodes with the smallest OverallCost
         */
        private node findSmallestNode()
        {
            int cheapest = 0;
            node cheap = new node();
            for(int x =0;x< openNodes.Count;x++)
            {
                if (openNodes[x].OverallCost < cheapest || cheapest == 0)
                {
                    cheapest = openNodes[x].OverallCost;
                    cheap = openNodes[x];
                }
            }
            return cheap;
        }

        private int addMapping(Point relativeLocation, int value, List<node> path, node smallestNode)
        {
            if (map[relativeLocation].DistanceFromStart > smallestNode.DistanceFromStart + value)
            {
                map[relativeLocation].newStartDistance(smallestNode.DistanceFromStart + value, path);
            }
            if (map[relativeLocation].Target == 1)
            {
                return 1;
            }
            return 0;
        }

        private int checkNode(Point relativeLocation,node smallestNode,Point destination,List<node> path,int value)
        {
            if (checkPosition(relativeLocation) == 1)
            {
                if (map.ContainsKey(relativeLocation) == false)
                {
                    if (!closedNodes.Contains(smallestNode))
                    {
                        map.Add(relativeLocation, (new node(smallestNode.DistanceFromStart + value, calculateLength(destination, relativeLocation), 0, relativeLocation, path)));
                        openNodes.Add(map[relativeLocation]);
                    }
                }
                else
                {
                    if (addMapping(relativeLocation, value, path, smallestNode) == 1) return 1;
                }
            }
            return 0;
        }

        private node findPath(Point destination)
        {
            node smallestNode = new node();
            while (true)
            {
                smallestNode = findSmallestNode();
                if (smallestNode.Target != 1)
                {
                    smallestNode.Path.Add(smallestNode);
                    List<node> path = new List<node>(smallestNode.Path);
                    var relativeLocation = new Point(smallestNode.RelativeLocation.X +1, smallestNode.RelativeLocation.Y);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 10) == 1) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X -1 , smallestNode.RelativeLocation.Y );
                    if (checkNode(relativeLocation, smallestNode, destination, path, 10) == 1) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X, smallestNode.RelativeLocation.Y + 1);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 10) == 1) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X, smallestNode.RelativeLocation.Y - 1);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 10) == 1) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X + 1, smallestNode.RelativeLocation.Y + 1);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 14) == 1) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X -1 , smallestNode.RelativeLocation.Y - 1);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 14) == 1) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X + 1, smallestNode.RelativeLocation.Y - 1);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 14) == 1) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X - 1, smallestNode.RelativeLocation.Y + 1);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 14) == 1) break;
                }
                openNodes.Remove(smallestNode);
                closedNodes.Add(smallestNode);
            }
            smallestNode.Path.Add(map[destination]);
            return smallestNode;
        }

        private void newPath(Point destination, Point currentTile)
        {
            if (!destination.Equals(currentTile))
            {
                map = new Dictionary<Point, node>();

                map.Add(destination, new node(1000, 0, 1, destination, new List<node> { }));
                map.Add(currentTile, new node(0, calculateLength(currentTile, destination), 0, currentTile, new List<node> { }));
                openNodes = new List<node> { };
                closedNodes = new List<node> { };
                openNodes.Add(map[currentTile]);
                node test = findPath(destination);
                completedPath = new List<node>(test.Path);
                timePath = DateTime.UtcNow - new DateTime(1, 1, 1);
            }
            else
            {
                var path = new node(1000, 0, 1, destination, new List<node> { });
                path.Path.Add(path);
                completedPath = new List<node>(path.Path);
                timePath = DateTime.UtcNow - new DateTime(1, 1, 1);

            }
        }

        private int playerInView()
        {
            var player = this.Scene.GameObjects.Where(x => x is Character).FirstOrDefault();
            var playerTile = this.Scene.GridToTileLocation(player.Position);
            var currentTile = this.Scene.GridToTileLocation(this.Position);
            if(Math.Abs(playerTile.X - currentTile.X) < 10 && Math.Abs(playerTile.Y - currentTile.Y) < 10)
            {
                return 1; 
            }
            return 0;
        }

        private void pathCreation()
        {
            var player = this.Scene.GameObjects.Where(x => x is Character).FirstOrDefault();
            var playerTile = this.Scene.GridToTileLocation(player.Position);
            var enemyTile = this.Scene.GridToTileLocation(this.Position);
            if (playerInView() == 1)
            {
                newPath(playerTile, enemyTile);
                followingPath = 1;
            }
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            timerPath.Update(delta);
            timerPath.Start();
            var time = (float)(delta.ElapsedGameTime.TotalSeconds);
            var newPos = newPosition(time);
            if (followingPath == 0)
            {
                if (playerInView() == 0)
                {
                    if (this.Scene.GridToTileLocation(newPos) != this.Scene.GridToTileLocation(this.Position))
                    {
                        this.currentDistance++;
                        if (checkPosition(this.Scene.GridToTileLocation(newPos)) == 0)
                        {
                            this.direction = -this.direction;
                            this.currentDistance = 0;
                        }
                    }
                    if (currentDistance == patrolDistance)
                    {
                        direction = -direction;
                        currentDistance = 0;
                    }
                    this.Position = newPosition(time);
                }
            }
            else
            {
                Point current = this.Scene.GridToTileLocation(this.Position);
                Point target = new Point(completedPath[0].RelativeLocation.X, completedPath[0].RelativeLocation.Y);
                int xValue = 0;
                int yValue = 0;
                if (current.Equals(target))
                {
                    if(completedPath.Count == 1)
                    {
                        followingPath = 0;
                    }
                    else
                    {
                        completedPath.RemoveAt(0);
                        target = new Point(completedPath[0].RelativeLocation.X, completedPath[0].RelativeLocation.Y);
                    }
                }
                if (followingPath == 1)
                {
                    if (current.X > target.X)
                    {
                        xValue = -1;
                    }
                    if (current.X < target.X)
                    {
                        xValue = 1;
                    }
                    if (current.Y > target.Y)
                    {
                        yValue = -1;
                    }
                    if (current.Y < target.Y)
                    {
                        yValue = 1;
                    }
                    Position = new Vector2(Position.X + (xValue * time * this.speed), Position.Y + (yValue * time * this.speed));
                }
            }
        }
    }
}
