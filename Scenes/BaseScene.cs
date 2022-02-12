﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using CampusCrawl.Characters;
using tileEngine.SDK.Components;

namespace CampusCrawl.Scenes
{
    public class BaseScene : Scene
    {
        public override void Initialize()
        {
            base.Initialize();
            //...
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            //...
        }

        public void spawnEnemy(int amount, int[] rangeX, int[] rangeY, int layer)
        {
            Scene scene = TileEngine.Instance.GetScene();
            List<Point> spawnableTiles = new List<Point>();
            for (int x = rangeX[0]; x < rangeX[1]; x++)
            {
                for (int y = rangeY[0]; y < rangeY[1]; y++)
                {
                    if (!scene.Map.Layers.Where(a => a.ID == layer).FirstOrDefault().CollisionHull.ContainsKey(new Point(x, y)))
                    {
                        spawnableTiles.Add(new Point(x, y));
                    }
                }
            }
            for (int x = 0; x < amount; x++)
            {
                Random random = new Random();
                var index = random.Next(spawnableTiles.Count);
                var enemyPos = spawnableTiles[index];
                spawnableTiles.RemoveAt(index);
                var newEnemy = new Enemy("up", 4, scene.TileToGridLocation(enemyPos));
                TileEngine.Instance.GetScene().AddObject(newEnemy);
                newEnemy.SetLayer("Objects");
            }
        }

    }
}