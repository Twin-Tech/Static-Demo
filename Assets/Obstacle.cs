using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;

public class Obstacle : MonoBehaviour 
{


    void OnTriggerEnter(Collider other)
    {

        if (other.name.Contains("malcolm"))
        {
            BodyProperties BP = GameObject.Find("skeleton").GetComponentInChildren<BodyProperties>();
            if(BP != null)
            {
                BP.GD.accuracy = 0;
            }
            scoring sc = GameObject.Find("Scoring").GetComponent<scoring>();
            sc.setGesture(2);
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
