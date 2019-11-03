using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] float fireRate = 0.5f;
    LineRenderer laser;
    private float nextFireTime;
    int score;
    public float shotVisibilyDur;

    public override void OnStartClient()
    {
        //DISABLE the firing ability so not every player has this
        laser = transform.Find("Laser").GetComponent<LineRenderer>();
        laser.enabled = false;
        enabled = false;
    }

    public override void OnStartLocalPlayer()
    {
        //ENABLE the firing ability ONLY to work on EACH player
        enabled = true;
    }

    void Update()
    {
        //shooting cooldown functionality
        if (Input.GetAxis("Fire1") != 0 && canFire)
        {
            nextFireTime = Time.time + fireRate;
            Fire();
        }
    }

    void Fire()
    {
        //Create a ray
        Ray ray = new Ray(laser.transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10f))
        {
            var hitPlayer = hit.collider.gameObject;
            CmdDoShotSomeone(gameObject, hitPlayer);
        }

        StartCoroutine(ShowLaser());
        CmdShowLaser();
    }

    [Command]
    void CmdDoShotSomeone(GameObject fromPlayer, GameObject hitPlayer)
    {
        hitPlayer.GetComponent<HealthAndDamage>().TakeDamage(fromPlayer);
    }
    [Command]
    void CmdShowLaser()
    {
        RpcShowLaser();
    }

    // In the RpcShowLaser function, the first thing we need to do is determine
    // if this is running on our local player object or the player object
    // belonging to another client.
    // Basically, if isLocalPlayer is true, that means we fired our laser, and 
    // as we have already displayed the laser effect on our PC during the Fire() function, 
    // we don’t need to do it again and can just exit the function.
    // However if isLocalPlayer is false, then that means that someone else fired their
    // laser, and they are telling us that we need to display the laser effect on our
    // copy of their player object, so we start the ShowLaser coroutine.
    [ClientRpc]
    void RpcShowLaser()
    {
        if (isLocalPlayer)
        {
            return;
        }
        StartCoroutine(ShowLaser());
    }

    IEnumerator ShowLaser()
    {
        laser.enabled = true;
        yield return new WaitForSeconds(shotVisibilyDur);
        laser.enabled = false;
    }

    public bool canFire
    {
        get { return Time.time >= nextFireTime; }
    }

    [Server]
    public void AddScore()
    {
        RpcAddScore();
    }

    [ClientRpc]
    void RpcAddScore()
    {
        if (isLocalPlayer)
        {
            score += 5;
            HUD.instance.DisplayScore(score);

        }
    }
}