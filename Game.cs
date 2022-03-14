﻿using Microsoft.Xna.Framework;
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
using CampusCrawl.Entities.Weapons;
using Microsoft.Xna.Framework.Graphics;

namespace CampusCrawl
{
   public class Game : TileEngineGame
   {
        private Player mainPlayer;
        private RectangleButton startButton;
        private Picture guiImage;
        private Label title;
        public void startGame(Point location)
        {
            SoundInstance soundInstance = TileEngine.Instance.Sound.PlaySound(TileEngine.Instance.Sound.LoadSound("Sound/click.mp3"));
            soundInstance.Volume = 0.3f;
            soundInstance.Looping = false;
            UI.RemoveElement(startButton);
            UI.RemoveElement(guiImage);
            UI.RemoveElement(title);
            mainPlayer = new Player();
            var entity = new Entities.Entity("Assets/TestModel.png");
            TileEngine.Instance.SetScene(typeof(AlexandraSquare));
            TileEngine.Instance.GetScene().AddObject(mainPlayer);
            TileEngine.Instance.GetScene().AddObject(entity);
            TileEngine.Instance.GetScene().CameraPosition = new Vector2(-320, -320);
            mainPlayer.SetLayer("Objects");
            BaseScene scene = (BaseScene)TileEngine.Instance.GetScene();
            mainPlayer.CreateAndSetWeapon(new Fists());
        }
        public override void Initialize()
        {
            //Create movement bindings.
            TileEngine.Instance.MouseInput.AddBinding(MouseInputType.Position, "MousePosition");
            TileEngine.Instance.KeyboardInput.AddAxisBinding(Keys.D, Keys.A, Keys.S, Keys.W, "Movement");
            UI.Initialize("Fonts/MakanHati-vmp94.ttf");
            Texture2D logo = AssetManager.AttemptLoad<Texture2D>("logo.png");
            guiImage = new Picture();
            guiImage.Texture = logo;
            guiImage.Anchor = UIAnchor.Top;
            guiImage.Offset = new Vector2(0, 100);
            guiImage.Scale = 0.4f;
            title = new Label();
            title.Text = "CAMPUS CRAWL";
            title.FontSize = 32;
            title.Anchor = UIAnchor.Top;
            title.Offset = new Vector2(0, 220);
            startButton = new RectangleButton();
            startButton.OnClick += startGame;
            startButton.Anchor = UIAnchor.Center;
            startButton.BorderColour = Color.Black;
            startButton.BackgroundColour = Color.Green;
            //startButton.Offset = new Vector2(0,80);
            Label label = new Label();
            label.Text = "Start game";
            label.FontSize = 32;
            label.Colour = Color.Black;
            startButton.Label = label;
            UI.AddElement(title);
            UI.AddElement(startButton);
            UI.AddElement(guiImage);
            TileEngine.Instance.SetScene(typeof(Spar));
        }


        public override void Shutdown()
        {
            //...
        }
    }
}

