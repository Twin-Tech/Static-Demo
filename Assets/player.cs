﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using System;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    public Material pickup;
    public GameObject BodySourceManager;
    public GameObject ground;
    //public Text[] TextArr;
    public static float XHumanScalingFactor = 1;
    public static float YHumanScalingFactor = 1;
    public GameObject startCircle;
    public Kinect.Vector4 floorClipPlane;
    public Text counttext;
    public int count = 0;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    void Start()
    {

    }


    void FixedUpdate()
    {

        if (BodySourceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        //floorClipPlane = _BodyManager.GetFloorClipPlane();

        //Vector3 InNormal = new Vector3(floorClipPlane.X, floorClipPlane.Y, floorClipPlane.Z);
        //float floorDistance = floorClipPlane.W;

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        int i = 0;
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
            i++;
        }
        BodyProperties bp = GameObject.Find("skeleton").GetComponentInChildren<BodyProperties>();
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 movement = new Vector3(0.0f, 0.0f, 1.0f);
        if (bp.startGame)
        {
            if (bp.GD.gestureName == "highknees")
            {
                if (bp.GD.gestureTrue)
                    rb.AddForce(movement * 75);
            }
            else if (bp.GD.gestureName == "jumpingjack")
            {
                if (bp.GD.accuracy > 70)
                    rb.AddForce(movement * 75);
            }
        }
        //transform.Translate(Vector3.forward * 0);
    }

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        //body.transform.position = new Vector3(0.0f, 10.0f, 0.0f);

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            jointObj.name = "player" + jt.ToString();
            //jointObj.gameObject.tag = jt.ToString();
            //SphereCollider scj = jointObj.GetComponent<SphereCollider>();
            //scj.isTrigger = false;
            //if(jt.ToString().Equals("SpineBase"))
              //  jointObj.AddComponent<Obstacle>();
            /*if (jt == Kinect.JointType.HandLeft || jt == Kinect.JointType.HandRight)
            {
                scj.radius = 1.5f;
            }*/

            Rigidbody rbj = jointObj.AddComponent<Rigidbody>() as Rigidbody;
            rbj.useGravity = false;

            // jointObj.AddComponent<colloideDetect>();

            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.material = pickup;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;

            jointObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        body.transform.parent = transform;

        //body.AddComponent<BodyProperties>();
        Rigidbody rb = body.AddComponent<Rigidbody>();

        rb.useGravity = false;

        return body;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        count++;
        Kinect.Joint s = body.Joints[Kinect.JointType.WristLeft];
        Kinect.Joint t = body.Joints[Kinect.JointType.WristRight];

        Kinect.Joint bodyJoint = body.Joints[Kinect.JointType.SpineBase];
        bodyObject.transform.position = GetVector3FromJoint(bodyJoint);


        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }
            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.position = GetVector3FromJoint(sourceJoint);



            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.position);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.startColor = GetColorForState(sourceJoint.TrackingState);
                lr.endColor = GetColorForState(targetJoint.Value.TrackingState);
            }
            else
            {
                lr.enabled = false;
            }
        }
    }

    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;

            case Kinect.TrackingState.Inferred:
                return Color.red;
            default:
                return Color.black;
        }
    }

    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * XHumanScalingFactor, joint.Position.Y * YHumanScalingFactor, joint.Position.Z);
    }

}
