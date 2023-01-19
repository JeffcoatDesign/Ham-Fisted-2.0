using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum OptionType
{
    stage,
    text,
    num
}

public class HorizontalSelection : MonoBehaviour
{
    public OptionType optionType;
    public Transform centerObj;
    public GameObject spawnedObjectPrefab;
    public Object[] options;
    public int index = 0;

    public Button leftBtn;
    public Button rightBtn;

    private void Start()
    {
        SetContent(0);
    }

    public void LeftButton ()
    {
        index--;
        if (index < 0)
            index = options.Length - 1;
        SetContent(index);
    }

    public void RightButton()
    {
        index++;
        if (index >= options.Length)
            index = 0;
        SetContent(index);
    }

    public void ClearContent ()
    {
        if (centerObj.childCount > 0)
            Destroy(centerObj.GetChild(0).gameObject);
    }

    void SetContent (int i)
    {
        index = i;
        ClearContent();
        GameObject obj = Instantiate(spawnedObjectPrefab, centerObj);
        if (optionType == OptionType.stage)
        {
            StageOption stageOption = (StageOption)options[i];
            obj.GetComponent<Image>().sprite = stageOption.sprite;
            Menu.instance.selectedStage = stageOption.scene;
        }
        else
        {
            SettingsOption settingsOption = (SettingsOption)options[i];
            if (optionType == OptionType.text)
            {
                Menu.instance.SendMessage("SetOpt" + settingsOption.functionName, index);
                obj.GetComponent<TextMeshProUGUI>().text = settingsOption.settingName;
            }
            else if (optionType == OptionType.num)
            {
                Menu.instance.SendMessage("SetOpt" + settingsOption.functionName, settingsOption.value);
                obj.GetComponent<TextMeshProUGUI>().text = index.ToString(); 
            }
        }
    }

    public void SetIndex(int i)
    {
        index = i;
        SetContent(index);
    }
}
