using UnityEngine;

[CreateAssetMenu(fileName = "PersistentData", menuName = "Scriptable Objects/PersistentData")]
public class PersistentData : ScriptableObject
{
    private int targetLevel = 1;
    private int moveLeft = 0;
    private int initialMoveLimited = 0;
    private string targetColor = "R";
    public enum LoseReason { None, WrongColor, OutOfMove}
    public LoseReason loseReason;

    public int TargetLevel
    {
        get { return targetLevel; }
        set {targetLevel = value;}
    }
    public int MoveLeft
    {
        get { return moveLeft; }
        set { moveLeft = value; }
    }

    public int InitialMoveLimited
    {
        get { return initialMoveLimited; }
        set { initialMoveLimited = value; }
    }

    public string TargetColor
    {
        get { return targetColor; }
        set { targetColor = value; }
    }

}
