using System;
using UnityEditor;
using UnityEngine;

public static class CommonVars
{
    public const float GRID_SIZE = 1.26f;
    public const int GRID_WIDTH = 6;
    public const int GRID_HEIGHT = 7;
    public const float THRESHOLD_MOVE = .5f;
    
    public const int MARK_FILLED = -1;
    public const int MARK_DESTROYED = -1;
    public const int MARK_HORIZONTAL_BOMB = -2;
    public const int MARK_VERTICAL_BOMB = -3;
    public const int MARK_COLOR_BOMB = -4;

    public enum GameMode: byte
    {
        Menu,
        Play,
        Pause,
        Start,
        Finish
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
        float hour = time / 60;
        float minute = time % 60;
        
        return hour.ToString("00")+":"+minute.ToString("00");
    }

}
