using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class ReadLevelData : MonoBehaviour
{
    public static ReadLevelData instance;
    public List<LevelData> levelDataList = new();
    [SerializeField] private PersistentData persistentData;
    public TextMeshProUGUI levelText;
    public Image targetColor;
    public TextMeshProUGUI moveText;
    public string color;
    public int moveLimited;
    int currentLevel = 5;

    void Start()
    {
        if (levelText == null && moveText == null && targetColor == null) return;
        //int currentLevel = persistentData.TargetLevel;
        ReadData();
        DisplayData(currentLevel);
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    Sprite GetColorSprite(string color)
    {
        string colorPath = color switch
        {
            "R" => "Sprites/red_paint_",
            "G" => "Sprites/green_paint_",
            "B" => "Sprites/blue_paint_",
            "Y" => "Sprites/yellow_paint_",
            "P" => "Sprites/purple_paint_",
            _ => "Sprites/red_paint_"
        };

        return Resources.Load<Sprite>(colorPath);
    }

    void ReadData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Data/LevelData");
        StringReader stringReader = new(textAsset.text);

        stringReader.ReadLine();
        while (true)
        {
            string line = stringReader.ReadLine();
            if (string.IsNullOrEmpty(line)) break;
            string[] values = line.Split(',');
            if (values.Length != 3 || string.IsNullOrWhiteSpace(values[0])) continue;
            int level = int.Parse(values[0]);
            string targetColor = values[1].Trim();
            int move = int.Parse(values[2]);

            levelDataList.Add(new LevelData(level, targetColor, move));
        }
    }
    void DisplayData(int currentLevel) 
    {
        for (int i = 0; i < levelDataList.Count; i++) {
            LevelData levelData = levelDataList[currentLevel - 1];
            levelText.text = levelData.level.ToString();
            moveLimited = levelData.move;
            moveText.text = moveLimited.ToString();
            targetColor.sprite = GetColorSprite(levelData.targetColor);
            color = levelData.targetColor;
        }
    }

    public void LevelUp()
    {
        persistentData.TargetLevel += 1;
        SceneManager.LoadScene("PlayScene");
    }

    public void Retry()
    {
        SceneManager.LoadScene("PlayScene");
    }
}
