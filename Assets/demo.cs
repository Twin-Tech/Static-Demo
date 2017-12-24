using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demo : MonoBehaviour {

    public int frames;
    int leftctr;
	// Use this for initialization
	void Start () {
        frames = 0;
        leftctr = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        BodyProperties bp = GameObject.Find("skeleton").GetComponentInChildren<BodyProperties>();
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 movement = new Vector3(0.0f, 0.0f, -0.05f);
        frames++;
        if (bp != null && bp.startGame)
        {
            if (bp.GD.gestureName == "highknees")
            {
                if (bp.GD.gestureTrue)
                    transform.Translate(movement,Space.Self);
              //  else
                //    rb.AddForce(movement * 0);
            }
            else if (bp.GD.gestureName == "jumpingjack")
            {
                if (bp.GD.gestureTrue)
                    transform.Translate(movement*2, Space.Self);
            }
            if (bp.rotation > 35 && frames > 60)
            {
                if (bp.rotationDirection == "Right")
                {
                    frames=0;
                    transform.Rotate(Vector3.up, 90);
                    GameObject.Find("Main Camera").transform.Rotate(Vector3.up, 90, Space.World);
                    CameraController cc = GameObject.Find("Main Camera").GetComponent<CameraController>();
                    float temp;
                    temp = cc.offset.x;
                    cc.offset.x = cc.offset.z;
                    cc.offset.z = -temp;
                }
                else
                {
                    frames = 0;
                    transform.Rotate(Vector3.down,90);
                    GameObject.Find("Main Camera").transform.Rotate(Vector3.down, 90, Space.World);
                    CameraController cc = GameObject.Find("Main Camera").GetComponent<CameraController>();
                    float temp;
                    leftctr++;
                    temp = cc.offset.x;
                    cc.offset.x = -cc.offset.z;
                    cc.offset.z = temp;
                    /*if (leftctr == 2)
                    {
                        cc.offset.x = -cc.offset.x;
                        leftctr = 0;
                    }*/
                }
            }
        }
	}
}
