using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Vaz.ManagerSingletons
{
    public class VolumeManager : MonoBehaviour
    {
        public static VolumeManager singleton { get; private set; }
        // ^ I'm probably going to fucking hate myself for adding this later.

        public AudioMixer audioMixer;

        void Awake()
        {
            // ensures this remains a singleton
            if (singleton != null && singleton != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                singleton = this; 
            }

            DontDestroyOnLoad(this.gameObject);
        }

        public void SetMasterVolume (float volume)
        {
            audioMixer.SetFloat("MSTvolume", volume);
        }

        public void SetSFXVolume (float volume)
        {
            audioMixer.SetFloat("SFXvolume", volume);
        }

        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("BGMvolume", volume);
        }

        public void SetDialogueVolume (float volume)
        {
            audioMixer.SetFloat("DIAvolume", volume);
        }    
    }
}

