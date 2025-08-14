using UnityEngine;

public class LoseSceneController : MonoBehaviour
{
    [SerializeField] private PersistentData persistentData;
    [SerializeField] private GameObject outOfMoveTitle;
    [SerializeField] private GameObject wrongColorTitle;

    void Start()
    {
        wrongColorTitle.SetActive(false);
        outOfMoveTitle.SetActive(false);
        if(persistentData.loseReason == PersistentData.LoseReason.WrongColor)
        {
            wrongColorTitle.SetActive(true);
        } else if (persistentData.loseReason == PersistentData.LoseReason.OutOfMove)
        {
            outOfMoveTitle.SetActive(true);
        }
    }
}
