using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HealthPack : NetworkBehaviour
{
    [SyncVar(hook = "OnVisibleChanged")]
    bool visible;
    public int healthPackCD;
    public int healthIncreaseAmo;

    public override void OnStartServer()
    {
        visible = true;
    }

    public override void OnStartClient()
    {
        OnVisibleChanged(visible);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(handlePickup());
        other.GetComponent<HealthAndDamage>().health += healthIncreaseAmo;
    }

    [Server]
    IEnumerator handlePickup()
    {
        visible = false;
        yield return new WaitForSeconds(healthPackCD);
        visible = true;
    }

    void OnVisibleChanged(bool newValue)
    {
        GetComponent<Renderer>().enabled = newValue;
        GetComponent<Collider>().enabled = newValue;
    }
}