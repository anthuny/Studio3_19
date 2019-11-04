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

    //Camera
    Camera playerCam;
    public float rotationSpeed = 3;
    public Transform target, player;
    float mouseX, mouseY;

    public override void OnStartLocalPlayer()
    {
        playerCam = GetComponent<NetworkPlayer>().playerCam;
    }

    void Awake()
    {
        trans = transform;
        body = GetComponent<Rigidbody>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
        //if (screenRect.Contains(Input.mousePosition))
        {
            //Rotation detection from mouse
            //var ray = playerCam.ScreenPointToRay(Input.mousePosition);

            //var mousePosition = new Vector3(ray.origin.x, transform.position.y, ray.origin.z);

            //transform.LookAt(mousePosition);
        }

        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -80, 50);

        transform.LookAt(target);

        target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        transform.rotation = Quaternion.Euler(0, mouseX, 0);

        //Vertical Input
        if (vInput > 0)
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }
        else if (vInput < 0)
        {
            transform.position += -transform.forward * Time.deltaTime * speed;
        }

        //Horizontal Input
        if (hInput > 0)
        {
            transform.position += transform.right * Time.deltaTime * speed;
        }
        else if (hInput < 0)
        {
            transform.position += -transform.right * Time.deltaTime * speed;
        }
    }
}
