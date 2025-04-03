using System;
using UnityEditor;

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
        Pause
    };

}
