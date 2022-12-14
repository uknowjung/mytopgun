using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimVertical : MonoBehaviour
{
    private UnityEngine.UI.Text txt;

    private AttackControl attackControl;

    // Start is called before the first frame update
    void Start()
    {

        txt = GetComponent<UnityEngine.UI.Text>();

        attackControl = GameObject.FindGameObjectWithTag("Player").GetComponent<AttackControl>();

    }

    // Update is called once per frame
    void Update()
    {
        if(attackControl.attack == 1)
        {
            txt.text = "I\nI";
        }
        else
        {
            txt.text = "";
        }

    }
}
