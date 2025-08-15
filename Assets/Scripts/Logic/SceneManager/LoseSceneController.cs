using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseSceneController : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI moveText;
    public GameObject targetColor;
    public ReadLevelData levelData;
    public Animator sceneTransition;

    [SerializeField] private PersistentData persistentData;
    [SerializeField] private GameObject outOfMoveTitle;
    [SerializeField] private GameObject wrongColorTitle;

    void Start()
    {
        if (sceneTransition != null)
        {
            sceneTransition.SetTrigger("LoseSceneTransition");
        }
        DisplayLoseTitle();
        DisplayLoseData();
    }

    public void Retry()
    {
        SceneManager.LoadScene("StartScene");
    }

    void DisplayLoseTitle()
    {
        wrongColorTitle.SetActive(false);
        outOfMoveTitle.SetActive(false);
        if (persistentData.loseReason == PersistentData.LoseReason.WrongColor)
        {
            wrongColorTitle.SetActive(true);
        }
        else if (persistentData.loseReason == PersistentData.LoseReason.OutOfMove)
        {
            outOfMoveTitle.SetActive(true);
        }
    }

    void DisplayLoseData()
    {
        levelText.text = persistentData.TargetLevel.ToString();
        moveText.text = $"{persistentData.MoveLeft}/{persistentData.InitialMoveLimited}";
        targetColor.GetComponent<Image>().sprite = levelData.GetColorSprite(persistentData.TargetColor);
    }
}
