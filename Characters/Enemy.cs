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
        private Dictionary<Point, node> map;
        private List<node> openNodes;
        private List<node> closedNodes;
        private List<node> completedPath;
        private TimeSpan timePath;
        private int state = 0;
        private int followingPath = 0;


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
        }

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

        private int funCalc(int value)
        {
            while (true)
            {
                if (value < 0)
                    value += 10;
                else
                    break;
            }
            while(true)
            {
                if (value > 10)
                    value -= 10;
                else
                    break;
            }
            return value;
        }
        private int calculateLength(Point destinationPoint, Point startPoint)
        {
            var destination = new int[2];
            destination[0] = destinationPoint.Y;
            destination[1] = destinationPoint.X;
            var start = new int[2];
            start[0] = startPoint.Y;
            start[1] = startPoint.X;
            var total = 0;
            while (start[0] != destination[0] || start[1] != destination[1])
            {
                if (start[0] < destination[0] && start[1] < destination[1])
                {
                    total += 14;
                    start[0] += 1;
                    start[1] += 1;
                }
                else if (start[0] > destination[0] && start[1] > destination[1])
                {
                    total += 14;
                    start[0] -= 1;
                    start[1] -= 1;
                }
                else if (start[0] < destination[0])
                {
                    start[0] += 1;
                    total += 10;
                }
                else if (start[0] > destination[0])
                {
                    start[0] -= 1;
                    total += 10;
                }
                else if (start[1] < destination[1])
                {
                    start[1] += 1;
                    total += 10;
                }
                else if (start[1] > destination[1])
                {
                    start[1] -= 1;
                    total += 10;
                }
            }
            return total;
        }

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
                    if (checkPosition(relativeLocation) == 1)
                    {
                        if (map.ContainsKey(relativeLocation) == false)
                        {
                            if (!closedNodes.Contains(smallestNode))
                            {
                                map.Add(relativeLocation, (new node(smallestNode.DistanceFromStart + 10, calculateLength(destination, relativeLocation), 0, relativeLocation, path)));
                                openNodes.Add(map[relativeLocation]);
                            }
                        }
                        else
                        {
                            if (map[relativeLocation].DistanceFromStart > smallestNode.DistanceFromStart + 10)
                            {
                                map[relativeLocation].newStartDistance(smallestNode.DistanceFromStart + 10,path);
                            }
                            if (map[relativeLocation].Target == 1)
                            {
                                break;
                            }
                        }
                    }
                    relativeLocation = new Point(smallestNode.RelativeLocation.X -1 , smallestNode.RelativeLocation.Y );
                    if (checkPosition(relativeLocation) == 1)
                    {
                        if (map.ContainsKey(relativeLocation) == false)
                        {
                            if (!closedNodes.Contains(smallestNode))
                            {
                                map.Add(relativeLocation, (new node(smallestNode.DistanceFromStart + 10, calculateLength(destination, relativeLocation), 0, relativeLocation, path)));
                                openNodes.Add(map[relativeLocation]);
                            }
                        }
                        else
                        {
                            if (map[relativeLocation].DistanceFromStart > smallestNode.DistanceFromStart + 10)
                            {
                                map[relativeLocation].newStartDistance(smallestNode.DistanceFromStart + 10, path);
                            }
                            if (map[relativeLocation].Target == 1)
                            {
                                break;
                            }
                        }
                    }
                    relativeLocation = new Point(smallestNode.RelativeLocation.X, smallestNode.RelativeLocation.Y + 1);
                    if (checkPosition(relativeLocation) == 1)
                    {
                        if (map.ContainsKey(relativeLocation) == false)
                        {
                            if (!closedNodes.Contains(smallestNode))
                            {
                                map.Add(relativeLocation, (new node(smallestNode.DistanceFromStart + 10, calculateLength(destination, relativeLocation), 0, relativeLocation, path)));
                                openNodes.Add(map[relativeLocation]);
                            }
                        }
                        else
                        {
                            if (map[relativeLocation].DistanceFromStart > smallestNode.DistanceFromStart + 10)
                            {
                                map[relativeLocation].newStartDistance(smallestNode.DistanceFromStart + 10, path);
                            }
                            if (map[relativeLocation].Target == 1)
                            {
                                break;
                            }
                        }
                    }
                    relativeLocation = new Point(smallestNode.RelativeLocation.X, smallestNode.RelativeLocation.Y - 1);
                    if (checkPosition(relativeLocation) == 1)
                    {
                        if (map.ContainsKey(relativeLocation) == false)
                        {
                            if (!closedNodes.Contains(smallestNode))
                            {
                                map.Add(relativeLocation, (new node(smallestNode.DistanceFromStart + 10, calculateLength(destination, relativeLocation), 0, relativeLocation, path)));
                                openNodes.Add(map[relativeLocation]);
                            }
                        }
                        else
                        {
                            if (map[relativeLocation].DistanceFromStart > smallestNode.DistanceFromStart + 10)
                            {
                                map[relativeLocation].newStartDistance(smallestNode.DistanceFromStart + 10, path);
                            }
                            if (map[relativeLocation].Target == 1)
                            {
                                break;
                            }
                        }
                    }
                    relativeLocation = new Point(smallestNode.RelativeLocation.X + 1, smallestNode.RelativeLocation.Y + 1);
                    if (checkPosition(relativeLocation) == 1)
                    {
                        if (map.ContainsKey(relativeLocation) == false)
                        {
                            if (!closedNodes.Contains(smallestNode))
                            {
                                map.Add(relativeLocation, (new node(smallestNode.DistanceFromStart + 14, calculateLength(destination, relativeLocation), 0, relativeLocation, path)));
                                openNodes.Add(map[relativeLocation]);
                            }
                        }
                        else
                        {
                            if (map[relativeLocation].DistanceFromStart > smallestNode.DistanceFromStart + 14)
                            {
                                map[relativeLocation].newStartDistance(smallestNode.DistanceFromStart + 14, path);
                            }
                            if (map[relativeLocation].Target == 1)
                            {
                                break;
                            }
                        }
                    }
                    relativeLocation = new Point(smallestNode.RelativeLocation.X -1 , smallestNode.RelativeLocation.Y - 1);
                    if (checkPosition(relativeLocation) == 1)
                    {
                        if (map.ContainsKey(relativeLocation) == false)
                        {
                            if (!closedNodes.Contains(smallestNode))
                            {
                                map.Add(relativeLocation, (new node(smallestNode.DistanceFromStart + 14, calculateLength(destination, relativeLocation), 0, relativeLocation, path)));
                                openNodes.Add(map[relativeLocation]);
                            }
                        }
                        else
                        {
                            if (map[relativeLocation].DistanceFromStart > smallestNode.DistanceFromStart + 14)
                            {
                                map[relativeLocation].newStartDistance(smallestNode.DistanceFromStart + 14, path);
                            }
                            if (map[relativeLocation].Target == 1)
                            {
                                break;
                            }
                        }
                    }
                    relativeLocation = new Point(smallestNode.RelativeLocation.X + 1, smallestNode.RelativeLocation.Y - 1);
                    if (checkPosition(relativeLocation) == 1)
                    {
                        if (map.ContainsKey(relativeLocation) == false)
                        {
                            if (!closedNodes.Contains(smallestNode))
                            {
                                map.Add(relativeLocation, (new node(smallestNode.DistanceFromStart + 14, calculateLength(destination, relativeLocation), 0, relativeLocation, path)));
                                openNodes.Add(map[relativeLocation]);
                            }
                        }
                        else
                        {
                            if (map[relativeLocation].DistanceFromStart > smallestNode.DistanceFromStart + 14)
                            {
                                map[relativeLocation].newStartDistance(smallestNode.DistanceFromStart + 14, path);
                            }
                            if (map[relativeLocation].Target == 1)
                            {
                                break;
                            }
                        }
                    }
                    relativeLocation = new Point(smallestNode.RelativeLocation.X - 1, smallestNode.RelativeLocation.Y + 1);
                    if (checkPosition(relativeLocation) == 1)
                    {
                        if (map.ContainsKey(relativeLocation) == false)
                        {
                            if (!closedNodes.Contains(smallestNode))
                            {
                                map.Add(relativeLocation, (new node(smallestNode.DistanceFromStart + 14, calculateLength(destination, relativeLocation), 0, relativeLocation, path)));
                                openNodes.Add(map[relativeLocation]);
                            }
                        }
                        else
                        {
                            if (map[relativeLocation].DistanceFromStart > smallestNode.DistanceFromStart + 14)
                            {
                                map[relativeLocation].newStartDistance(smallestNode.DistanceFromStart + 14, path);
                            }
                            if (map[relativeLocation].Target == 1)
                            {
                                break;
                            }
                        }
                    }
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
                for (int x = 0; x < test.Path.Count; x++)
                {
                    DiagnosticsHook.DebugMessage($"({test.Path[x].RelativeLocation.X}, {test.Path[x].RelativeLocation.Y})");
                }
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

        public override void Update(GameTime delta)
        {
            base.Update(delta);
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
                else
                {
                    var player = this.Scene.GameObjects.Where(x => x is Character).FirstOrDefault();
                    var playerTile = this.Scene.GridToTileLocation(player.Position);
                    var enemyTile = this.Scene.GridToTileLocation(this.Position);
                    newPath(playerTile, enemyTile);
                    followingPath = 1;
                }
            }
            else
            {
                TimeSpan currentTime = DateTime.UtcNow - new DateTime(1, 1, 1);
                if (currentTime.TotalSeconds - timePath.TotalSeconds > 1)
                {
                    var player = this.Scene.GameObjects.Where(x => x is Character).FirstOrDefault();
                    var playerTile = this.Scene.GridToTileLocation(player.Position);
                    var enemyTile = this.Scene.GridToTileLocation(this.Position);
                    newPath(playerTile, enemyTile);
                }
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
                    //DiagnosticsHook.DebugMessage($"({xValue}, {yValue})");
                    Position = new Vector2(Position.X + (xValue * time * this.speed), Position.Y + (yValue * time * this.speed));
                }
                //followingPath = 0;
            }
        }
    }
}
