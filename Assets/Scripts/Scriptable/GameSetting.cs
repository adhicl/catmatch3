using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/Setting", order = 0)]
public class GameSetting : ScriptableObject
{
    public string[] stoneTypes;
    public string[] pickTypes;
    public string[] spriteTypes;

    public bool hasShowTutorial = false;

    public bool isSoundOn;
    public bool isMusicOn;

    public float curScore;
    public float curHighScore;

    public float curAccumulatedScore;
    public int dailyLoginBonus;

    public int countAccumulatedClear;
    public int countHighscore;
    public int countFinishDaily;
    
    public UnlockCat[] unlockCats;

    public string curPlayerId;
    public string curPlayerName;

    public void Reset()
    {
        for (int i = 0; i < 4; i++)
        {
            pickTypes[i] = stoneTypes[3 + i];
        }
        
        hasShowTutorial = false;
        isSoundOn = true;
        isMusicOn = true;
        curScore = 0;
        curHighScore = 0;
        
        curAccumulatedScore = 0;
        dailyLoginBonus = 0;
        
        countAccumulatedClear = 0;
        countHighscore = 0;
        countFinishDaily = 0;
        
        curPlayerId = "";
        curPlayerName = "";
    }

    public void SaveData()
    {
        PlayerPrefs.SetString("curPlayerId", curPlayerId);
        PlayerPrefs.SetString("curPlayerName", curPlayerName);
        
        PlayerPrefs.SetFloat("curHighScore", curHighScore);
        PlayerPrefs.SetFloat("curAccumulatedScoer", curAccumulatedScore);
        PlayerPrefs.SetInt("dailyLoginBonus", dailyLoginBonus);
        PlayerPrefs.SetInt("countAccumulatedClear", countAccumulatedClear);
        PlayerPrefs.SetInt("countHighscore", countHighscore);
        PlayerPrefs.SetInt("countFinishDaily", countFinishDaily);
        
        PlayerPrefs.SetInt("isSoundOn", isSoundOn ? 1 : 0);
        PlayerPrefs.SetInt("isMusicOn", isMusicOn ? 1 : 0);
        PlayerPrefs.SetInt("hasShowTutorial", hasShowTutorial ? 1 : 0);

        string pickTypeString = String.Join(",", pickTypes);
        PlayerPrefs.SetString("pickTypes", pickTypeString);
    }

    public void LoadData()
    {
        curPlayerId = PlayerPrefs.GetString("curPlayerId", string.Empty);
        curPlayerName = PlayerPrefs.GetString("curPlayerName", string.Empty);

        curScore = 0;
        curHighScore = PlayerPrefs.GetFloat("curHighScore", 0f);
        curAccumulatedScore = PlayerPrefs.GetFloat("curAccumulatedScoer", 0f);
        dailyLoginBonus = PlayerPrefs.GetInt("dailyLoginBonus", 0);
        countAccumulatedClear = PlayerPrefs.GetInt("countAccumulatedClear", 0);
        countHighscore = PlayerPrefs.GetInt("countHighscore", 0);
        countFinishDaily = PlayerPrefs.GetInt("countFinishDaily", 0);
        
        isSoundOn = PlayerPrefs.GetInt("isSoundOn", 1) == 1 ? true : false;
        isMusicOn = PlayerPrefs.GetInt("isMusicOn", 1) == 1 ? true : false;
        hasShowTutorial = PlayerPrefs.GetInt("hasShowTutorial", 0) == 1 ? true : false;
        
        string pickTypeString = PlayerPrefs.GetString("pickTypes", string.Empty);
        if (pickTypeString == string.Empty)
        {
            pickTypes = new string[4];
            for (int i = 0; i < 4; i++)
            {
                pickTypes[i] = stoneTypes[3 + i];
            }
        }
        else
        {
            pickTypes = pickTypeString.Split(",");
        }
    }
}