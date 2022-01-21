using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using CampusCrawl.Scenes;

namespace CampusCrawl
{
   public class Game : TileEngineGame
    {
        public override void Initialize()
        {
            TileEngine.Instance.SetScene(typeof(AlexandraSquare));
        }

        public override void Shutdown()
        {
            //...
        }
    }
}

