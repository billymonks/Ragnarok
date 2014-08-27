using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.display._3d;

namespace wickedcrush.manager.audio
{
    public class SoundManager
    {
        private Dictionary<String, SoundEffect> soundsList;
        private List<SoundEffectInstance> loopingSoundsList;

        private ContentManager _cm;
        private Camera _gameCam;

        public AudioListener listener;

        public SoundManager(ContentManager cm)
        {
            soundsList = new Dictionary<String, SoundEffect>();
            loopingSoundsList = new List<SoundEffectInstance>();

            SoundEffect.DistanceScale = 140f;

            _cm = cm;
        }

        public void Update(GameTime gameTime)
        {
            if (_gameCam != null && listener != null)
            {
                listener.Position = new Vector3(_gameCam.cameraPosition.X, _gameCam.cameraPosition.Y, 0f);
            }
        }

        public void setCam(Camera gameCam)
        {
            _gameCam = gameCam;

            listener = new AudioListener();
            listener.Position = new Vector3(_gameCam.cameraPosition.X, _gameCam.cameraPosition.Y, 0f);
            listener.Forward = Vector3.Down;
            listener.Up = Vector3.Forward;
        }

        public void clearList()
        {
            soundsList.Clear();
        }

        public void addSound(String key, String name)
        {
            if (!soundsList.ContainsKey(key))
            {
                soundsList.Add(key, _cm.Load<SoundEffect>(@"sounds/" + name));
            }
        }

        public void playSound(String key)
        {
            if (soundsList.ContainsKey(key))
            {
                soundsList[key].Play();
            }
        }

        public void playAmbientLoop(String key)
        {
            if (soundsList.ContainsKey(key))
            {
                SoundEffectInstance temp = soundsList[key].CreateInstance();

                temp.IsLooped = true;
                temp.Volume = .3f;
                temp.Play();

                loopingSoundsList.Add(temp);
                
            }
        }

        public void stopAmbientLoops()
        {
            for (int j = loopingSoundsList.Count; j >= 0; j--)
            {
                loopingSoundsList[j].Stop();
                loopingSoundsList.Remove(loopingSoundsList[j]);
            }
        }

        public SoundEffectInstance getSoundInstance(String key)
        {
            if (soundsList.ContainsKey(key))
            {
                return soundsList[key].CreateInstance();
            }
            else return null;
        }
    }
}
