using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReadMapData : MonoBehaviour
{
    public GameObject emptyBlockPrefab;
    public GameObject redBlockPrefab;
    public GameObject blueBlockPrefab;
    public GameObject greenBlockPrefab;
    public GameObject yellowBlockPrefab;
    public GameObject purpleBlockPrefab;

    public RectTransform mapParent;
    public RectTransform mapContainer;
    public float tileSize = 100f;
    public float space = 30f;
    public int targetLevel = 3;

    public GameObject[,] blockGrid;
    public int colCount;
    public int rowCount;
    private Vector2 gridOffset;


    void Start() => ReadData();

    GameObject GetBlockPrefab(string cell)
    {
        if (string.IsNullOrEmpty(cell))
            return null;

        if (int.TryParse(cell, out int number))
        {
            if (number == 0)
                return emptyBlockPrefab;
            return null;
        }

        return cell switch
        {
            "R" => redBlockPrefab,
            "G" => greenBlockPrefab,
            "B" => blueBlockPrefab,
            "Y" => yellowBlockPrefab,
            "P" => purpleBlockPrefab,
            _ => null,
        };
    }

    void SetUpMap(int rows, int cols)
    {
        float totalWidth = colCount * tileSize + (colCount - 1) * space;
        float totalHeight = rowCount * tileSize + (rowCount - 1) * space;

        float parentWidth = mapParent.rect.width;
        float parentHeight = mapParent.rect.height;

        float scaleX = parentWidth / totalWidth;
        float scaleY = parentHeight / totalHeight;
        float scale = Mathf.Min(scaleX, scaleY, 1f);

        mapContainer.localScale = new Vector3(scale, scale, 1f);
        mapContainer.anchoredPosition = Vector2.zero;

        float offsetX = -totalWidth / 2f;
        float offsetY = totalHeight / 2f;
        gridOffset = new(offsetX, offsetY);
    }

    void SetUpBlock(GameObject block, int row, int col, string cell) 
    {
        RectTransform rt  = block.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = new Vector2(tileSize, tileSize);
            rt.anchoredPosition = GetBlockPosition(col, row);
        }

        blockGrid[row, col] = block;
        if(cell != "0")
        {
            if (!block.TryGetComponent<SwipeController>(out var swipeController))
            {
                swipeController = block.AddComponent<SwipeController>();
            }
            swipeController.row = row;
            swipeController.col = col;
            swipeController.mapData = this;
        }
    }
    void ReadData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Data/MapData");
        string[] lines = textAsset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        bool isLevelFound = false;
        List<string> levelLines = new();

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine))
                continue;
            string[] parts = trimmedLine.Split(',');
            if (int.TryParse(parts[0], out int levelNumber))
            {
                if (levelNumber == targetLevel)
                {
                    isLevelFound = true;
                    levelLines.Clear();
                    levelLines.Add(string.Join(",", parts.Skip(1)));
                }
                else
                {
                    isLevelFound = false;
                }
            }
            else if (isLevelFound)
            {
                if (parts.Length > 1)
                {
                    levelLines.Add(string.Join(",", parts.Skip(1)));  
                }
            }
        }

        rowCount = levelLines.Count;
        colCount = levelLines.Max(line => line.TrimEnd(',').Split(',').Length);
        blockGrid = new GameObject[rowCount, colCount];
        SetUpMap(rowCount, colCount);

        for (int i = 0; i < rowCount; i++)
        {
            string[] mapRow = levelLines[i].Split(',');

            for (int j = 0; j < mapRow.Length; j++)
            {
                string cell = mapRow[j].Trim();

                GameObject blockPrefab = GetBlockPrefab(cell);
                if (blockPrefab != null)
                {
                    GameObject obj = Instantiate(blockPrefab, mapContainer);
                    SetUpBlock(obj, i, j, cell);
                } 
                

            }
        }
    }

    public Vector2 GetBlockPosition(int col, int row)
    {
       float x = col * (tileSize + space);
       float y = -row * (tileSize + space);

       return gridOffset + new Vector2(x, y);
    }
}
