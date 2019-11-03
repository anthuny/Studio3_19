using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour
{
    [SyncVar(hook = "OnPlayerIDChanged")] public string playerID;
    [SyncVar(hook = "OnTeamChanged")] public int teamNumber;
    public Camera playerCam;
    Transform labelHolder;
    private void Awake()
    {
        labelHolder = transform.Find("LabelHolder");

        //Set cameras for all players OFF
        playerCam = GetComponentInChildren<Camera>();
        playerCam.gameObject.SetActive(false);
    }

    [Command]
    void CmdSetPlayerID(string newID)
    {
        playerID = newID;
    }

    [Command]
    void CmdSetTeam(GameObject player)
    {
        TeamManager.SetPlayerTeam(gameObject);
    }

    public override void OnStartLocalPlayer()
    {
        string myPlayerID = string.Format("Player {0}", netId.Value - 1);
        CmdSetPlayerID(myPlayerID);

        //Set this player's camera ON
        playerCam.gameObject.SetActive(true);

        CmdSetTeam(gameObject);
    }

    public override void OnStartClient()
    {
        OnPlayerIDChanged(playerID);
        OnTeamChanged(teamNumber);
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            playerCam.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        }

        labelHolder.rotation = Quaternion.identity;
    }

    void OnPlayerIDChanged(string newValue)
    {
        playerID = newValue;
        var textMesh = labelHolder.Find("Label").GetComponent<TextMesh>();
        textMesh.text = newValue;

        //This is to allow the local player to have "Local" in it's name
        name = playerID;
        if (isLocalPlayer)
        {
            name += "Local";
        }
    }

    public void OnTeamChanged(int newTeamNumber)
    {
        teamNumber = newTeamNumber;
        GetComponent<Renderer>().material.color = teamNumber == 0 ? Color.red : Color.blue;
    }
}