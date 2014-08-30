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
        private Dictionary<String, SoundEffectInstance> instancedSounds; 
        //todo: hashmap? for instanced sounds from an entity, to contain emitter also. for stuff that needs to be synced, ambient is wrong 

        private ContentManager _cm;
        private Camera _gameCam;

        public AudioListener listener;

        public SoundManager(ContentManager cm)
        {
            soundsList = new Dictionary<String, SoundEffect>();
            instancedSounds = new Dictionary<String, SoundEffectInstance>();

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

        public void addInstance(String key, String instanceKey, bool loop)
        {
            if (soundsList.ContainsKey(key))
            {
                SoundEffectInstance temp = soundsList[key].CreateInstance();

                temp.IsLooped = loop;
                temp.Volume = 1f;

                if(!instancedSounds.ContainsKey(instanceKey))
                    instancedSounds.Add(instanceKey, temp);
                
            }
        }

        public void playAmbient(String instanceKey)
        {
            if(instancedSounds.ContainsKey(instanceKey))
            {
                instancedSounds[instanceKey].Play();
            }
        }

        public void playInstanced(String instanceKey, AudioEmitter emitter)
        {
            if(instancedSounds.ContainsKey(instanceKey))
            {
                instancedSounds[instanceKey].Apply3D(listener, emitter);
                instancedSounds[instanceKey].Play();
            }
        }

        public void stopInstancedSounds()
        {
            foreach(KeyValuePair<String, SoundEffectInstance> pair in instancedSounds)
            {
                pair.Value.Stop();
            }

            instancedSounds.Clear();
        }

        public void stopInstancedSound(String key)
        {
            if(instancedSounds.ContainsKey(key))
                instancedSounds[key].Stop();
        }

        public SoundEffectInstance getAmbientSound(String instanceKey)
        {
            return instancedSounds[instanceKey];
        }

        public SoundEffectInstance createSoundInstance(String key)
        {
            if (soundsList.ContainsKey(key))
            {
                return soundsList[key].CreateInstance();
            }
            else return null;
        }

        public void fire3DSound(String key, AudioEmitter emitter)
        {
            SoundEffectInstance temp = createSoundInstance(key);
            temp.Apply3D(listener, emitter);
            temp.Play();
        }
    }
}
