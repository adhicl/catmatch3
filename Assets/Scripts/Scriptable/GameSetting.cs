using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/Setting", order = 0)]
public class GameSetting : ScriptableObject
{
    public string[] stoneTypes;
    public string[] pickTypes;
    public string[] spriteTypes;

    public bool hasShowTutorial;

    public bool isSoundOn;
    public bool isMusicOn;

    public float curScore;
    public float curHighScore;
    public bool isNewHighScore;

    public float curAccumulatedScore;
    public int dailyLoginBonus;

    public int countAccumulatedClear;
    public int countHighscore;
    public int countFinishDaily;
    
    public UnlockCat[] unlockCats;
}