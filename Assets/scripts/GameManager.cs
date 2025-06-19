using NUnit.Framework;
using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance{ get; private set; }

    public event OnClick onClicked;
    public delegate void OnClick(float x, float y , PlayerType playerType);


    public event ONGameStarted onGameStarted;
    public delegate void ONGameStarted();


    public event OnCurrentPlayableTypeChanged onCurrentPlayableTypeChanged;
    public delegate void OnCurrentPlayableTypeChanged();
    private PlayerType localplayerType;
    private NetworkVariable<PlayerType> currentPlayablePlayerType = new NetworkVariable<PlayerType>();
    public PlayerType[,] playerTypeArray;
    public enum PlayerType
    {
        none, 
        Cross,
        Circle
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        playerTypeArray = new PlayerType[3,3]; 
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        onGameStarted.Invoke();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("Network Spawned " + NetworkManager.Singleton.LocalClientId);
        if(NetworkManager.Singleton.LocalClientId == 0)
        {
            localplayerType = PlayerType.Cross;
        }
        else
        {
            localplayerType = PlayerType.Circle;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkManager_ClientConnected;
        }

        currentPlayablePlayerType.OnValueChanged += (PlayerType OldplayerType, PlayerType NewplayerType)=> { onCurrentPlayableTypeChanged.Invoke(); };
    }


    public void OnNetworkManager_ClientConnected(ulong id)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            currentPlayablePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void ClickedPositionRpc(float x , float y , PlayerType playerType , int index)
    {
        //Debug.Log("Position "+x +" , "+y);

        if (playerType != currentPlayablePlayerType.Value)
        {
            return;
        }
        int indexRow = index / 3;
        int IndexColumn = index % 3;
        if (playerTypeArray[indexRow, IndexColumn] != PlayerType.none)
        {
            //block is already occupied 
            return;
        }

        playerTypeArray[indexRow, IndexColumn] = playerType;
        onClicked.Invoke(x, y , playerType);

        switch (currentPlayablePlayerType.Value)
        {
            default:
            case PlayerType.Cross:
                currentPlayablePlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                currentPlayablePlayerType.Value = PlayerType.Cross;
                break;
        }
        //Debug.Log("OnGameManager Current player " + GameManager.instance.GetCurrentPlayablePlayerType());
        //TriggerOnCurrentPlayableTypeChangedRpc();
        TestWinner();
    }

    public PlayerType GetLocalPlayerType()
    {
        return localplayerType;
    }
    public PlayerType GetCurrentPlayablePlayerType()
    {
        return currentPlayablePlayerType.Value;
    }

    bool TestWinnerLine(PlayerType playerA , PlayerType playerB , PlayerType playerC)
    {
        return (playerA != PlayerType.none && playerA == playerB && playerB == playerC); 
    }
    void TestWinner()
    {
        if (TestWinnerLine(playerTypeArray[0,0] , playerTypeArray[0 , 1] , playerTypeArray[0 , 2]))
        {
            Debug.Log("Winner");
            currentPlayablePlayerType.Value = PlayerType.none;
        }
    }
}
