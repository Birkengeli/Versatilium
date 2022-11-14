using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{

    public enum SoundTypes
    {
        Unassigned,

        ___Weapons__,
        OnFire,
        OnFire_Charged,
        OnReload,

        ___Combat__,
        OnTakingDamage,
        OnTakingDamageWhileInvincible,
    }

    [Header("Reminder: Not every script needs every Sound filled out.")]
    public SoundTypes soundType;
    public AudioClip[] audioClips;
    [Header("Settings")]
    public randomTypes random = randomTypes.NeverTwice;

    private int lastIndex = 999;

    public static void Play(SoundTypes soundType, Sound[] sounds, AudioSource audioSource, bool ignoreMissing = false)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].soundType == soundType)
            {
                Play(sounds[i], audioSource);
                return;
            }
        }

        if (!ignoreMissing)
            Debug.LogWarning("Could Not find '" + soundType.ToString() + "' among the given Sounds.");
    }

    public static void Play(Sound sound, AudioSource audioSource)
    {
        AudioClip clip = sound.audioClips[0];

        if (sound.audioClips.Length < 2) // If it's set to never twice and it's 1 or zero files, it will never find it.
            sound.random = randomTypes.Random;

        #region Random Order
        if (sound.random == randomTypes.Random)
            clip = sound.audioClips[Random.Range(0, sound.audioClips.Length)];

        if (sound.random == randomTypes.NeverTwice)
        {
            int randomIndex = -1;
            while (true)
            {
                randomIndex = Random.Range(0, sound.audioClips.Length);

                if (randomIndex != sound.lastIndex)
                {
                    sound.lastIndex = randomIndex;
                    clip = sound.audioClips[randomIndex];

                    break;
                }
            }
        }

        if (sound.random == randomTypes.Sequential)
        {
            int clipCount = sound.audioClips.Length;

            if (sound.lastIndex >= clipCount)
                sound.lastIndex = 0;

            clip = sound.audioClips[sound.lastIndex];
            sound.lastIndex++;
        }

        #endregion

        audioSource.PlayOneShot(clip, 1);
    }

    public enum randomTypes
    { Sequential, NeverTwice, Random }

}
