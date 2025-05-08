using UnityEngine;

public class SaveLoadData : MonoBehaviour
{
    #region singleton
    public static SaveLoadData Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    #endregion

    public GameSetting gameSetting;
    
    public void SaveGame()
    {
        
    }

    public void LoadGame()
    {
        
    }

    public void CheckUnlockAccumulated()
    {
        // int lvl = gameSetting.levelAccumulatedScore;
        //
        // int max = (lvl > CommonVars.AccumulatedScore.Length) ? lvl : CommonVars.AccumulatedScore.Length;
        // float targetAccumulated = CommonVars.AccumulatedScore[lvl - 1];
        // if (gameSetting.curAccumulatedScore >= targetAccumulated)
        // {
        //     
        // }
    }
}