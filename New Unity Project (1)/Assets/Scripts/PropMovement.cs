using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropMovement : Photon.MonoBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    public Transform lookAt;

    public Camera cam;

    public bool isGrounded;
    public Rigidbody rb;

    void Start()
    {
        cam.enabled = true;
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == ("Ground") && isGrounded == false)
        {
            isGrounded = true;
        }
    }

    public void FixedUpdate()
    {
        
        
        if (Input.GetKey(KeyCode.W))
        {
            transform.position -= transform.forward * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += transform.right * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position -= transform.right * Time.deltaTime * speed;
        }
        

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector3(0, 2, 0) * jumpSpeed, ForceMode.Impulse);
            isGrounded = false;
        }
        
    }

    void Update()
    {
  
    }
        void LateUpdate()
    {
       transform.LookAt(new Vector3(lookAt.position.x, transform.position.y, lookAt.position.z));
    }
}
