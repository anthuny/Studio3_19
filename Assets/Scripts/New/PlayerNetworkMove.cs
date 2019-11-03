using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkMove : NetworkBehaviour
{
    Transform trans; // Used to cache the transform
    Rigidbody body; // Used to cache the RigidBody
    float hInput;
    float vInput;
    public float speed;
    Camera playerCam;

    public override void OnStartLocalPlayer()
    {
        playerCam = GetComponent<NetworkPlayer>().playerCam;
    }

    void Awake()
    {
        trans = transform;
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //If this is NOT the local player of this script
        if (!isLocalPlayer)
        {
            return;
        }

        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        //If this is NOT the local player of this script
        if (!isLocalPlayer)
        {
            return;
        }

        // Remove unwanted forces resulting from collisions
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;

        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if (screenRect.Contains(Input.mousePosition))
        {
            //Rotation detection from mouse
            var ray = playerCam.ScreenPointToRay(Input.mousePosition);

            var mousePosition = new Vector3(ray.origin.x, transform.position.y, ray.origin.z);

            transform.LookAt(mousePosition);
        }

        //Vertical Input
        if (vInput > 0)
        {
            transform.position += Vector3.forward * Time.deltaTime * speed;
        }
        else if (vInput < 0)
        {
            transform.position += Vector3.back * Time.deltaTime * speed;
        }

        //Horizontal Input
        if (hInput > 0)
        {
            transform.position += Vector3.right * Time.deltaTime * speed;
        }
        else if (hInput < 0)
        {
            transform.position += Vector3.left * Time.deltaTime * speed;
        }
    }
}
