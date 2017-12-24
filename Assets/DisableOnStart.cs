using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DisableOnStart : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);
    }
}
