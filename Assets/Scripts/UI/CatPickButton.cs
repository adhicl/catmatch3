using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class CatPickButton : MonoBehaviour
{
    public Image imgCat;
    public Image objCheck;
    
    public Sprite spriteCheck;
    public Sprite spriteUncheck;
    
    public Sprite spriteDisable;
    public UnlockCat unlockCat;

    public void Init(UnlockCat _unlockCat, bool hasUnlockedCat)
    {
        unlockCat = _unlockCat;
        imgCat.sprite = hasUnlockedCat?StoneFactory.Instance.GetCatSprite(unlockCat.catSprite):spriteDisable;
    }

    public void ShowCheck(bool showCheck)
    {
        objCheck.sprite = showCheck?spriteCheck:spriteUncheck;
    }
}