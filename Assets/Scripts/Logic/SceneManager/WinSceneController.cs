using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;

public class WinSceneController : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public GameObject targetColor;
    public ReadLevelData levelData;
    [SerializeField] private PersistentData persistentData;

    public Animator sceneTransition;
    public List<int> randomLevels  = new List<int>();
    void Start()
    {
        if (sceneTransition != null)
        {
            sceneTransition.SetTrigger("WinSceneTransition");
        }
        DisplayWinData();
        AdsController.instance.bannerAds.ShowBannerAd();
    }


    void DisplayWinData()
    {
        levelText.text = persistentData.TargetLevel.ToString();
        targetColor.GetComponent<Image>().sprite = levelData.GetColorSprite(persistentData.TargetColor);
    }

    public void LevelUp()
    {
        if (!persistentData.IsGameCompleted)
        {
            persistentData.TargetLevel++;
            if (persistentData.TargetLevel > 20)
            {
                persistentData.IsGameCompleted = true;
            }
        }
        if (persistentData.IsGameCompleted)
        {
            System.Random randomLevel = new();
            persistentData.TargetLevel = randomLevel.Next(11, 20);
        }

        SceneManager.LoadScene("LoadingScene");
    }

}
