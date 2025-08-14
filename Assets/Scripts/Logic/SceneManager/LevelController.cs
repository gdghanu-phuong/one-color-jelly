using UnityEngine;
using TMPro;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
public class LevelController : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI moveText;
    public GameObject targetColor;
    public ReadLevelData levelData;
    [SerializeField] private PersistentData persistentData;
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "WinScene")
        {
            DisplayWinData();
        }
        else if(SceneManager.GetActiveScene().name == "LoseScene")
        {
            DisplayLoseData();
        }
    }

    public void DisplayWinData()
    {
        levelText.text = persistentData.TargetLevel.ToString();
        targetColor.GetComponent<Image>().sprite = levelData.GetColorSprite(persistentData.TargetColor);
    }

    public void Retry()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void LevelUp()
    {
        bool isGameCompleted = false;
        if (!isGameCompleted)
        {
            persistentData.TargetLevel++;
            if (persistentData.TargetLevel > 20)
            {
                isGameCompleted = true;
            }
        }
        if (isGameCompleted)
        {
            System.Random randomLevel = new();
            persistentData.TargetLevel = randomLevel.Next(11, 20);
        }
        
        SceneManager.LoadScene("StartScene");
    }

    public void DisplayLoseData()
    {
        levelText.text = persistentData.TargetLevel.ToString();
        moveText.text = $"{persistentData.MoveLeft}/{persistentData.InitialMoveLimited}";
        targetColor.GetComponent<Image>().sprite = levelData.GetColorSprite(persistentData.TargetColor);
    }
}
