using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class character : MonoBehaviour {
    public int frames;
    int leftctr;
    Animator Anim;
    int IdleHash;
    int MoveHash;
    int JumpHash;
    // Use this for initialization
    void Start ()
    {
        Anim = GetComponent<Animator>();
        frames = 0;
        leftctr = 0;
        IdleHash = Animator.StringToHash("Idle");
        MoveHash = Animator.StringToHash("Move");
        JumpHash = Animator.StringToHash("Jump");
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        BodyProperties bp = GameObject.Find("skeleton").GetComponentInChildren<BodyProperties>();
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 movement = new Vector3(0.0f, 0.0f, 0.02f);
        frames++;
        if (bp != null && bp.startGame)
        {
            if (bp.GD.gestureName == "highknees")
            {
                if (bp.GD.gestureTrue)
                {
                    transform.Translate(movement, Space.Self);
                    Anim.ResetTrigger(JumpHash);
                    Anim.ResetTrigger(IdleHash);
                    Anim.SetTrigger(MoveHash);
                }
                else
                {
                    //    rb.AddForce(movement * 0);
                    Anim.ResetTrigger(JumpHash);
                    Anim.ResetTrigger(MoveHash);
                    Anim.SetTrigger(IdleHash);
                }
            }
            else if (bp.GD.gestureName == "jumpingjack")
            {
                if (bp.GD.accuracy>75)
                {
                    transform.Translate(movement*2, Space.Self);
                    Anim.ResetTrigger(MoveHash);
                    Anim.ResetTrigger(IdleHash);
                    Anim.SetTrigger(JumpHash);
                }
                else
                {
                    Anim.ResetTrigger(JumpHash);
                    Anim.ResetTrigger(MoveHash);
                    Anim.SetTrigger(IdleHash);
                }
            }
            if (bp.rotation > 35 && frames > 60)
            {
                if (bp.rotationDirection == "Right")
                {
                    frames = 0;
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
                    transform.Rotate(Vector3.down, 90);
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
