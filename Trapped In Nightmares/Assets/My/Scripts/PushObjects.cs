using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PushObjects : MonoBehaviour {
    
    public float ObMass = 300;
    public float PushAtMass = 100;
    public float PushingTime;
    public float ForceToPush;
    Rigidbody rb;
    public float vel;
    Vector3 dir;

    Vector3 lastPos ;
    float _PushingTime = 0;

 

void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) return;

        rb.mass = ObMass;
    }

    bool IsMoving()
    {
        if (rb.velocity.magnitude > 0.06f)
        {
            return true;
        }
        return false;

    }

     private void Update()
    {
        vel = rb.velocity.magnitude;
        if (Input.GetKeyUp(KeyCode.F))
        {
            rb.isKinematic = true;
        }

        if (!rb.isKinematic)
        {
            _PushingTime += Time.deltaTime;
            if (_PushingTime >= PushingTime)
            {
                _PushingTime = PushingTime;
            }

            rb.mass = Mathf.Lerp(ObMass, PushAtMass, _PushingTime / PushingTime);
            rb.AddForce(dir * ForceToPush, ForceMode.Force);
        }
        else
        {
            rb.mass = ObMass;
            _PushingTime = 0;
           
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            if (Input.GetKey(KeyCode.F))
            {
                rb.isKinematic = false;
                dir = collision.contacts[0].point - transform.position;
                // We then get the opposite (-Vector3) and normalize it
                dir = -dir.normalized;
              
               
            }
        }

    }

}
