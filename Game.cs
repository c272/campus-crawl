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
            // TODO: TileEngine.Instance.GetScene().AddObject(mainPlayer);
            // TODO: Latch onto scene change event to call that ^
        }

        public override void Shutdown()
        {
            //...
        }
    }
}

