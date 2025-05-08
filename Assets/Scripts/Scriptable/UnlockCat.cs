using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/Unlockable", order = 0)]
[Serializable]
public class UnlockCat : ScriptableObject
{
    public int id;
    public bool hasUnlocked;
    public string catObject;
    public string catSprite;

    public CommonVars.UnlockType unlockType;
    public int unlockLevel;
}