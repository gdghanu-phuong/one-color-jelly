using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneController : MonoBehaviour
{
    public ReadMapData mapData;
    public ReadLevelData levelData;
    public Animator sceneTransition;
    [SerializeField] private PersistentData persistentData;

    void Start()
    {
        if (mapData == null || levelData == null) return;
        persistentData.MoveLeft = 0;
        if (sceneTransition != null)
        {
            sceneTransition.SetTrigger("PlaySceneTransition");

        }
    }

    public void Retry()
    {
        GameObject hand = GameObject.FindGameObjectWithTag("Hand");
        if (hand != null)
        {
            hand.SetActive(false);
        }
        mapData.ReadData(persistentData.TargetLevel);
        levelData.DisplayData(persistentData.TargetLevel);
    }

    public void CheckWinLoseCondition()
    {
        string targetColor = persistentData.TargetColor;
        int moveLimited = persistentData.MoveLeft;

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

            }
        }

        if (isColorMatch && moveLimited >= 0)
        {
            SceneManager.LoadScene("WinScene");
        }
        else if (!hasTargetColorBlock)
        {   
            persistentData.loseReason = PersistentData.LoseReason.WrongColor;
            SceneManager.LoadScene("LoseScene");
        }
        else if (moveLimited == 0)
        {
            persistentData.loseReason = PersistentData.LoseReason.OutOfMove;
            SceneManager.LoadScene("LoseScene");
        }
    } 
}