using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HintController : MonoBehaviour
{
    public GameObject handPrefab;
    public ReadMapData mapData;
    public AdsController adsController;
    [SerializeField] private PersistentData persistentData;
    private GameObject currentHand;
    public RectTransform mapContainer;
    public List<HintData> hintDataList = new List<HintData>();

    private List<HintData> currentLevelHints;
    private int currentHintIndex = 0;
    private bool isAdsLoaded = false;

    void LoadHints()
    {
        string[] lines = Resources.Load<TextAsset>("Data/HintData").text.Split('\n');
        int currentLevel = -1;

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');
            if (values.Length < 4) continue;

            if (!string.IsNullOrWhiteSpace(values[0]))
            {
                currentLevel = int.Parse(values[0]);
            }

            if (currentLevel == -1) continue;

            if (!int.TryParse(values[1], out int nX)) continue;
            if (!int.TryParse(values[2], out int nY)) continue;

            HintData hintData = new()
            {
                N_Level = currentLevel,
                N_X = nX,
                N_Y = nY,
                HintMoveType = values[3].Trim()
            };
            hintDataList.Add(hintData);
        }
    }

    public List<HintData> GetHintsForLevel(int level)
    {
        if (hintDataList.Count == 0)
            LoadHints();

        return hintDataList.FindAll(hint => hint.N_Level == level);
    }

    public void OnHintButton()
    {
        int currentLevel = persistentData.TargetLevel;
        mapData.ReadData(currentLevel);
        currentLevelHints = GetHintsForLevel(currentLevel);
        currentHintIndex = 0;
        if (adsController != null)
        {
            isAdsLoaded = true;
            adsController.ShowInterstitialAd();
        }
        if (isAdsLoaded)
        {
            ShowCurrentHint();
        }
    }

    private void ShowCurrentHint()
    {
        if (currentLevelHints == null || currentHintIndex >= currentLevelHints.Count)
            return;

        HintData hint = currentLevelHints[currentHintIndex];
        var block = mapData.blockGrid[hint.N_X, hint.N_Y];
        if (block == null) return;

        if (currentHand != null)
        {
            currentHand.SetActive(false);
        }

        Vector2 handPosition = mapData.GetBlockPosition(hint.N_X, hint.N_Y);
        currentHand = Instantiate(handPrefab, mapContainer);
        currentHand.SetActive(true);
        currentHand.tag = "Hand";
        RectTransform handRect = currentHand.GetComponent<RectTransform>();
        handRect.localPosition = handPosition + Vector2.down * 70f;
        handRect.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        StartCoroutine(AnimateHand(currentHand, hint.HintMoveType));
    }

    public void OnPlayerMove(int fromX, int fromY)
    {
        if (currentLevelHints == null || currentHintIndex >= currentLevelHints.Count)
            return;

        HintData hint = currentLevelHints[currentHintIndex];

        if (hint.N_X == fromX && hint.N_Y == fromY)
        {
            currentHintIndex++;
            ShowCurrentHint();
        }
    }

    private System.Collections.IEnumerator AnimateHand(GameObject hand, string moveType)
    {
        if (hand == null) yield break;
        RectTransform handRect = hand.GetComponent<RectTransform>();
        if (handRect == null) yield break;

        Vector3 startPos = handRect.localPosition;
        Vector3 targetPos = startPos;
        float moveDistance = 70f;
        float duration = 0.5f;

        switch (moveType)
        {
            case "Left2Right": targetPos += Vector3.right * moveDistance; break;
            case "Right2Left": targetPos += Vector3.left * moveDistance; break;
            case "Top2Bot": targetPos += Vector3.down * moveDistance; break;
            case "Bot2Top": targetPos += Vector3.up * moveDistance; break;
        }

        while (hand != null && handRect != null)
        {
            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;
                handRect.localPosition = Vector3.Lerp(startPos, targetPos, t / duration);
                yield return null;
            }
            handRect.localPosition = startPos;

            yield return new WaitForSeconds(0.5f);
        }

    }
}
