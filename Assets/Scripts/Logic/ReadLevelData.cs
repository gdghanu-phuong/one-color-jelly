using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class ReadLevelData : MonoBehaviour
{
    public List<LevelData> levelDataList = new();
    public TextMeshProUGUI levelText;
    public Image targetColor;
    public TextMeshProUGUI moveText;
    public string color;
    public int targetLevel;
    public int moveLimited;

    void Start()
    {
        ReadData();
        DisplayData();
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
    void DisplayData() 
    {         
        for (int i = 0; i < levelDataList.Count; i++)
        {
            LevelData levelData = levelDataList[targetLevel - 1];
            levelText.text = levelData.level.ToString();
            moveLimited = levelData.move;
            moveText.text = moveLimited.ToString();
            targetColor.sprite = GetColorSprite(levelData.targetColor);
            color = levelData.targetColor;
        }
    }
}
