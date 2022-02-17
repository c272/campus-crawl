using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using CampusCrawl.Scenes;
using CampusCrawl.Characters;
using CampusCrawl.Entities;

namespace CampusCrawl
{
   public class Game : TileEngineGame
   {
        public override void Initialize()
        {
            var mainPlayer = new Character();
            var enemy = new Enemy("left",5);
            //var shank = new Weapon("Assets/TestModel.png");

            TileEngine.Instance.SetScene(typeof(AlexandraSquare));
            TileEngine.Instance.GetScene().AddObject(mainPlayer);
            TileEngine.Instance.GetScene().AddObject(enemy);
            //TileEngine.Instance.GetScene().AddObject(shank);
            TileEngine.Instance.GetScene().CameraPosition = new Vector2(-320, -320);
            enemy.SetLayer("Objects");
            mainPlayer.SetLayer("Objects");

            //shank.Initialize();
        }
        public override void Shutdown()
        {
            //...
        }
    }
}

