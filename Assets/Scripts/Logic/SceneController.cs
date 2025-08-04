using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public ReadMapData mapData;
    public ReadLevelData levelData;
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CheckWinCondition()
    {
        string targetColor = levelData.color;
        bool isColorMatch = true;

        for (int i = 0; i < mapData.rowCount; i++)
        {
            for (int j = 0; j < mapData.colCount; j++)
            {
                var block = mapData.blockGrid[i, j];
                if (block == null) continue;
                string blockColor = block.Color;
                Debug.Log($"Block ({i},{j}) color: {blockColor}");

                if (!block.Block.CompareTag("Empty"))
                {
                    if (blockColor != targetColor)
                    {
                        isColorMatch = false;
                    } else
                    {
                        isColorMatch = true;
                    }
                }
            }
        }
        
        if(isColorMatch)
        {
            Debug.Log("You Win!");
            SceneManager.LoadScene("WinScene");
        }
    }
}
