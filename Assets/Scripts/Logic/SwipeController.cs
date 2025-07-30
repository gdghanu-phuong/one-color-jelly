using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int row, col;
    public ReadMapData mapData;

    private Vector2 startPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (startPos == Vector2.zero)
            startPos = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 endPos = eventData.position;
        Vector2 delta = endPos - startPos;

        if (delta.magnitude < 50f) return; 

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

        //loop until find a valid block
        while (nextRow >= 0 && nextRow < mapData.rowCount && 
            nextCol >= 0 && nextCol < mapData.colCount &&
            mapData.blockGrid[nextRow, nextCol] != null &&
            mapData.blockGrid[nextRow, nextCol].CompareTag("Empty"))
        {
            currentRow = nextRow;
            currentCol = nextCol;
            nextRow += dRow;
            nextCol += dCol;
        }

        //if cannot move, return 
        if (currentRow == row && currentCol == col) return;

        //create empty block at current position
        GameObject emptyBlock = Instantiate(mapData.emptyBlockPrefab, mapData.mapContainer);
        if(emptyBlock.TryGetComponent<RectTransform>(out var rt))
        {
            rt.sizeDelta = new Vector2(mapData.tileSize, mapData.tileSize);
            rt.anchoredPosition = mapData.GetBlockPosition(col, row);
        }
        emptyBlock.tag = "Empty";
        mapData.blockGrid[row, col] = emptyBlock;
        mapData.blockGrid[currentRow, currentCol] = gameObject;
        row = currentRow;
        col = currentCol;
        transform.SetAsLastSibling();
        Vector2 targetPos = mapData.GetBlockPosition(col, row);
        StartCoroutine(MoveToPosition(targetPos));
    }

    private System.Collections.IEnumerator MoveToPosition(Vector2 targetPos)
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
    }
}
