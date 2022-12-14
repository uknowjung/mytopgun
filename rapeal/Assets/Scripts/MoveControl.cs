using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    private Health health;
    private Tracking tracking;
    private float turn;
    private float updown;
    private int isVelocityMode;
    //private float up = 40f;
    //private float down = 40f;
    //private float left = 40f;
    //private float right = 40f;
    private float rotation;

    void Start()
    {
        health = GetComponent<Health>();
        tracking = GetComponent<Tracking>();
    }

    void Update()
    {
        if (health.health > 0)
        {
            turn = tracking.turn;
            updown = tracking.updown;
            isVelocityMode = tracking.velocityHandleBox;

            rotation = transform.localEulerAngles.z;

            if (isVelocityMode == 0)
            {
                transform.Rotate(new Vector3(-1f * updown * Time.deltaTime, 0, 0), Space.Self);
                transform.Rotate(new Vector3(0, 0, -1f * turn * Time.deltaTime), Space.Self);

                //if (Input.GetKey(KeyCode.UpArrow)) { transform.Rotate(new Vector3(-1f * up * Time.deltaTime, 0, 0), Space.Self); }
                //if (Input.GetKey(KeyCode.DownArrow)) { transform.Rotate(new Vector3(down * Time.deltaTime, 0, 0), Space.Self); }
                //if (Input.GetKey(KeyCode.LeftArrow)) { transform.Rotate(new Vector3(0, 0, left * Time.deltaTime), Space.Self); }
                //if (Input.GetKey(KeyCode.RightArrow)) { transform.Rotate(new Vector3(0, 0, -1f * right * Time.deltaTime), Space.Self); }

                if (rotation > 15 && rotation <= 180) { transform.Translate(10f * Vector3.left * Time.deltaTime, Space.Self); }
                else if (rotation > 180 && rotation < 345) { transform.Translate(10f * Vector3.right * Time.deltaTime, Space.Self); }
            }
        }
    }
}
