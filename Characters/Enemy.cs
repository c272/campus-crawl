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
        private float speed = 100;
        private float direction = -1;
        private float patrolDistance = 5;
        private float currentDistance = 0;
        private string directionName;
        private Dictionary<Point, Node> map;
        private List<Node> openNodes;
        private List<Node> closedNodes;
        private List<Node> completedPath;
        private Boolean followingPath = false;
        private Timer timerPath;


        public struct Node
        {
            public Node(int distanceFromStart, int distanceFromEnd, int target, Point relativeLocation, List<Node> path)
            {
                DistanceFromStart = distanceFromStart;
                DistanceFromEnd = distanceFromEnd;
                OverallCost = DistanceFromStart + DistanceFromEnd;
                Path = path;
                Target = target;
                RelativeLocation = relativeLocation;
            }

            public void newStartDistance(int distanceFromStart, List<Node> path)
            {
                DistanceFromStart = distanceFromStart;
                OverallCost = DistanceFromStart + DistanceFromEnd;
                Path = path;
            }
            public int DistanceFromStart { get; set; }
            public int DistanceFromEnd { get; }
            public int OverallCost { get; set; }
            public List<Node> Path { get; set; }
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
            this.Position = new Vector2(320, 0);
            sprite = new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(-1280955819),
                Position = new Vector2(0, 0),
                Scale = new Vector2(1, 1)
            };
            AddComponent(sprite);
            timerPath = new Timer(0.5f);
            timerPath.OnTick += pathCreation;
            timerPath.Loop = true;
        }

        public override void Initialize()
        {
            base.Initialize();
            collider = new BoxColliderComponent()
            {
                Size = new Vector2(Scene.Map.TileTextureSize - 1, Scene.Map.TileTextureSize - 1),
                Location = new Vector2(0, 0)
            };
            AddComponent(collider);
            timerPath.Start();
        }
        /*
         * Checks if a tile has collision at set point
         */
        private Boolean checkPosition(Point point)
        {
            if (this.Scene.Map.Layers.Where(x => x.ID == this.Layer).FirstOrDefault().CollisionHull.ContainsKey(point))
            {
                return false;
            }
            return true;
        }

        private Vector2 newPosition(float delta)
        {
            if (this.directionName == "left" || this.directionName == "right")
            {
                return new Vector2(Position.X + (this.direction * delta * this.speed), Position.Y + (0 * delta * this.speed));
            }
            return new Vector2(Position.X + (0 * delta * this.speed), Position.Y + (this.direction * delta * this.speed));
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
        private Node findSmallestNode()
        {
            int cheapest = 0;
            Node cheap = new Node();
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
        /*
         * checks if this is the new fastest way to the node and then returns if the node is the target node
         */
        private Boolean targetLocation(Point relativeLocation, int value, List<Node> path, Node smallestNode)
        {
            if (map[relativeLocation].DistanceFromStart > smallestNode.DistanceFromStart + value)
            {
                map[relativeLocation].newStartDistance(smallestNode.DistanceFromStart + value, path);
            }
            if (map[relativeLocation].Target == 1)
            {
                return true;
            }
            return false;
        }

        private Boolean checkNode(Point relativeLocation,Node smallestNode,Point destination,List<Node> path,int value)
        {
            if (checkPosition(relativeLocation))
            {
                if (!map.ContainsKey(relativeLocation))
                {
                    if (!closedNodes.Contains(smallestNode))
                    {
                        map.Add(relativeLocation, (new Node(smallestNode.DistanceFromStart + value, calculateLength(destination, relativeLocation), 0, relativeLocation, path)));
                        openNodes.Add(map[relativeLocation]);
                    }
                }
                else
                {
                    if (targetLocation(relativeLocation, value, path, smallestNode)) return true;
                }
            }
            return false;
        }

        private Node findPath(Point destination)
        {
            Node smallestNode = new Node();
            while (true)
            {
                smallestNode = findSmallestNode();
                if (smallestNode.Target != 1)
                {
                    smallestNode.Path.Add(smallestNode);
                    List<Node> path = new List<Node>(smallestNode.Path);
                    var relativeLocation = new Point(smallestNode.RelativeLocation.X +1, smallestNode.RelativeLocation.Y);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 10)) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X -1 , smallestNode.RelativeLocation.Y );
                    if (checkNode(relativeLocation, smallestNode, destination, path, 10)) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X, smallestNode.RelativeLocation.Y + 1);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 10)) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X, smallestNode.RelativeLocation.Y - 1);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 10)) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X + 1, smallestNode.RelativeLocation.Y + 1);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 14)) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X -1 , smallestNode.RelativeLocation.Y - 1);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 14)) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X + 1, smallestNode.RelativeLocation.Y - 1);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 14)) break;
                    relativeLocation = new Point(smallestNode.RelativeLocation.X - 1, smallestNode.RelativeLocation.Y + 1);
                    if (checkNode(relativeLocation, smallestNode, destination, path, 14)) break;
                }
                openNodes.Remove(smallestNode);
                if (openNodes.Count == 0 || closedNodes.Count >= 1000) break;
                closedNodes.Add(smallestNode);
            }
            smallestNode.Path.Add(map[destination]);
            return smallestNode;
        }

        private void newPath(Point destination, Point currentTile)
        {
            if (!destination.Equals(currentTile))
            {
                map = new Dictionary<Point, Node>();

                map.Add(destination, new Node(int.MaxValue, 0, 1, destination, new List<Node> { })); //It is assigned max value so that onces its found it will be assigned a path
                map.Add(currentTile, new Node(0, calculateLength(currentTile, destination), 0, currentTile, new List<Node> { }));
                openNodes = new List<Node> { };
                closedNodes = new List<Node> { };
                openNodes.Add(map[currentTile]);
                Node target = findPath(destination);
                completedPath = new List<Node>(target.Path);
            }
            else
            {
                var path = new Node(1000, 0, 1, destination, new List<Node> { });
                path.Path.Add(path);
                completedPath = new List<Node>(path.Path);
            }
        }

        private Boolean playerInView()
        {
            var player = this.Scene.GameObjects.Where(x => x is Character).FirstOrDefault();
            var playerTile = this.Scene.GridToTileLocation(player.Position);
            var currentTile = this.Scene.GridToTileLocation(this.Position);
            if(Math.Abs(playerTile.X - currentTile.X) < 10 && Math.Abs(playerTile.Y - currentTile.Y) < 10)
            {
                return true; 
            }
            return false;
        }

        private void pathCreation()
        {
            var player = this.Scene.GameObjects.Where(x => x is Character).FirstOrDefault();
            var playerTile = this.Scene.GridToTileLocation(player.Position);
            var enemyTile = this.Scene.GridToTileLocation(this.Position);
            if (playerInView())
            {
                newPath(playerTile, enemyTile);
                followingPath = true;
            }
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            timerPath.Update(delta);
            var time = (float)(delta.ElapsedGameTime.TotalSeconds);
            var newPos = newPosition(time);
            if (!followingPath)
            {
                if (!playerInView())
                {
                    if (this.Scene.GridToTileLocation(newPos) != this.Scene.GridToTileLocation(this.Position))
                    {
                        this.currentDistance++;
                        if (!checkPosition(this.Scene.GridToTileLocation(newPos)))
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
                        followingPath = false;
                    }
                    else
                    {
                        completedPath.RemoveAt(0);
                        target = new Point(completedPath[0].RelativeLocation.X, completedPath[0].RelativeLocation.Y);
                    }
                }
                if (followingPath)
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
