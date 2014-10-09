﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.display._3d;

namespace wickedcrush.manager.audio
{
    public class SoundManager
    {
        private Dictionary<String, Cue> cueList;
        private List<KeyValuePair<String, Cue>> tempList;


        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank;

        private ContentManager _cm;
        private Camera _gameCam;

        public AudioListener listener;

        public SoundManager(ContentManager cm)
        {
            audioEngine = new AudioEngine("Content/sounds/AudioEngine.xgs");
            waveBank = new WaveBank(audioEngine, "Content/sounds/Win/bfxr wave bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content/sounds/Win/bfxr sound bank.xsb");

            cueList = new Dictionary<String, Cue>();
            tempList = new List<KeyValuePair<String, Cue>>();

            SoundEffect.DistanceScale = 140f;

            _cm = cm;
        }

        public void Update(GameTime gameTime)
        {
            if (_gameCam != null && listener != null)
            {
                listener.Position = new Vector3(_gameCam.cameraPosition.X, _gameCam.cameraPosition.Y, 0f);
            }

            recreateCue();

            audioEngine.Update();
        }

        public void setCam(Camera gameCam)
        {
            _gameCam = gameCam;

            listener = new AudioListener();
            listener.Position = new Vector3(_gameCam.cameraPosition.X, _gameCam.cameraPosition.Y, 0f);
            listener.Forward = Vector3.Down;
            listener.Up = Vector3.Forward;
        }

        public void playCue(String name)
        {
            Cue cue = soundBank.GetCue(name);
            cue.Play();
            //soundBank.PlayCue(cue);
        }

        public void playCue(String name, AudioEmitter emitter)
        {
            Cue cue = soundBank.GetCue(name);
            cue.Apply3D(listener, emitter);
            cue.Play();
        }

        public void addCueInstance(String key, String instanceKey, bool loop)
        {
            Cue temp = soundBank.GetCue(key);

            if (!cueList.ContainsKey(instanceKey))
            {
                cueList.Add(instanceKey, temp);
                //cueList[instanceKey].Play();
                //cueList[instanceKey].Pause();
            }

        }

        public void playCueInstance(String instanceKey, AudioEmitter emitter)
        {
            if (cueList.ContainsKey(instanceKey) && cueList[instanceKey].IsPrepared)
            {
                cueList[instanceKey].Apply3D(listener, emitter);
                //if (cueList[instanceKey].IsStopped)
                //cueList[instanceKey].Resume();
                cueList[instanceKey].Play();
            }
        }

        public void stopCueInstance(String instanceKey, AudioEmitter emitter)
        {
            if (cueList.ContainsKey(instanceKey))
            {
                if (cueList[instanceKey].IsPlaying)
                {
                    cueList[instanceKey].Stop(AudioStopOptions.AsAuthored);
                    
                }
                
            }
        }

        private void recreateCue()
        {
            tempList.Clear();
            
            foreach(KeyValuePair <String, Cue> cue in cueList)
            {
                if(cue.Value.IsStopped)
                {
                    tempList.Add(cue);
                }
            }

            foreach(KeyValuePair<String, Cue> pair in tempList)
            {
                cueList[pair.Key] = soundBank.GetCue(pair.Value.Name);
            }
        }
    }
}
