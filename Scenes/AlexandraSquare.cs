using CampusCrawl.Characters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using tileEngine.SDK.Audio;
using tileEngine.SDK.GUI;
using tileEngine.SDK.GUI.Elements;
using tileEngine.SDK.Utility;

namespace CampusCrawl.Scenes
{
    public class AlexandraSquare : BaseScene
    {
        Timer spawnCooldown;
        Player player;
        Boolean timed = false;
        Label waveInfo;
        public override void Initialize()
        {
            base.Initialize();
            SoundReference loadedSound = TileEngine.Instance.Sound.LoadSound("Sound/birdSound.mp3");
            SoundInstance soundInstance = TileEngine.Instance.Sound.PlaySound(loadedSound);
            soundInstance.Looping = true;
            soundInstance.Volume = 0.3f;
            spawnCooldown = new Timer(10f);
            spawnCooldown.OnTick += spawnReset;
            spawnCooldown.Loop = true;
            spawnCooldown.Start();
            waveInfo = new Label();
            waveInfo.FontSize = 32;
            waveInfo.Colour = Color.Black;
            waveInfo.Anchor = UIAnchor.Right | UIAnchor.Top;
            UI.AddElement(waveInfo);
            //...
        }

        public void spawnReset()
        {
            if (player != null && timed)
            {
                player.respawn();
                spawnEnemy(1, new int[] { -10, 69 }, new int[] { -8, 6 }, player.Layer, 0);
            }
        }

        public void waveReset()
        {
            if (player.health < 100)
                if (player.health > 70)
                    player.health = 100;
                else
                    player.health += 30;
            spawnEnemy(waveCounter*4, new int[] { -10, 69 }, new int[] { -8, 6 }, player.Layer,0);
            spawnEnemy(waveCounter, new int[] { -10, 69 }, new int[] { -8, 6 }, player.Layer, 1);
            //make it so it spawns different enemy types at higher waves - need different enemy types
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            if (player == null)
            {
                player = (Player)this.GameObjects.Where(x => x is Player).FirstOrDefault();
            }
            var enemies = this.GameObjects.Where(x => x is Enemy);
            waveInfo.Text = "Wave " + waveCounter.ToString() + "   Enemies left: " + enemies.Count().ToString();
            if(this.GameObjects.Where(x => x is Enemy).FirstOrDefault() == null && TileEngine.Instance.GetScene() != null)
            {
                waveCounter++;
                waveReset();
            }
            spawnCooldown.Update(delta);
            //...
        }
    }
}
