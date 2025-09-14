using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSquare : MonoBehaviour
{
    public BlockController blockController;
    public AdsController adsController;
    public Transform[] blockImages;                 
    public Animator sceneTransition;
    private string[] colors = { "P", "R", "Y", "G" }; 
    private int currentColorIndex = 0;

    void Start()
    {
        if (sceneTransition != null)
        {
            sceneTransition.SetTrigger("LoadingSceneTransition");
        }

        foreach (var block in blockImages)
        {
            SpawnBlock("P", block, false);
        }

        StartCoroutine(LoadSceneAsync("StartScene"));
    }

    void SpawnBlock(string color, Transform images, bool isScaleUp)
    {
        GameObject prefab = blockController.GetBlockPrefab(color);
        if (prefab != null)
        {
            foreach (Transform child in images)
            {
                Destroy(child.gameObject);
            }
            var block = Instantiate(prefab, images);
            if (isScaleUp)
            {
                block.transform.localScale = Vector3.one;
                StartCoroutine(ScaleUp(block.transform));
            } else
            {
                block.transform.localScale = Vector3.one;
            }
        }
    }

    System.Collections.IEnumerator ScaleUp(Transform target)
    {
        Vector3 startScale = new(0.8f, 0.8f, 0.8f);
        Vector3 endScale = Vector3.one; 
        float t = 0f;
        float duration = 0.3f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            target.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        target.localScale = endScale;
    }

    System.Collections.IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            int colorIndex = Mathf.FloorToInt(progress * (colors.Length - 1));
            if (colorIndex != currentColorIndex && colorIndex < colors.Length)
            {
                currentColorIndex = colorIndex;
                string nextColor = colors[currentColorIndex];
                foreach (var block in blockImages)
                {
                    SpawnBlock(nextColor, block, true);
                    yield return new WaitForSeconds(0.1f);
                }
            }

            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
