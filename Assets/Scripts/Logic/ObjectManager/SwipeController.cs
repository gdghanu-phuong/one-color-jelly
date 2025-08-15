using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int row, col;
    public string blockColor;
    public ReadLevelData levelData;
    public ReadMapData mapData;
    public PlaySceneController playSceneController;
    public HintController hintController;
    private Vector2 startPos;
    [SerializeField] private PersistentData persistentData;

    public void SetPersistentData(PersistentData data)
    {
        persistentData = data;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (startPos == Vector2.zero)
            startPos = eventData.position;
    }

    void ChangeColor(int row, int col, string newColor)
    {
        if (row < 0 || row >= mapData.rowCount || col < 0 || col >= mapData.colCount)
        {
            return;
        }
        var touchedBlock = mapData.blockGrid[row, col];
        if (touchedBlock != null)
        {
            touchedBlock.Color = newColor;
            Destroy(touchedBlock.Block);

            GameObject newBlock = Instantiate(mapData.blockController.GetBlockPrefab(newColor), mapData.mapContainer);
            if (newBlock.TryGetComponent<RectTransform>(out var rt))
            {
                rt.sizeDelta = new Vector2(mapData.tileSize, mapData.tileSize);
                rt.anchoredPosition = mapData.GetBlockPosition(row, col);
            }

            if (!newBlock.TryGetComponent<SwipeController>(out var swipeController))
            {
                swipeController = newBlock.AddComponent<SwipeController>();
            }
            swipeController.row = row;
            swipeController.col = col;
            swipeController.blockColor = newColor;
            swipeController.levelData = levelData;
            swipeController.mapData = mapData;
            swipeController.playSceneController = playSceneController;
            swipeController.hintController = hintController;
            swipeController.persistentData = persistentData;
            touchedBlock.Block = newBlock;
        }
    }
    void SpreadColor(int row, int col, string oldColor, string newColor)
    {
        if (oldColor == newColor) return;
        bool[,] isBlockChecked = new bool[mapData.rowCount, mapData.colCount];
        Queue<(int, int)> queue = new();
        queue.Enqueue((row, col));
        isBlockChecked[row, col] = true;

        int[] dRow = { -1, 1, 0, 0 };
        int[] dCol = { 0, 0, -1, 1 };

        while (queue.Count > 0)
        {
            (int r, int c) = queue.Dequeue();
            var block = mapData.blockGrid[r, c];
            if (block != null && block.Color == oldColor)
            {
                ChangeColor(r, c, newColor);
                for (int i = 0; i < 4; i++)
                {
                    int newRow = r + dRow[i];
                    int newCol = c + dCol[i];
                    if (newRow >= 0 && newRow < mapData.rowCount && 
                        newCol >= 0 && newCol < mapData.colCount &&
                        mapData.blockGrid[newRow, newCol] != null &&
                        !isBlockChecked[newRow, newCol] &&
                        !mapData.blockGrid[newRow, newCol].Block.CompareTag("Empty") &&
                        mapData.blockGrid[newRow, newCol].Color == oldColor)
                    {
                        queue.Enqueue((newRow, newCol));
                        isBlockChecked[newRow, newCol] = true;
                    }
                }
            }
        }
    }

   
    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject hand = GameObject.FindGameObjectWithTag("Hand");
        if(hand != null)
        {
            hand.SetActive(false);
        }
        Vector2 endPos = eventData.position;
        Vector2 delta = endPos - startPos;

        if (delta.magnitude < 50f) return; 
        int startRow = row;
        int startCol = col;

        Vector2 direction = delta.normalized;
        int dRow = 0, dCol = 0;
        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            dCol = direction.x > 0 ? 1 : -1;
        }
        else
        {
            dRow = direction.y < 0 ? 1 : -1; 
        }

        int currentRow = row;
        int currentCol = col;
        int nextRow = currentRow + dRow;
        int nextCol = currentCol + dCol;

        while (nextRow >= 0 && nextRow < mapData.rowCount && 
            nextCol >= 0 && nextCol < mapData.colCount &&
            mapData.blockGrid[nextRow, nextCol] != null &&
            mapData.blockGrid[nextRow, nextCol].Block.CompareTag("Empty"))
        {
            currentRow = nextRow;
            currentCol = nextCol;
            nextRow += dRow;
            nextCol += dCol;
        }

        if (nextRow >= 0 && nextRow < mapData.rowCount &&
            nextCol >= 0 && nextCol < mapData.colCount &&
            mapData.blockGrid[nextRow, nextCol] != null &&
            !mapData.blockGrid[nextRow, nextCol].Block.CompareTag("Empty"))
        {
            string oldColor = mapData.blockGrid[nextRow, nextCol].Color;
            SpreadColor(nextRow, nextCol, oldColor, blockColor);
        }


        GameObject emptyBlock = Instantiate(mapData.blockController.emptyBlockPrefab, mapData.mapContainer);
        if(emptyBlock.TryGetComponent<RectTransform>(out var rt))
        {
            rt.sizeDelta = new Vector2(mapData.tileSize, mapData.tileSize);
            rt.anchoredPosition = mapData.GetBlockPosition(row, col);
        }
        emptyBlock.tag = "Empty";
        mapData.blockGrid[currentRow, currentCol].Color = mapData.blockGrid[row, col].Color;
        mapData.blockGrid[currentRow, currentCol].Block = mapData.blockGrid[row, col].Block;

        if (row != currentRow || col != currentCol)
        {
            mapData.blockGrid[row, col].Block = emptyBlock;
            mapData.blockGrid[row, col].Color = "0";
        }
        row = currentRow;
        col = currentCol;

        levelData.moveLimited -= 1;
        levelData.moveText.text = levelData.moveLimited.ToString();
        persistentData.MoveLeft = levelData.moveLimited;
        transform.SetAsLastSibling();
        Vector2 targetPos = mapData.GetBlockPosition(row, col);
        StartCoroutine(MoveToPosition(targetPos, startRow, startCol));
    }

    private System.Collections.IEnumerator MoveToPosition(Vector2 targetPos, int startRow, int startCol)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 startPos = rectTransform.anchoredPosition;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
        yield return new WaitForSeconds(0.1f);
        if (hintController != null)
        {
            hintController.OnPlayerMove(startRow, startCol);
        }
        playSceneController.CheckWinLoseCondition();

    }
}
