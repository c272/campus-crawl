using CampusCrawl.Characters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using tileEngine.SDK.Audio;
using tileEngine.SDK.Utility;

namespace CampusCrawl.Scenes
{
    public class AlexandraSquare : BaseScene
    {
        Timer spawnCooldown;
        Player player;
        Boolean timed = false;
        int waveCounter = 0;
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
            //...
        }

        public void spawnReset()
        {
            if(player != null && timed)
                spawnEnemy(1, new int[] { -10, 69 }, new int[] { -8, 6 },player.Layer);
        }

        public void waveReset()
        {
            spawnEnemy(waveCounter*4, new int[] { -10, 69 }, new int[] { -8, 6 }, player.Layer);
            //make it so it spawns different enemy types at higher waves - need different enemy types
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            if (player == null)
            {
                player = (Player)this.GameObjects.Where(x => x is Player).FirstOrDefault();
            }
            if(this.GameObjects.Where(x => x is Enemy).FirstOrDefault() == null)
            {
                waveCounter++;
                waveReset();
            }
            spawnCooldown.Update(delta);
            //...
        }
    }
}
