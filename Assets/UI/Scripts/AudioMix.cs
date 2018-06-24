using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMix : MonoBehaviour {
    public AudioMixer masterMixer;

    public void SetMasterLvl(float lvl)
    {
        masterMixer.SetFloat("masterVol", lvl);
    }

    public void SetMusicLvl(float lvl)
    {
        masterMixer.SetFloat("musicVol", lvl);
    }

    public void SetSfxLvl(float lvl)
    {
        masterMixer.SetFloat("sfxVol", lvl);
    }
}
