using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracking : MonoBehaviour
{
    public UDPReceive udpReceive;
    //public GameObject[] handPoints;
    public int velocityHandleBox;
    public int attackMode;
    public int turn;
    public int updown;
    public int velocity;

    void Start()
    {
        udpReceive = GameObject.Find("UDPReceive").GetComponent<UDPReceive>();
    }

    void Update()
    {
        string data = udpReceive.data;

        if (data.Length > 100)
        {
            data = data.Remove(0, 1);
            data = data.Remove(data.Length - 1, 1);
            string[] points = data.Split(',');

            //for (int i = 0; i < 21; i++)
            //{
            //    //float x = 7 - float.Parse(points[i * 3]) / 100;
            //    //float y = float.Parse(points[i * 3 + 1]) / 100 - 2;
            //    //float z = float.Parse(points[i * 3 + 2]) / 100;

            //    //handPoints[i].transform.localPosition = new Vector3(x, y, z);
            //}
            velocityHandleBox = int.Parse(points[63]);
            attackMode = int.Parse(points[64]);
            turn = int.Parse(points[65]);
            updown = int.Parse(points[66]);
            velocity = int.Parse(points[67]);
        }
    }
}
