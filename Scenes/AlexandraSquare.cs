﻿using CampusCrawl.Characters;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tileEngine.SDK;
using tileEngine.SDK.Attributes;
using tileEngine.SDK.Audio;
using tileEngine.SDK.Diagnostics;
using tileEngine.SDK.GUI;
using tileEngine.SDK.GUI.Elements;
using tileEngine.SDK.Map;
using tileEngine.SDK.Utility;

namespace CampusCrawl.Scenes
{
    public class AlexandraSquare : BaseScene
    {
        Timer spawnCooldown;
        Player player;
        Boolean timed = false;
        Label waveInfo;
        int timeStart = -1;
        public override void Initialize()
        {
            base.Initialize();

            //Load background sound.
            SoundReference loadedSound = TileEngine.Instance.Sound.LoadSound("Sound/birdSound.mp3");
            SoundInstance soundInstance = TileEngine.Instance.Sound.PlaySound(loadedSound);
            soundInstance.Looping = true;
            soundInstance.Volume = 0.3f;

            //Set up the spawn cooldown.
            spawnCooldown = new Timer(10f);
            spawnCooldown.OnTick += SpawnReset;
            spawnCooldown.Loop = true;
            spawnCooldown.Start();

            //Add wave information for the scene.
            waveInfo = new Label();
            waveInfo.FontSize = 32;
            waveInfo.Colour = Color.Black;
            waveInfo.Anchor = UIAnchor.Right | UIAnchor.Top;
            timeStart = (int)(DateTime.UtcNow - new DateTime(1000, 1, 1)).TotalSeconds;
            UI.AddElement(waveInfo);
        }

        public void SpawnReset()
        {
            timeStart = (int)(DateTime.UtcNow - new DateTime(1000, 1, 1)).TotalSeconds;
            if (player != null && timed)
            {
                player.Respawn();
            }
        }

        public void WaveReset()
        {
            if(waveCounter != 1)
                TileEngine.Instance.Sound.PlaySound(TileEngine.Instance.Sound.LoadSound("Sound/waveDone.mp3"));
            int timeElapsed = timeStart - (int)(DateTime.UtcNow - new DateTime(1000, 1, 1)).TotalSeconds;
            if(((waveCounter * 30) - timeElapsed) * 5 > 0 && waveCounter != 1)
            {
                player.Score += ((waveCounter * 60) - timeElapsed) *5;
            }
            if (player.health < 100)
                if (player.health > 70)
                    player.health = 100;
                else
                    player.health += 30;
            SpawnEnemy(waveCounter *2, new int[] { -10, 69 }, new int[] { -8, 6 }, player.Layer,0);
            SpawnEnemy((int)waveCounter /2, new int[] { -10, 69 }, new int[] { -8, 6 }, player.Layer, 1);
            SpawnEnemy((int)waveCounter/3, new int[] { -10, 69 }, new int[] { -8, 6 }, player.Layer, 2);
            timeStart = (int)(DateTime.UtcNow - new DateTime(1000, 1, 1)).TotalSeconds;
        }

        public override void Update(GameTime delta)
        {
            base.Update(delta);
            if (!paused)
            {
                if (player == null)
                    player = (Player)GameObjects.Where(x => x is Player).FirstOrDefault();

                var enemies = this.GameObjects.Where(x => x is Enemy);
                waveInfo.Text = "Wave " + waveCounter.ToString() + "   Enemies left: " + enemies.Count().ToString();
                if (this.GameObjects.Where(x => x is Enemy).FirstOrDefault() == null && TileEngine.Instance.GetScene() != null)
                {
                    waveCounter++;
                    WaveReset();
                }
                spawnCooldown.Update(delta);
            }
        }

        [EventFunction("OnShelteredArea")]
        public void OnShelteredArea(TileEventData e)
        {
            TileLayer roofLayer = Map.Layers.Where(x => x.Name == "Roof").FirstOrDefault();
            if (roofLayer != null)
            {
                roofLayer.Opacity = (float)0.5;
            }   
        }

        [EventFunction("ExitedShelteredArea")]
        public void ExitedShelteredArea(TileEventData e)
        {
            TileLayer roofLayer = Map.Layers.Where(x => x.Name == "Roof").FirstOrDefault();
            if (roofLayer != null)
            {
                roofLayer.Opacity = 1;
            }
        }
    }
}
