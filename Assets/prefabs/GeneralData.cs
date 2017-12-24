using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;

public class GeneralData
{
    public bool gestureTrue;
    public string totalTime;
    public float accuracy;
    public string gestureName;
    public GeneralData()
    {
        gestureTrue = false;
        totalTime = "";
        accuracy = 0;
        gestureName = null;
    }
}
