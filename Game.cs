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
        Character mainPlayer;
        public override void Initialize()
        {
            mainPlayer = new Character();
            TileEngine.Instance.SetScene(typeof(AlexandraSquare));
            TileEngine.Instance.GetScene().AddObject(mainPlayer);
            mainPlayer.SetLayer("Objects");/*
            TileEngine.Instance.KeyboardInput.AddBinding(Keys.W, "Up",false,false,moveUp);
            TileEngine.Instance.KeyboardInput.AddBinding(Keys.S, "Down",false,false,moveDown);
            TileEngine.Instance.KeyboardInput.AddBinding(Keys.A, "Left",false, false, moveLeft);
            TileEngine.Instance.KeyboardInput.AddBinding(Keys.D, "Right",false, false, moveRight);
            // TODO: TileEngine.Instance.GetScene().AddObject(mainPlayer);*/
            // TODO: Latch onto scene change event to call that ^
        }

        private void moveLeft()
        {
            mainPlayer.Position = mainPlayer.Position + new Vector2(-1, 0);
        }
        private void moveRight()
        {
            mainPlayer.Position = mainPlayer.Position + new Vector2(1, 0);
        }
        private void moveUp()
        {
            mainPlayer.Position = mainPlayer.Position + new Vector2(0, -1);
        }
        private void moveDown()
        {
            mainPlayer.Position = mainPlayer.Position + new Vector2(0, 1);
        }

        public override void Shutdown()
        {
            //...
        }
    }
}

