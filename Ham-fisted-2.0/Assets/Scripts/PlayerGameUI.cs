using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerGameUI : MonoBehaviour
{
    public int id = -1;
    [SerializeField] private Transform livesRow;
    [SerializeField] private Transform sliderContainer;
    [SerializeField] private Transform controlsContainer;
    [SerializeField] private Transform deadScreen;
    [SerializeField] private GameObject playerIconPrefab;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private Image sliderFill;
    [SerializeField] private Slider chargeSlider;
    [SerializeField] private ControlIcon attackIcon;
    [SerializeField] private ControlIcon shieldIcon;
    [SerializeField] private ControlIcon leaveIcon;
    [SerializeField] private ControlIcon nextIcon;
    [SerializeField] private ControlIcon lastIcon;
    private float currentChargeValue;

    private PlayerIcon[] icons = new PlayerIcon[12];

    public void UpdateSliderValue (float value)
    {
        currentChargeValue = value;
        StartCoroutine(LerpSlider());
    }

    public void SetScheme(ControlIconScheme controlIconScheme)
    {
        deadScreen.gameObject.SetActive(true);
        attackIcon.SwitchScheme(controlIconScheme);
        shieldIcon.SwitchScheme(controlIconScheme);
        leaveIcon.SwitchScheme(controlIconScheme);
        nextIcon.SwitchScheme(controlIconScheme);
        lastIcon.SwitchScheme(controlIconScheme);
        deadScreen.gameObject.SetActive(false);
    }

    public void Start()
    {
        PodiumManager.instance.WinnersEvent.AddListener(SetText);
        deadScreen.gameObject.SetActive(false);
    }

    public void SetSliderColor (Color color)
    {
        sliderFill.color = color;
        if (icons[id] != null)
            icons[id].SetColor(color);
    }

    public void SpawnPlayerIcon (int index)
    {
        GameManager.instance.players[index].OnDie.AddListener(EnableDeadGUI);
        if (id < 0) id = index;
        PlayerIcon icon = Instantiate(playerIconPrefab, livesRow).GetComponentInChildren<PlayerIcon>();
        icon.SetNumber(index);
        icon.SetColor(index);
        icon.SpawnLives(GameManager.instance.playerLives);
        icons[index] = icon;
    }

    public void RemoveLife(int index)
    {
        icons[index].RemoveLife();
    }

    public void RemoveIcon (int index)
    {
        icons[index].Remove();
    }

    public void SetText(string text)
    {
        winText.gameObject.SetActive(true);
        winText.text = text;
    }

    public void HideText()
    {
        winText.gameObject.SetActive(false);
        winText.text = "";
    }

    public void EnableDeadGUI ()
    {
        sliderContainer.gameObject.SetActive(false);
        controlsContainer.gameObject.SetActive(false);
        deadScreen.gameObject.SetActive(true);
    }

    public void HideGUI ()
    {
        sliderContainer.gameObject.SetActive(false);
        controlsContainer.gameObject.SetActive(false);
        deadScreen.gameObject.SetActive(false);
        SetText("");
    }

    IEnumerator LerpSlider ()
    {
        while (chargeSlider.value != currentChargeValue)
        {
            chargeSlider.value = Mathf.Lerp(chargeSlider.value, currentChargeValue, 0.01f);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
