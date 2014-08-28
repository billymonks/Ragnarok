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
        private Dictionary<String, SoundEffectInstance> instancedAmbientSounds; 
        //todo: hashmap? for instanced sounds from an entity, to contain emitter also. for stuff that needs to be synced, ambient is wrong 

        private ContentManager _cm;
        private Camera _gameCam;

        public AudioListener listener;

        public SoundManager(ContentManager cm)
        {
            soundsList = new Dictionary<String, SoundEffect>();
            instancedAmbientSounds = new Dictionary<String, SoundEffectInstance>();

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

        public void addAmbient(String key, String instanceKey, bool loop)
        {
            if (soundsList.ContainsKey(key))
            {
                SoundEffectInstance temp = soundsList[key].CreateInstance();

                temp.IsLooped = loop;
                temp.Volume = .3f;

                if(!instancedAmbientSounds.ContainsKey(instanceKey))
                    instancedAmbientSounds.Add(instanceKey, temp);
                
            }
        }

        public void playAmbient(String instanceKey)
        {
            if(instancedAmbientSounds.ContainsKey(instanceKey))
            {
                instancedAmbientSounds[instanceKey].Play();
            }
        }

        public void stopAmbientSounds()
        {
            foreach(KeyValuePair<String, SoundEffectInstance> pair in instancedAmbientSounds)
            {
                pair.Value.Stop();
            }

            instancedAmbientSounds.Clear();
        }

        public void stopAmbientSound(String key)
        {
            if(instancedAmbientSounds.ContainsKey(key))
                instancedAmbientSounds[key].Stop();
        }

        public SoundEffectInstance getAmbientSound(String instanceKey)
        {
            return instancedAmbientSounds[instanceKey];
        }

        public SoundEffectInstance createSoundInstance(String key)
        {
            if (soundsList.ContainsKey(key))
            {
                return soundsList[key].CreateInstance();
            }
            else return null;
        }
    }
}
