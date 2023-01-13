using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage Option", menuName = "Ham-fisted/Stage Option")]
public class StageOption : ScriptableObject
{
    public Sprite sprite;
    public string scene;
}
