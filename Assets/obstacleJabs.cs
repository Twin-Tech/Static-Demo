using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleJabs : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {

        if (other.name.Contains("malcolm"))
        {
            scoring sc = GameObject.Find("Scoring").GetComponent<scoring>();
            sc.setGesture(4);
        }
    }

    void OnTriggerStay(Collider other)
    {
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("malcolm"))
        {
            scoring sc = GameObject.Find("Scoring").GetComponent<scoring>();
            sc.setGesture(1);
        }
    }
}
