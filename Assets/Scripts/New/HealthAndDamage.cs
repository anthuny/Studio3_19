using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HealthAndDamage : NetworkBehaviour
{
    [SyncVar(hook = "OnHealthChanged")]
    public int health;
    [SyncVar] Vector3 respawnPos;
    Camera playerCam;
    public Vector3 camPos;

    public override void OnStartLocalPlayer()
    {
        playerCam = GetComponent<NetworkPlayer>().playerCam;
        CmdSetHealth(100);
    }

    [Command]
    void CmdSetHealth(int newHealth)
    {
        health = newHealth;
    }

    [Server]
    public void TakeDamage(GameObject fromPlayer)
    {
        health = Mathf.Max(health - 10, 0);

        if (health <= 0)
        {
            DoDeath();
            fromPlayer.GetComponent<PlayerShoot>().AddScore();
        }
    }

    [Server]
    void DoDeath()
    {
        respawnPos = GetRandomSpawnPoint();
        RpcHandlePlayerDeath();
    }

    Vector3 GetRandomSpawnPoint()
    {
        var spawnPoints = NetworkManager.singleton.startPositions;
        return spawnPoints[Random.Range(0, spawnPoints.Count)].position;
    }

    [Client]
    IEnumerator HandlePlayerDeath()
    {
        setVisibleState(false);
        if (isLocalPlayer)
        {
            playerCam.transform.SetParent(null, true);
            transform.position = respawnPos;
        }

        yield return new WaitForSeconds(2f);
        Respawn();
    }

    [Client]
    void Respawn()
    {
        if (isLocalPlayer)
        {
            playerCam.transform.SetParent(transform);
            playerCam.transform.localPosition = camPos;
            CmdSetHealth(100);
        }

        setVisibleState(true);
    }

    [Client]
    void setVisibleState(bool state)
    {
        GetComponent<Renderer>().enabled = state;
        transform.Find("LabelHolder").gameObject.SetActive(state);
    }

    [ClientRpc]
    void RpcHandlePlayerDeath()
    {
        StartCoroutine(HandlePlayerDeath());
    }


    void OnHealthChanged(int newHealth)
    {
        health = newHealth;

        if (health < 100)
        {
            StartCoroutine(ShowHitEffect());
        }

        if (isLocalPlayer)
        {
            HUD.instance.DisplayHealth(health);
        }
    }

    IEnumerator ShowHitEffect()
    {
        var material = GetComponent<Renderer>().material;
        Color savedColor = material.color;
        material.color = Color.yellow;
        yield return new WaitForSeconds(0.4f);
        material.color = savedColor;
    }
}