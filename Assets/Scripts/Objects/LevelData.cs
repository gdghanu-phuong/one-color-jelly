using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int Level; 
    public string TargetColor; 
    public int Move;

    public LevelData(int level, string targetColor, int move)
    {
        Level = level;
        TargetColor = targetColor;
        Move = move;
    }
}
