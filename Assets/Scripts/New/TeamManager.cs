using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class TeamManager : NetworkBehaviour
{
    public static TeamManager instance;
    static int playerCount = 0;

    void Awake()
    {
        //Making sure there is only every one instance of the TeamManager objject
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
    }

    [Server]
    public static void SetPlayerTeam(GameObject newPlayer)
    {
        var player = newPlayer.GetComponent<NetworkPlayer>();
        player.teamNumber = (int)Mathf.Repeat(playerCount, 2);
        playerCount++;
    }


}