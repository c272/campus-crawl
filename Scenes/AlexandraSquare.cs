using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using tileEngine.SDK.Audio;

namespace CampusCrawl.Scenes
{
    public class AlexandraSquare : BaseScene
    {
        public override void Initialize()
        {
            base.Initialize();
            SoundReference loadedSound = TileEngine.Instance.Sound.LoadSound("Sound/birdSound.mp3");
            SoundInstance soundInstance = TileEngine.Instance.Sound.PlaySound(loadedSound);
            soundInstance.Looping = true;
            soundInstance.Volume = 0.3f;
            //...
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            //...
        }
    }
}
