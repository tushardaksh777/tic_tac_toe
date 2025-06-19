using Unity.Netcode;
using UnityEngine;

public class Control : MonoBehaviour
{

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    protected void OnMouseDown()
    {
        int index =  int.Parse(gameObject.name.Split("Block")[1]);
        GameManager.instance.ClickedPositionRpc(transform.position.x , transform.position.y , GameManager.instance.GetLocalPlayerType() , index);
    }
}
