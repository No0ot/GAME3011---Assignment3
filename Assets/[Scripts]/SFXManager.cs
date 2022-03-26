using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

    public void PlayClip(AudioClip clip)
    {
            source.clip = clip;
            source.Play();
        if (!source.isPlaying)
        {
        }
    }
}
