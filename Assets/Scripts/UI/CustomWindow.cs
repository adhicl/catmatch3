using System;
using System.Collections.Generic;
using System.Linq;
using UI;
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
        int num = 0;
        if (catPickButtons == null)
        {
            catPickButtons = new List<CatPickButton>();

            int totalRow = Mathf.CeilToInt((float) gameSetting.unlockCats.Length / 3f);
            
            for (int i = 0; i < totalRow; i++)
            {
                GameObject obj = Instantiate(prefabUnlockCat, rectContent);
                CatPickButton[] buttons = obj.GetComponent<RowCatPickButtons>().GetCatPickButtons();
                foreach (var cat in buttons)
                {
                    bool isShow = num < gameSetting.unlockCats.Length;
                    
                    cat.gameObject.SetActive(isShow);
                    if (isShow)
                    {
                        cat.Init(gameSetting.unlockCats[num], gameSetting.unlockCats[num].hasUnlocked);
                        var i1 = num;
                        cat.GetComponent<Button>().onClick.AddListener(() => onPickCatListener(i1) );
                        catPickButtons.Add(cat);
                    }
                    num++;
                }
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