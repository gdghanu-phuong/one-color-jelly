using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int level; 
    public string targetColor; 
    public int move;

    public LevelData(int level, string targetColor, int move)
    {
        this.level = level;
        this.targetColor = targetColor;
        this.move = move;
    }
}
