using UnityEngine;

public class BlockController : MonoBehaviour
{
    public GameObject emptyBlockPrefab;
    public GameObject redBlockPrefab;
    public GameObject greenBlockPrefab;
    public GameObject blueBlockPrefab;
    public GameObject yellowBlockPrefab;
    public GameObject purpleBlockPrefab;

    public GameObject GetBlockPrefab(string color)
    {
        return color switch
        {
            "0" => emptyBlockPrefab,
            "R" => redBlockPrefab,
            "G" => greenBlockPrefab,
            "B" => blueBlockPrefab,
            "Y" => yellowBlockPrefab,
            "P" => purpleBlockPrefab,
            _ => null
        };
    }
}
