using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnection : NetworkBehaviour
{
    public GameObject playerUnitPrefab;

    public int currentPlayers = 0;

    // SyncVars are variables where if their value changes on the SERVER, then all clients
    // are automatically informed of the new value.
    [SyncVar(hook = "OnPlayerNameChanged")]

    public string playerName = "Anonymous";

    // Start is called before the first frame update
    void Start()
    {
        // Is this actually my own local player?
        if (!isLocalPlayer)
        {
            // This object belongs to another player
            return;
        }

        // Spawn my Unit
        CmdSpawnMyUnit();

        // Set name my the Unit
        SetNameOfPlayerUnit();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
    }

    void SetNameOfPlayerUnit()
    {
        currentPlayers++;
        string n = "Player " + currentPlayers;

        Debug.Log("sending server a request to change name to " + n);
        CmdChangePlayerName(n);
    }
    void OnPlayerNameChanged(string newName)
    {
        Debug.Log("OnPlayerNameChanged: OldName: " + playerName + "    NewName: " + newName);

        playerName = newName;
        gameObject.name = "PlayerConnection [" + newName + "]";
    }

    ////////////////////////// COMMANDS
    // Commands are special functions that ONLY get exectued on the server.


    [Command]
    void CmdSpawnMyUnit()
    {
        Debug.Log("PlayerConnection::Start -- spawning my own personal Unit");

        // We are garanteed to be on the server right now.
        GameObject go = Instantiate(playerUnitPrefab);

        // Now that the object exists on the server, propagate it to all
        // the clients (and also wire up the NetworkIdentity)
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);

    }

    [Command]
    void CmdChangePlayerName(string n)
    {
        Debug.Log("CmdChangePlayerName: " + n);

        playerName = n;
    }

    ////////////////////////// RPC
    // RPCs are special functions that ONLY get executed on the clients.

        /*
    [ClientRpc]
    void RpcChangePlayerName(string n)
    {
        Debug.Log("RpcChangePlayerName: We were asked to change the player name on a particular PlayerConnection: " + n);
        playerName = n; 
    }
    */
}
