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

namespace CampusCrawl
{
   public class Game : TileEngineGame
   {
        public override void Initialize()
        {
            var mainPlayer = new Character();
            TileEngine.Instance.SetScene(typeof(AlexandraSquare));
            TileEngine.Instance.GetScene().AddObject(mainPlayer);
            mainPlayer.SetLayer("Objects");
        }
        public override void Shutdown()
        {
            //...
        }
    }
}

