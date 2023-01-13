using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Settings Option", menuName = "Ham-fisted/Settings Option")]
public class SettingsOption : ScriptableObject
{
    public string settingName;
    public string functionName;
    public int value;
}
