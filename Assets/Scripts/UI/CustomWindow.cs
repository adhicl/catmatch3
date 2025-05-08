using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomWindow : MonoBehaviour
{
    public GameObject prefabUnlockCat;

    public GameSetting gameSetting;

    [SerializeField] private Button btnBack;
    [SerializeField] private Transform rectContent;
    
    private List<CatPickButton> catPickButtons;

    private void Start()
    {
        btnBack.onClick.AddListener(BackButtonListener);
    }

    private void BackButtonListener()
    {
        SoundController.Instance.PlayButtonClip();
        this.gameObject.SetActive(false);
    }

    public void RedrawUI()
    {
        if (catPickButtons == null)
        {
            catPickButtons = new List<CatPickButton>();
            for (int i = 0; i < gameSetting.unlockCats.Length; i++)
            {
                GameObject obj = Instantiate(prefabUnlockCat, rectContent);
                CatPickButton cat  = obj.GetComponent<CatPickButton>();
                cat.Init(gameSetting.unlockCats[i], gameSetting.unlockCats[i].hasUnlocked);
                var i1 = i;
                cat.GetComponent<Button>().onClick.AddListener(() => onPickCatListener(i1) );
                catPickButtons.Add(cat);
            }
        }
        
        foreach (var catPickButton in catPickButtons)
        {
            catPickButton.ShowCheck(gameSetting.pickTypes.Contains(catPickButton.unlockCat.catObject));
        }
        
    }

    private int pickCatPosition = 0; 
    private void onPickCatListener(int pos)
    {
        UnlockCat cat = catPickButtons[pos].unlockCat;
        if (!cat.hasUnlocked) return;
        
        if (!gameSetting.pickTypes.Contains(cat.catObject))
        {
            gameSetting.pickTypes[pickCatPosition] = cat.catObject;
            pickCatPosition++;
            if (pickCatPosition >= 4) pickCatPosition = 0;
        }

        RedrawUI();
    }
    
}