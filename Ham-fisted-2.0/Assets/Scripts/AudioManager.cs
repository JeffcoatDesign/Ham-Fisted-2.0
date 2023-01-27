using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private string volumeParameter;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider volumeSlider;
    private float volume = 0.5f;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(volumeParameter)) { 
            volume = PlayerPrefs.GetFloat(volumeParameter);
        }
    }

    private void Start()
    {
        if (volumeSlider != null)
            volumeSlider.value = volume;
        audioSource.volume = volume;
    }

    public void OnSliderValueChanged()
    {
        HandleSliderValueChanged(volumeSlider.value);
    }

    public void HandleSliderValueChanged (float value)
    {
        PlayerPrefs.SetFloat(volumeParameter, value);
        PlayerPrefs.Save();
        volume = value;
        audioSource.volume = volume;
    }
}
