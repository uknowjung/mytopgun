using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityText : MonoBehaviour
{
    private UnityEngine.UI.Text velocity;

    private VelocityControl velocityControl;

    // Start is called before the first frame update
    void Start()
    {

        velocity = GetComponent<UnityEngine.UI.Text>();

        velocityControl = GameObject.FindGameObjectWithTag("Player").GetComponent<VelocityControl>();

    }

    // Update is called once per frame
    void Update()
    {
        int k = (int)velocityControl.velocity * 10;

        string str = k.ToString();

        velocity.text = str + " km/h";
    }
}
