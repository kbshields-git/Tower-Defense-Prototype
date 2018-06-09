using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio Events/Simple")]
public class SimpleAudioEvent : AudioEvent
{
    public int clipIndex = 0;
    public bool playRandom = false;
    public AudioClip[] clips;

    public RangedFloat volume;

    [MinMaxRange(0, 2)]
    public RangedFloat pitch;

    public override void Play(AudioSource source)
    {
        if (clips.Length == 0) return;

        if (playRandom)
        {
            source.clip = clips[Random.Range(0, clips.Length)];
        }
        else
        {
            if(clipIndex >= clips.Length) { clipIndex = 0; }
            source.clip = clips[clipIndex];
            clipIndex += 1;
            // Return to first clip after exceeding total clip count
            
        }
        source.volume = Random.Range(volume.minValue, volume.maxValue);
        source.pitch = Random.Range(pitch.minValue, pitch.maxValue);
        source.Play();
    }
}