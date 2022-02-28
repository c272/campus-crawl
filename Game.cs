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
using tileEngine.SDK.Input;
using tileEngine.SDK.GUI;
using tileEngine.SDK.GUI.Elements;

namespace CampusCrawl
{
   public class Game : TileEngineGame
   {
        private Player mainPlayer;
        public override void Initialize()
        {
            //Create movement bindings.
            TileEngine.Instance.MouseInput.AddBinding(MouseInputType.Position, "MousePosition");
            TileEngine.Instance.KeyboardInput.AddAxisBinding(Keys.D, Keys.A, Keys.S, Keys.W, "Movement");
            mainPlayer = new Player();
            var entity = new Entities.Entity("Assets/TestModel.png");
            TileEngine.Instance.SetScene(typeof(AlexandraSquare));
            TileEngine.Instance.GetScene().AddObject(mainPlayer);
            TileEngine.Instance.GetScene().AddObject(entity);
            TileEngine.Instance.GetScene().CameraPosition = new Vector2(-320, -320);
            mainPlayer.SetLayer("Objects");
            UI.Initialize("Fonts/MakanHati-vmp94.ttf");
            BaseScene scene = (BaseScene)TileEngine.Instance.GetScene();
            mainPlayer.spawnRandomWeapon();
        }

        public override void Shutdown()
        {
            //...
        }
    }
}

