using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using CampusCrawl.Characters;
using tileEngine.SDK.Components;
using CampusCrawl.Entities.Weapons;

namespace CampusCrawl.Scenes
{
    public abstract class BaseScene : Scene
    {
        //Static random to avoid re-seeding unnecessarily.
        private static Random random = new Random();

        public int waveCounter = 0;
        public Boolean paused = false;
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
        }

        public void SpawnEnemy(int amount, Point rangeX, Point rangeY, int layer,int enemyType)
        {
            if (amount != 0)
            {
                string[] directions = new string[] { "up", "down", "left", "right" };
                List<Point> spawnableTiles = new List<Point>();
                for (int x = rangeX.X; x < rangeX.Y; x++)
                {
                    for (int y = rangeY.X; y < rangeY.Y; y++)
                    {
                        if (!Map.Layers.Where(a => a.ID == layer).FirstOrDefault().CollisionHull.ContainsKey(new Point(x, y)))
                        {
                            spawnableTiles.Add(new Point(x, y));
                        }
                    }
                }

                for (int x = 0; x < amount; x++)
                {
                    var index = random.Next(spawnableTiles.Count);
                    var enemyPos = spawnableTiles[index];
                    spawnableTiles.RemoveAt(index);
                    var newEnemy = new Enemy(directions[random.Next(directions.Count())], random.Next(30), TileToGridLocation(enemyPos));
                    if (enemyType == 1)
                        newEnemy = new EnemyTank(directions[random.Next(directions.Count())], random.Next(10), TileToGridLocation(enemyPos));
                    if (enemyType == 2)
                        newEnemy = new EnemySprint(directions[random.Next(directions.Count())], random.Next(30), TileToGridLocation(enemyPos));
                    TileEngine.Instance.GetScene().AddObject(newEnemy);
                    newEnemy.SetLayer("Objects");
                    newEnemy.CreateAndSetWeapon(new Fists());
                }
            }
        }

    }
}
