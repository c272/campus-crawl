using CampusCrawl.Entities;
using CampusCrawl.Entities.Weapons;
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
    public class Enemy : Character
    {
        protected float direction = -1;
        protected float patrolDistance = 5;
        protected float currentDistance = 0;
        protected string directionName;
        protected Dictionary<Point, Node> map;
        protected List<Node> openNodes;
        protected List<Node> closedNodes;
        protected List<Node> completedPath;
        protected Timer timerPath;
        protected Timer attackCooldown;
        protected Timer heavyAttackCooldown;
        protected Player player;
        protected int scoreValue = 100;
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

        public Enemy(string directionName, float distance, Vector2 location)
        {
            playerModelPath = "FinalAssets/Zombie.png";
            isEnemy = true;
            if (directionName == "up" || directionName == "left")
                direction = -1;
            if (directionName == "down" || directionName == "right")
                direction = 1;
            this.directionName = directionName;
            patrolDistance = distance;
            Position = location;
            sprite = new SpriteComponent()
            {
                Texture = AssetManager.AttemptLoad<Texture2D>(playerModelPath),
                Position = new Vector2(-16, -16),
                Scale = new Vector2(1, 1)
            };
            AddComponent(sprite);
            heavyAttackCooldown = new Timer(5f);
            heavyAttackCooldown.OnTick += heavyAttack;
            heavyAttackCooldown.Loop = true;
            attackCooldown = new Timer(0.5f);
            attackCooldown.OnTick += attack;
            attackCooldown.Loop = true;
            timerPath = new Timer(0.5f);
            timerPath.OnTick += createPath;
            timerPath.Loop = true;
            speed = 100;
            health = 100;
            damage = 10;

        }

        public override void Initialize()
        {
            base.Initialize();
            collider = new BoxColliderComponent()
            {
                Size = new Vector2(Scene.Map.TileTextureSize - 1, Scene.Map.TileTextureSize - 1),
                Location = new Vector2(-15.5f,-15.5f)
            };
            AddComponent(collider);
            timerPath.Start();
            attackCooldown.Start();
            heavyAttackCooldown.Start();
        }
        /*
         * Checks if a tile has collision at set point
         */
        private bool checkPosition(Point point)
        {
            if (Scene.Map.Layers.Where(x => x.ID == Layer).FirstOrDefault().CollisionHull.ContainsKey(point))
            {
                return false;
            }
            return true;
        }

        private Vector2 newPosition(float delta)
        {
            if (directionName == "left" || directionName == "right")
            {
                return new Vector2(Position.X + (direction * delta * speed), Position.Y + (0 * delta * speed));
            }
            return new Vector2(Position.X + (0 * delta * speed), Position.Y + (direction * delta * speed));
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
        private bool targetLocation(Point relativeLocation, int value, List<Node> path, Node smallestNode)
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

        private bool checkNode(Point relativeLocation,Node smallestNode,Point destination,List<Node> path,int value)
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

        private bool playerInView(int distance, bool flag)
        {
            Point playerTile = Scene.GridToTileLocation(player.Position);
            Point currentTile = Scene.GridToTileLocation(Position);
            if (flag)
            {
                playerTile = new Point((int)player.Position.X, (int)player.Position.Y);
                currentTile = new Point((int)Position.X, (int)Position.Y);
            }
            if (Math.Abs(playerTile.X - currentTile.X) < distance && Math.Abs(playerTile.Y - currentTile.Y) < distance)
            {
                return true;
            }
            return false;
        }

        private void createPath()
        {
            var playerTile = Scene.GridToTileLocation(player.Position);
            var enemyTile = Scene.GridToTileLocation(Position);
            if (playerInView(15,false))
            {
                newPath(playerTile, enemyTile);
                followingPath = true;
            }
        }

        public float[] playerDirection()
        {
            var direction = new float[2] { 0, 0 };
            var playerTile = Scene.GridToTileLocation(player.Position);
            var currentTile = Scene.GridToTileLocation(Position);
            if (currentTile.X - playerTile.X > 0)
            {
                direction[0] = 1;
            }
            if (currentTile.X - playerTile.X < 0)
            {
                direction[0] = -1;
            }
            if (currentTile.Y - playerTile.Y > 0)
            {
                direction[1] = 1;
            }
            if (currentTile.Y - playerTile.Y < 0)
            {
                direction[1] = -1;
            }
            return direction;
        }

        private void attack()
        {
            if (playerInView(37,true) && player.pushStats.isPushed() == false && player.attacking == false && !attacking)
            {
                //attacking = true;
                if (weapon != null)
                {
                    attackDirection = playerDirection();
                    weapon.Attack(false, false);
                    //((Fists)weapon).Lunge(1, false);
                }
            }
        }

        private void heavyAttack()
        {
           
            if (playerInView(120, true) && player.pushStats.isPushed() == false && player.attacking == false)
            {
                if (weapon != null)
                {
                    attacking = true;
                    float[] prepareDirection = playerDirection();
                    float xMovement = -prepareDirection[0];
                    float yMovement = -prepareDirection[1];
                    Position = new Vector2(Position.X + xMovement, Position.Y + yMovement);
                    attackDirection = prepareDirection;
                    ((Fists)weapon).Lunge(0.04f, false);
                    //heavyAttackCooldown.Reset();
                   
                }
            }
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            if (health <= 0)
            {
                Scene = null;
                player.score += scoreValue;
            }
            else
            {
                if (player == null)
                {
                    player = (Player)Scene.GameObjects.Where(x => x is Player).FirstOrDefault();
                }
                if(player!=null)
                {
                    Vector2 deltaPos = new Vector2((player.Position.X - Position.X), -(player.Position.Y - Position.Y));
                    float angleA = sprite.Rotation;
                    if (deltaPos.Y >= 0)
                    {
                        angleA = (float)Math.Atan(deltaPos.X / deltaPos.Y);
                    }
                    else
                    {
                        angleA = (float)(Math.Atan(deltaPos.X / deltaPos.Y) + Math.PI);
                    }

                    sprite.Rotation = angleA;
                }
                if (!pushStats.isPushed() && attacking)
                {
                    attacking = false;
                }
                heavyAttackCooldown.Update(delta);
                timerPath.Update(delta);
                attackCooldown.Update(delta);
                var time = (float)(delta.ElapsedGameTime.TotalSeconds);
                var newPos = newPosition(time);
                if (!attacking)
                {
                    if (!followingPath)
                    {
                        if (!playerInView(15, false))
                        {
                            if (Scene.GridToTileLocation(newPos) != Scene.GridToTileLocation(Position))
                            {
                                currentDistance++;
                                if (!checkPosition(Scene.GridToTileLocation(newPos)))
                                {
                                    direction = -direction;
                                    currentDistance = 0;
                                }
                            }
                            if (currentDistance == patrolDistance)
                            {
                                direction = -direction;
                                currentDistance = 0;
                            }
                            Position = newPosition(time);
                            doNotPickUp = null;
                        }
                    }
                    else
                    {
                        Point current = Scene.GridToTileLocation(Position);
                        Point target = new Point(completedPath[0].RelativeLocation.X, completedPath[0].RelativeLocation.Y);
                        int xValue = 0;
                        int yValue = 0;
                        if (current.Equals(target))
                        {
                            if (completedPath.Count == 1)
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
                                xValue = -1;
                            if (current.X < target.X)
                                xValue = 1;
                            if (current.Y > target.Y)
                                yValue = -1;
                            if (current.Y < target.Y)
                                yValue = 1;
                            Position = new Vector2(Position.X + (xValue * time * speed), Position.Y + (yValue * time * speed));
                            doNotPickUp = null;
                        }
                    }
                }
            }
        }
    }
}
