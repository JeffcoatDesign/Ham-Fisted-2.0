using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource music;
    public Slider volumeSlider;
    private float volume;

    private void Start()
    {
        volume = .5f;
        if (volumeSlider != null)
            volumeSlider.value = volume;
        music.volume = volume;
    }

    public void OnSliderValueChanged()
    {
        HandleSliderValueChanged(volumeSlider.value);
    }

    public void HandleSliderValueChanged (float value)
    {
        volume = value;
        music.volume = volume;
    }
}
