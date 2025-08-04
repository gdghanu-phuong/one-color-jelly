using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReadMapData : MonoBehaviour
{
    public BlockController blockController;
    public ReadLevelData levelData;
    public SceneController sceneController;

    public RectTransform mapParent;
    public RectTransform mapContainer;
    public float tileSize = 100f;
    public float space = 30f;
    public BlockData[,] blockGrid;

    public int colCount;
    public int rowCount;
    private Vector2 gridOffset;
    void Start() => ReadData();

    void SetUpMap(int rowCount, int colCount)
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
       
        if (block.TryGetComponent<RectTransform>(out var rt))
        {
            rt.sizeDelta = new Vector2(tileSize, tileSize);
            rt.anchoredPosition = GetBlockPosition(col, row);
        }

        BlockData blockData = new(row, col, cell, block);
        blockGrid[row, col] = blockData;

        if(cell != "0")
        {
            if (!block.TryGetComponent<SwipeController>(out var swipeController))
            {
                swipeController = block.AddComponent<SwipeController>();
            }
            swipeController.row = row;
            swipeController.col = col;
            swipeController.blockColor = cell;
            swipeController.mapData = this;
            swipeController.levelData = levelData;
            swipeController.sceneController = sceneController;
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
                if (levelNumber == levelData.targetLevel)
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
        blockGrid = new BlockData[rowCount, colCount];
        SetUpMap(rowCount, colCount);

        for (int i = 0; i < rowCount; i++)
        {
            string[] mapRow = levelLines[i].Split(',');

            for (int j = 0; j < mapRow.Length; j++)
            {
                string cell = mapRow[j].Trim();

                GameObject blockPrefab = blockController.GetBlockPrefab(cell);
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
