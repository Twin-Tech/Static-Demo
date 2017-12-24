using UnityEngine;
using System.Collections;
using Windows.Kinect;
using UnityEngine.UI;

public class Handposition : MonoBehaviour
{
    public Windows.Kinect.JointType _jointType;
    public GameObject _bodySourceManager;
    private BodySourceManager _bodyManager;
    private Rigidbody rb;
    public Text countText;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {


        if (_bodySourceManager == null)
        {
            return;
        }

        _bodyManager = _bodySourceManager.GetComponent<BodySourceManager>();
        if (_bodyManager == null)
        {
            return;
        }

        Body[] data = _bodyManager.GetData();
        if (data == null)
        {
            return;
        }

        // get the first tracked body...
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                //this.gameObject.transform.position = new Vector3
                // this.gameObject.transform.localPosition =  body.Joints[_jointType].Position;
                var pos = body.Joints[_jointType].Position;
                if (pos.X != 0 || pos.Y != 0)
                {
                    countText.text = "";
                    countText.text = "HOR1:" + pos.X;
                    countText.text = countText.text + ",VER2:" + pos.Z;
                }

                Vector3 movement = new Vector3(pos.Z,0, pos.Y);
                Vector3 movement1 = new Vector3(pos.X, 0, pos.Z);
                gameObject.transform.position = new Vector3(pos.X, 0.5f, pos.Z);

               // rb.AddForce(movement * 10);
               // rb.AddForce(movement1 * 10);
                break;
            }
        }
    }
}
