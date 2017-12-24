using UnityEngine;
using System.Collections;
using Windows.Kinect;
using UnityEngine.UI;

public class JointPosition : MonoBehaviour
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
                    countText.text = "X:" + pos.X;
                    countText.text = countText.text + ",Y:" + pos.Y;
                    countText.text = countText.text + ",Z:" + pos.Z;
                }

                float mh=0f;
                float mv=0f;
                if (pos.X > 0) mh = 1; else mh = -1;
                if (pos.Y > 0) mv = 1; else mv = -1;
                Vector3 movement = new Vector3(mh,0, mv);
                // Vector3 movement1 = new Vector3(pos.X, 0, pos.Z);
                // gameObject.transform.position = new Vector3(pos.X*100, 0.5f, pos.Z*100);

                countText.text = "hoz val:" + mh.ToString();
                countText.text = countText.text + "ver val:" + mv.ToString();
                countText.text = countText.text +"pos x:" + pos.X;
                countText.text = countText.text + "pos Y:" + pos.Y;

                rb.AddForce(movement * 2);
               // rb.AddForce(movement1 * 10);
                break;
            }
        }
    }
}
