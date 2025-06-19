using Unity.Netcode;
using UnityEngine;

public class Visuals : NetworkBehaviour
{
    [SerializeField]
    private Transform CrossPrefab;
    [SerializeField]
    private Transform CirclePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.onClicked += GameManager_Onclicked;
    }
    
    private void GameManager_Onclicked(float x , float y , GameManager.PlayerType playerType)
    {
        SpawnObjectRpc(x, y , playerType);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(float x , float y , GameManager.PlayerType playerType)
    {
        Transform prefab;
        switch (playerType)
        {
            default:
            case GameManager.PlayerType.Cross:
                prefab = CrossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = CirclePrefab;
                break;

        }
        Transform spawnedObject = Instantiate(prefab  , new Vector3(x, y, 0f) , Quaternion.identity);
        spawnedObject.GetComponent<NetworkObject>().Spawn();
    }
}
