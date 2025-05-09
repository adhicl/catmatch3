using System;
using UnityEditor;
using UnityEngine;

public static class CommonVars
{
    public const float GRID_SIZE = 1.26f;
    public const int GRID_WIDTH = 6;
    public const int GRID_HEIGHT = 7;
    public const float THRESHOLD_MOVE = .5f;

    public const int MARK_FILLED = 0;
    public const int MARK_DESTROYED = -1;
    public const int MARK_HORIZONTAL_BOMB = -2;
    public const int MARK_VERTICAL_BOMB = -3;
    public const int MARK_COLOR_BOMB = -4;

    public static bool StartGame = true;
    public static bool AnimateStart = true;
    
    public static Vector3 shakeStrength = new Vector3(.5f, .5f, 0f);

    public static float[] AccumulatedScore =
    {
        100000, 300000, 500000, 500000, 600000, 700000, 800000, 900000, 1000000
    };

    public static float[] HighScoreTarget =
    {
        50000, 65000, 90000, 100000, 110000, 120000, 130000, 140000, 150000, 160000, 
        170000, 180000, 190000, 200000, 210000, 220000, 230000, 240000, 250000, 260000, 
        270000, 280000, 290000, 300000, 310000, 320000, 330000, 340000, 350000, 360000, 
        370000, 380000, 390000, 400000, 410000, 420000, 430000, 440000, 450000, 460000, 
        470000, 480000, 490000, 500000
    };

    public enum GameMode: byte
    {
        Menu,
        Play,
        Pause,
        Start,
        Finish
    };

    public enum UnlockType : int
    {
        always,
        totalScore,
        highScore,
        dailyBonus
    };

    public static string GetValueString(float value)
    {
        if (value >= 1000)
        {
            double shorten = Mathf.Floor(value / 1000);
            double rest = value % 1000;
            return $"{shorten}.{rest}K";
        }
        else
        {
            return value.ToString();
        }
    }

    public static String FloatToTimeString(float time)
    {
        return time.ToString("00");
        //
        // float hour = Mathf.Floor(time / 60);
        // float minute = time % 60;
        //
        // return hour.ToString("00")+":"+minute.ToString("00");
    }

}
