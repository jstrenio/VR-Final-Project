using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blood_script : MonoBehaviour
{
    public ParticleSystem part;
    public bool squirt;
    public AudioSource audio;
    public AudioClip squish;

    // Start is called before the first frame update
    void Start()
    {
        part = GetComponent<ParticleSystem>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (squirt && !part.isPlaying)
        {
            part.Play();
            audio.Play();
        }
        else if (!squirt)
        {
            part.Pause();
            part.Clear();
        }
    }
}
