using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public static void Fire(string file)
    {
        if (Resources.Load<AudioClip>($"Sound/{file}") is not null)
        {
            _audioSource.PlayOneShot(Resources.Load<AudioClip>($"Sound/{file}"));
        }
    }
}
