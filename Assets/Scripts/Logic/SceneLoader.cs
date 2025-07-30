using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Slider loadingBar;
    public TextMeshProUGUI loadingProgress;
    void Start ()
    {
        StartCoroutine(LoadAsynchronously(1));
    }

    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = progress;
            loadingProgress.text = $"{progress * 100f}%";
            yield return null;
        }
    }
}
