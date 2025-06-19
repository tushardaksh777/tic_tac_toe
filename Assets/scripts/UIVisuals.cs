using UnityEngine;

public class UIVisuals : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject CrossArrow;
    public GameObject CrossYOU;

    public GameObject CircleArrow;
    public GameObject CircleYOU;


    private void Awake()
    {
        CrossArrow.SetActive(false);
        CrossYOU.SetActive(false);

        CircleArrow.SetActive(false);
        CircleYOU.SetActive(false);
    }

    private void Start()
    {
        GameManager.instance.onGameStarted += GameManager_OnGameStarted;
        GameManager.instance.onCurrentPlayableTypeChanged += GameManager_OnCurrentPlayableTypeChanged;
    }
    private void UpdateArrow()
    {
        if (GameManager.instance.GetCurrentPlayablePlayerType() == GameManager.PlayerType.Cross)
        {
            CrossArrow.SetActive(true);
            CircleArrow.SetActive(false);
        }
        else
        {
            CrossArrow.SetActive(false);
            CircleArrow.SetActive(true);
        }
    }
    private void GameManager_OnGameStarted()
    {
        if (GameManager.instance.GetLocalPlayerType() == GameManager.PlayerType.Cross)
        {
            CrossYOU.SetActive(true);
        }
        else
        {
            CircleYOU.SetActive(true);
        }
        UpdateArrow();
    }
    private void GameManager_OnCurrentPlayableTypeChanged()
    {
        Debug.Log("OnUI Visual Current player "+ GameManager.instance.GetCurrentPlayablePlayerType() );
        UpdateArrow();
    }
}
