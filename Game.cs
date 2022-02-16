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
using tileEngine.SDK.Audio;

namespace CampusCrawl
{
   public class Game : TileEngineGame
   {
        private Player mainPlayer;
        public override void Initialize()
        {
            //Create movement bindings.
            TileEngine.Instance.KeyboardInput.AddAxisBinding(Keys.D, Keys.A, Keys.S, Keys.W, "Movement");

            mainPlayer = new Player();
            var enemy = new Enemy("left",5,new Vector2(320,0));
            TileEngine.Instance.SetScene(typeof(AlexandraSquare));
            TileEngine.Instance.GetScene().AddObject(mainPlayer);
            TileEngine.Instance.GetScene().AddObject(enemy);
            TileEngine.Instance.GetScene().CameraPosition = new Vector2(-320, -320);
            enemy.SetLayer("Objects");
            mainPlayer.SetLayer("Objects");
            BaseScene scene = (BaseScene)TileEngine.Instance.GetScene();
            scene.spawnEnemy(2, new int[2] { -5, 5 }, new int[2] { -5, 5 },mainPlayer.Layer);
        }

        public override void Shutdown()
        {
            //...
        }
    }
}

