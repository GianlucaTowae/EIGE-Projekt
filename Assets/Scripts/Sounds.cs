using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sounds : MonoBehaviour
{
    public enum Sound
    {
        PROJECTILE,
        HIT_STONE,
        HIT_METAL,
        DAMAGE_TAKEN,
        BOSS_BEAM_CHARGE,
        BOSS_BEAM,
        BOSS_PROJECTILE,
        BOSS_SPAWN,
        ABILITY_PICKUP,
        ASTEROID_EXPLOSION,
        PLANET_EXPLOSION,
        WIN,
        LOSE,
        LEVEL_UP,
        BUTTON
    }

    [Serializable]
    private class SingleAudioClip
    {
        public Sound sound;
        public AudioClip clip;
        public float volume = 1;
    }

    [SerializeField] private List<SingleAudioClip> audioClips;

    private static AudioSource _audioSource;
    private static List<SingleAudioClip> _staticAudioClips;

    private void Awake()
    {
        if (Camera.main != null)
            _audioSource = Camera.main.transform.GetComponent<AudioSource>();
        _staticAudioClips = audioClips;
    }

    public static void Play(Sound sound)
    {
        SingleAudioClip audioClip;
        try
        {
            audioClip = _staticAudioClips.First(clip => clip.sound == sound);
        }
        catch (Exception e)
        {
            Debug.Log(sound + " isn't loaded");
            return;
        }
        _audioSource.PlayOneShot(audioClip.clip, audioClip.volume);
    }

    public static void PlayButtonSound()
    {
        Play(Sound.BUTTON);
    }
}