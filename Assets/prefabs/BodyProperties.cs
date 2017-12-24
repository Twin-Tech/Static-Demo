using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.UI;

public class BodyProperties : MonoBehaviour
{
    public Boolean startGame = false;
    public Boolean InitializeObj = false;
    public int countinst = 0;
    int approximation = 0;
    public float leftarmLength = 0.0f;
    public float rightarmLength = 0.0f;
    int NoOfInitializationFrame = 100;
    public bool createdagain = false;
    public float rightFootHeight = Single.PositiveInfinity;
    public float leftFootHeight = Single.PositiveInfinity;
    public Vector2 initialRotationVector;
    public float initialRotationZ;
    public string rotationDirection;
    public GeneralData GD;
    private SecretData SD;
    public double rotation;
    private bool firstread;
    List<GameObject> colls;
    // Use this for initialization
    void Start()
    {
        initialRotationZ = 0;
        rotation = 0;
        initialRotationVector.x = 0;
        initialRotationVector.y = 0;
        colls = new List<GameObject>();
        GD = new GeneralData();
        SD = new SecretData();
        GD.gestureName = "highknees";
        firstread = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startGame && GD.gestureName!=null)
        {
            if (!InitializeObj)
            {
                float leftArm = FindHandLength("left");
                float rightArm = FindHandLength("right");
                if (leftArm != -1.0f && rightArm != -1.0f)
                {
                    FindLegHeightFromGround("left");
                    FindLegHeightFromGround("right");
                    leftarmLength += FindHandLength("left");
                    rightarmLength += FindHandLength("right");
                    approximation++;      
                    if (approximation > NoOfInitializationFrame)
                    {
                        initialRotationZ = getShoulderZ();
                        initialRotationVector = getShoulderPosition();
                        countinst++;
                        InitializeObj = true;
                        leftarmLength /= NoOfInitializationFrame;
                        rightarmLength /= NoOfInitializationFrame;
                        CreateCoins();
                        SD.rate = Convert.ToInt32((0.75 * SD.totalFrames) / SD.totalInitial);
                    }
                }
            }
            if (checkCenter())
            {
                if (SD.InitialCollisions == SD.totalInitial && SD.timestarted == false)
                {
                    SD.timestarted = true;
                    SD.start = DateTime.Now;
                }
                if (InitializeObj)
                {
                    rotation = getRotation();
                    getRotationDirection();
                    Debug.Log(rotation + ":" + rotationDirection);
                }
                SD.noOfFrames++;
                checkGesture();
            }
        }
    }
    public bool checkCenter()
    {
        JointsProperties spine = GameObject.Find("SpineBase").GetComponent<JointsProperties>();
        Text display = GameObject.Find("Text 5").GetComponent<Text>();
        if (spine.position.x < -0.3)
        {
            display.text = "Move towards your right.";
            return false;
        }
        else if (spine.position.x > 0.3)
        {
            display.text = "Move towards your left.";
            return false;
        }
        else if (spine.position.z < 2.4)
        {
            display.text = "Move back";
            return false;
        }
        else if (spine.position.z > 2.6)
        {
            display.text = "Come forward";
            return false;
        }
        else
        {
            display.text = "";
            return true;
        }
    }
    public void checkGesture()
    {
        if (SD.noOfFrames>(SD.rate*SD.totalInitial))
        {
            if (SD.gestureType == "Dynamic")
            {
                if (SD.noOfFrames <= (SD.rate * SD.Noofcollisions))
                    GD.gestureTrue = true;
                else
                    GD.gestureTrue = false;
            }
        }
        GameObject statusBox = GameObject.Find("Text 1");
        Text status = statusBox.GetComponent<Text>();
        status.text = GD.gestureTrue.ToString();
    }
    public void getRotationDirection()
    {
        float curr = getShoulderZ();
        if (initialRotationZ - curr < 0)
        {
            rotationDirection = "Right";
        }
        else
            rotationDirection = "Left";
    }
    public double getRotation()
    {
        Vector2 currentShoulderPosition = getShoulderPosition();
        return Vector2.Angle(initialRotationVector, currentShoulderPosition);
    }
    public Vector2 getShoulderPosition()
    {
        GameObject shoulder = GameObject.Find("ShoulderRight");
        JointsProperties jpshoulder = shoulder.GetComponent<JointsProperties>();
        GameObject neck = GameObject.Find("SpineShoulder");
        JointsProperties jpspine = neck.GetComponent<JointsProperties>();
        Vector3 ans = jpshoulder.position - jpspine.position;
        Vector2 answer;
        answer.x = ans.x;
        answer.y = ans.z;
        return answer;
    }
    public float getShoulderZ()
    {
        GameObject shoulder = GameObject.Find("ShoulderRight");
        JointsProperties jpshoulder = shoulder.GetComponent<JointsProperties>();
        return jpshoulder.position.z;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("StartCircle"))
        {
            startGame = true;
            other.enabled = false;
        }
    }

    public float GetAccuracy()
    {
        return GD.accuracy;
    }
    public double CalculateAngle(GameObject start, GameObject mid, GameObject end)
    {
        Vector3 a = start.transform.position - mid.transform.position;
        Vector3 b = end.transform.position - mid.transform.position;
        return Vector3.Angle(a, b);
    }

    public float CalculateDistance(GameObject g1, GameObject g2)
    {
        return Vector3.Distance(g2.transform.position, g1.transform.position);
    }

    public float CalculateDistance(Windows.Kinect.Joint g1, Windows.Kinect.Joint g2)
    {
        return Vector3.Distance(GetVector3FromJoint(g1), GetVector3FromJoint(g2));
    }

    public void ResetColliders()
    {
       // Debug.Log(SD.InitialCollisions + "," + SD.totalInitial);
        if (SD.InitialCollisions >= (2 * SD.totalInitial))
        {
            CalculateAccuracy();
            SD.Noofcollisions =  SD.totalInitial;
            SD.InitialCollisions = SD.totalInitial;
            SD.stop = DateTime.Now;
            SD.timeTaken = SD.stop.Subtract(SD.start);
            SD.start = DateTime.Now;
            GD.totalTime = SD.timeTaken.TotalSeconds.ToString();
            SD.noOfFrames = 0;
            GameObject skel = GameObject.Find("skeleton");
            colloideDetect[] cd = skel.GetComponentsInChildren<colloideDetect>();
            foreach (var c in cd)
            {
                c.flag = true;
            }
        }
    }
    public void updateGesture(string gest)
    {
        SD.noOfFrames = 0;
        SD.totalInitial = 0;
        SD.NoOfCoins = 0;
        SD.InitialCollisions = 0;
        SD.totalcollisions = 0;
        SD.gestureType = " ";
        SD.Noofcollisions = 0;
        SD.totalFrames = 0;
        GD.gestureName=gest;
        GD.gestureTrue = false;
        foreach (var g in colls)
        {
                Destroy(g);
        }
        colls.Clear();
        CreateCoins();
        SD.rate = Convert.ToInt32((0.75 * SD.totalFrames) / SD.totalInitial);
        GameObject skel = GameObject.Find("skeleton");
        colloideDetect[] cd = skel.GetComponentsInChildren<colloideDetect>();
        foreach (var c in cd)
        {
            c.lastcollided = -1;
            c.flag = false;
            c.started = false;
        }
        SD.timestarted = false;
    }
    public double CalculateDistanceByFormula(Windows.Kinect.Joint g1, Windows.Kinect.Joint g2)
    {
        Vector3 s = GetVector3FromJoint(g1);
        Vector3 t = GetVector3FromJoint(g2);

        return Math.Sqrt(Math.Pow(s.x - t.x, 2) + Math.Pow(s.y - t.y, 2) + Math.Pow(s.z - t.z, 2));

    }
    private static Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }

    public void CalculateAccuracy()
    {

        SD.acc = GameObject.Find("Text 2");
        Text abc = SD.acc.GetComponent<Text>();
        GD.accuracy = (SD.Noofcollisions / SD.totalcollisions) * 100;
        abc.text = GD.accuracy.ToString();
        Debug.Log(GD.accuracy);
    }
    public float FindHandLength(string hand)
    {
        switch (hand)
        {
            case "left":
            case "Left":
            case "LEFT":
                GameObject leftShoulder = transform.Find("ShoulderLeft").gameObject;
                GameObject leftElbow = transform.Find("ElbowLeft").gameObject;
                GameObject leftWrist = transform.Find("WristLeft").gameObject;
                return Vector3.Distance(leftShoulder.transform.position, leftElbow.transform.position) + Vector3.Distance(leftElbow.transform.position, leftWrist.transform.position);
                break;
            case "right":
            case "Right":
            case "RIGHT":
                GameObject rightShoulder = transform.Find("ShoulderRight").gameObject;
                GameObject rightElbow = transform.Find("ElbowRight").gameObject;
                GameObject rightWrist = transform.Find("WristRight").gameObject;
                return Vector3.Distance(rightShoulder.transform.position, rightElbow.transform.position) + Vector3.Distance(rightElbow.transform.position, rightWrist.transform.position);
                break;
            default:
                return -1.0f;
        }
    }

    public void FindLegHeightFromGround(string leg)
    {
        switch (leg)
        {
            case "left":
            case "Left":
            case "LEFT":
                GameObject leftFoot = transform.Find("FootLeft").gameObject;
                JointsProperties LFJP = leftFoot.GetComponent<JointsProperties>();
                leftFootHeight = leftFootHeight > (float)(LFJP.distanceFromGround) ? (float)LFJP.distanceFromGround : leftFootHeight;
                break;
            case "right":
            case "Right":
            case "RIGHT":
                GameObject rightFoot = transform.Find("FootRight").gameObject;
                JointsProperties RFJP = rightFoot.GetComponent<JointsProperties>();
                rightFootHeight = rightFootHeight > (float)(RFJP.distanceFromGround) ? (float)RFJP.distanceFromGround : rightFootHeight;
                break;
        }
    }
    public string getType()
    {
        return SD.gestureType;
    }
    public void increaseInitial()
    {
        SD.InitialCollisions++;
    }
    public void increaseCollisions()
    {
        SD.Noofcollisions++;
    }
    public void CreateCoins()
    {
        double angle = 0;
        if (firstread)
        {
            GameObject head = transform.Find("Head").gameObject;
            GameObject shoulderLeft = transform.Find("ShoulderLeft").gameObject;
            GameObject shoulderRight = transform.Find("ShoulderRight").gameObject;
            GameObject foot = transform.Find("FootLeft").gameObject;
            JointsProperties jphead = head.GetComponent<JointsProperties>();
            JointsProperties jpshoulderl = shoulderLeft.GetComponent<JointsProperties>();
            JointsProperties jpshoulderr = shoulderRight.GetComponent<JointsProperties>();
            JointsProperties jpfoot = foot.GetComponent<JointsProperties>();
            SD.realheight = jphead.position.y;
            SD.realshoulder = Vector3.Distance(jpshoulderl.position, jpshoulderr.position);
            SD.realhandlength = (rightarmLength + leftarmLength) / 2;
            SD.realfootlevel = jpfoot.position.y;
            firstread = false;
        }
        XmlDocument doc = new XmlDocument();
        doc.Load("C:\\Users\\rajan\\Desktop\\" + GD.gestureName + ".xml");
        foreach (XmlNode node in doc.DocumentElement)
        {
            string jointname = node.Attributes[0].InnerText;
            if (jointname == "HandLength")
            {
                SD.handlengthdelta = SD.realhandlength - float.Parse(node.ChildNodes[0].InnerText);
            }
            else if (jointname == "HeadPos")
            {
                SD.heightdelta = SD.realheight - float.Parse(node.ChildNodes[0].InnerText);
            }
            else if (jointname == "ShoulderWidth")
            {
                SD.shoulderdelta = (SD.realshoulder - float.Parse(node.FirstChild.InnerText)) / 2;
            }
            else if (jointname == "AngleAdjust")
            {
                SD.angleadjust = float.Parse(node.FirstChild.InnerText) - SD.realfootlevel;
            }
            else if (jointname == "TotalCollisions")
            {
                SD.totalcollisions = float.Parse(node.FirstChild.InnerText);
            }
            else if (jointname == "GestureType")
            {
                SD.gestureType = node.FirstChild.InnerText;
            }
            else if (jointname == "TotalFrames")
            {
                SD.totalFrames = Convert.ToInt32(node.FirstChild.InnerText);
            }
            else if (jointname != "ValidAngle")
            {
                if (jointname.Contains("Initial"))
                    SD.totalInitial++;
                float x, y, z;
                x = float.Parse(node.ChildNodes[0].InnerText);
                y = float.Parse(node.ChildNodes[1].InnerText) - SD.angleadjust;
                z = float.Parse(node.ChildNodes[2].InnerText);
                if (jointname.Contains("Hand"))
                {
                    angle = Convert.ToDouble(node.ChildNodes[3].InnerText);
                    if (jointname.Contains("Right"))
                    {
                        if (angle < 90)
                            y = y /*- SD.angleadjust*/ + SD.heightdelta - (SD.handlengthdelta * Math.Abs(Convert.ToSingle(Math.Cos(ConvertDegreeToRadian(angle)))));
                        else
                            y = y /*- SD.angleadjust*/ + SD.heightdelta + (SD.handlengthdelta * Math.Abs(Convert.ToSingle(Math.Cos(ConvertDegreeToRadian(angle)))));
                        x += SD.shoulderdelta + (SD.handlengthdelta * Math.Abs(Convert.ToSingle(Math.Sin(ConvertDegreeToRadian(angle)))));
                    }
                    else if (jointname.Contains("Left"))
                    {
                        if (angle < 90)
                            y = y /*- SD.angleadjust*/ + SD.heightdelta - (SD.handlengthdelta * Math.Abs(Convert.ToSingle(Math.Cos(ConvertDegreeToRadian(angle)))));
                        else
                            y = y /*- SD.angleadjust*/ + SD.heightdelta + (SD.handlengthdelta * Math.Abs(Convert.ToSingle(Math.Cos(ConvertDegreeToRadian(angle)))));
                        x = x - SD.shoulderdelta - (SD.handlengthdelta * Math.Abs(Convert.ToSingle(Math.Sin(ConvertDegreeToRadian(angle)))));
                    }
                }
                else if (jointname.Contains("Head"))
                {
                    y = y /*- SD.angleadjust*/ + SD.heightdelta;
                }
                else if (jointname.Contains("Foot"))
                {
                    /*y = y - SD.angleadjust;*/
                    if (jointname.Contains("Right"))
                        x += SD.shoulderdelta;
                    else if (jointname.Contains("Left"))
                        x -= SD.shoulderdelta;
                }
                else if (jointname.Contains("Wrist"))
                {
                    y = y + SD.heightdelta;
                    z = z + SD.handlengthdelta;
                    if (jointname.Contains("Right"))
                        x += SD.shoulderdelta;
                    else if (jointname.Contains("Left"))
                        x -= SD.shoulderdelta;
                }
                GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                coin.transform.position = new Vector3(x, y, z);
                coin.transform.localScale = new Vector3(0.175f, 0.15f, 0.06f);
                coin.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
                coin.name = jointname;
                CapsuleCollider sc = coin.AddComponent<CapsuleCollider>();
                sc.isTrigger = true;
                sc.height = 2f;
                colls.Add(coin);
                /*Rigidbody rb = coin.AddComponent<Rigidbody>();
                rb.useGravity = false;*/
                //coin.tag = jointname;
                SD.NoOfCoins++;
            }
        }
    }
    public Vector3 PlaceCoinsIn2D(double angle, Vector3 origin, float radius, int angleDeviation)
    {
        float rad = ConvertDegreeToRadian(angle - angleDeviation);
        float x = (float)((origin.x + radius * Math.Cos(rad)));
        float y = (float)(origin.y + radius * Math.Sin(rad));
        float z = origin.z;

        return new Vector3(x, y, z);
    }

    public float ConvertDegreeToRadian(double degree)
    {
        return (float)(Math.PI * degree / 180.0);
    }

    public bool CheckLegAboveGround()
    {
        GameObject leftFoot = transform.Find("FootLeft").gameObject;
        JointsProperties LFJP = leftFoot.GetComponent<JointsProperties>();
        float leftLegHeight = (float)LFJP.distanceFromGround;

        GameObject rightFoot = transform.Find("FootRight").gameObject;
        JointsProperties RFJP = rightFoot.GetComponent<JointsProperties>();
        float rightLegHeight = (float)RFJP.distanceFromGround;

        float rightLegDiff = rightLegHeight - rightFootHeight;
        float leftLegDiff = leftLegHeight - leftFootHeight;

        if (rightLegDiff >= 0.02f && leftLegDiff >= 0.02f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /* public double getangle()
     {
         GameObject rightElbow = transform.Find("ElbowRight").gameObject;
         GameObject rightShoulder = transform.Find("ShoulderRight").gameObject;
         GameObject rightWrist = transform.Find("WristRight").gameObject;
         double armangle = CalculateAngle(rightShoulder, rightElbow, rightWrist);
         return armangle;
     }*/
    public float findDistanceBtwnFoots()
    {
        GameObject leftFoot = transform.Find("FootLeft").gameObject;
        GameObject rightFoot = transform.Find("FootRight").gameObject;

        return CalculateDistance(leftFoot, rightFoot);
    }

    public bool findAngleOfHand()
    {
        GameObject spineShoulder = transform.Find("SpineShoulder").gameObject;

        GameObject rightShoulder = transform.Find("ShoulderRight").gameObject;
        GameObject rightElbow = transform.Find("ElbowRight").gameObject;
        GameObject rightWrist = transform.Find("WristRight").gameObject;

        GameObject leftShoulder = transform.Find("ShoulderLeft").gameObject;
        GameObject leftElbow = transform.Find("ElbowLeft").gameObject;
        GameObject leftWrist = transform.Find("WristLeft").gameObject;

        double leftarmangle = CalculateAngle(leftShoulder, leftElbow, leftWrist);
        double leftarmshoulderangle = CalculateAngle(spineShoulder, leftShoulder, leftWrist);

        double rightarmangle = CalculateAngle(rightShoulder, rightElbow, rightWrist);
        double rightarmshoulderangle = CalculateAngle(spineShoulder, rightShoulder, rightWrist);

        //Debug.Log(leftarmangle);
        //Debug.Log(leftarmshoulderangle);

        bool lefthand = false;
        bool righthand = false;
        if ((leftarmangle > 150 && leftarmangle < 200) && (leftarmshoulderangle > 240 && leftarmshoulderangle < 300))
        {
            lefthand = true;
        }

        if ((rightarmangle > 150 && rightarmangle < 200) && (rightarmshoulderangle > 240 && rightarmshoulderangle < 300))
        {
            righthand = true;
        }

        return lefthand && righthand;

    }

}