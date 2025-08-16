using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSquare : MonoBehaviour
{
    public BlockController blockController;
    public AdsController adsController;
    public Transform[] blockImages;                 
    public Animator sceneTransition;
    private string[] colors = { "P", "R", "Y", "G", "B" }; 
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

        StartCoroutine(AnimateLoading());
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

    System.Collections.IEnumerator AnimateLoading()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            currentColorIndex = (currentColorIndex + 1) % colors.Length;
            string nextColor = colors[currentColorIndex];
            foreach (var block in blockImages)
            {
                SpawnBlock(nextColor, block, true);
                yield return new WaitForSeconds(0.3f); 
            }
            if (currentColorIndex >= 4)
            {
                SceneManager.LoadScene("StartScene");
            }
        }
    }
}
