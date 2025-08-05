using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public ReadMapData mapData;
    public ReadLevelData levelData;
    [SerializeField] private PersistentData persistentData;

    void Start()
    {
        if (mapData == null || levelData == null) return;
    }
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CheckWinLoseCondition()
    {
        string targetColor = levelData.color;
        int moveLimited = levelData.moveLimited;

        bool isColorMatch = true;     
        bool hasTargetColorBlock = false; 

        for (int i = 0; i < mapData.rowCount; i++)
        {
            for (int j = 0; j < mapData.colCount; j++)
            {
                var block = mapData.blockGrid[i, j];
                if (block == null) continue;
                if (block.Block.CompareTag("Empty")) continue;

                string blockColor = block.Color;

                if (blockColor != targetColor)
                {
                    isColorMatch = false;
                }
                else
                {
                    hasTargetColorBlock = true;
                }
                
                if (isColorMatch) break;
            }
            if (isColorMatch && moveLimited >= 0)
            {
                Debug.Log("You win!");
            }
        }

        if (!hasTargetColorBlock || moveLimited <= 0)
        {
            Debug.Log("You lose!");
        }
    }
}
