using UnityEngine;

[CreateAssetMenu(fileName = "PersistentData", menuName = "Scriptable Objects/PersistentData")]
public class PersistentData : ScriptableObject
{
    private int targetLevel = 1;
    public int TargetLevel
    {
        get { return targetLevel; }
        set
        {
            targetLevel = value;
        }
    }
}
