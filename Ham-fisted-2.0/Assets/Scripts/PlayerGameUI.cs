using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerGameUI : MonoBehaviour
{
    [SerializeField] private Transform livesRow;
    [SerializeField] private GameObject playerIconPrefab;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private Image sliderFill;
    [SerializeField] private Slider chargeSlider;
    private float currentChargeValue;

    private PlayerIcon[] icons = new PlayerIcon[12];

    public void UpdateSliderValue (float value)
    {
        currentChargeValue = value;
        StartCoroutine(LerpSlider());
    }

    public void SetSliderColor (Color color)
    {
        sliderFill.color = color;
    }

    public void SpawnPlayerIcon (int index)
    {
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

    public void SetWinText (string text)
    {
        winText.gameObject.SetActive(true);
        winText.text = text + " is the Champion";
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
