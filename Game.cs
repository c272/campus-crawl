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
            TileEngine.Instance.SetScene(typeof(AlexandraSquare));

            var mainPlayer = new Character();
            // TODO: TileEngine.Instance.GetScene().AddObject(mainPlayer);
            // TODO: Latch onto scene change event to call that ^
        }

        public override void Shutdown()
        {
            //...
        }
    }
}

