using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static AudioSource _audioSource;
    private static AudioSource _musicSource;

    private static float _fadeTime = 0.75f;

    private void Awake()
    {
        _audioSource = GetComponents<AudioSource>()[0];
        _musicSource = GetComponents<AudioSource>()[1];
    }

    private void Start()
    {
        _musicSource.clip = Resources.Load<AudioClip>("Sound/keine");
        _musicSource.Play();
    }

    public static void Fire(string file)
    {
        if (Resources.Load<AudioClip>($"Sound/{file}") is not null)
        {
            _audioSource.PlayOneShot(Resources.Load<AudioClip>($"Sound/{file}"));
        }
    }

    public static IEnumerator PlayMusic(string file)
    {
        var elapsed = 0f;
        while (elapsed < _fadeTime)
        {
            var time = Time.deltaTime;
            elapsed += time;
            _musicSource.volume = Mathf.Lerp(0.5f, 0f, elapsed / _fadeTime);
            yield return new WaitForSeconds(time);
        }
        
        if (Resources.Load<AudioClip>($"Sound/{file}") is not null)
        {
            _musicSource.clip = Resources.Load<AudioClip>($"Sound/{file}");
            _musicSource.volume = 0.5f;
            _musicSource.Play();
        }
    }
}
