using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityControl : MonoBehaviour
{
    private Health health;
    private Tracking tracking;
    public float velocity;


    void Start()
    {
        health = GetComponent<Health>();
        tracking = GetComponent<Tracking>();
        velocity = 1f;
    }

    void Update()
    {

        if (health.health > 0)
        {
            velocity = tracking.velocity / 10;
            //if (Input.GetKey(KeyCode.Q)) { velocity = 20f; }
            //if (Input.GetKey(KeyCode.W)) { velocity = 40f; }
            //if (Input.GetKey(KeyCode.E)) { velocity = 60f; }
            //if (Input.GetKey(KeyCode.R)) { velocity = 80f; }

            transform.Translate(Vector3.forward * Time.deltaTime * velocity);
        }
        else
        {
            velocity = 0f;
            if (transform.position.y > 100)
            {
                transform.Translate(0, -100 * Time.deltaTime, 0);
            }
        }       
    }
}
