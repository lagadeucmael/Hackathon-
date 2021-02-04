using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class KaplaBehavior : MonoBehaviour
{
    protected bool rightHand;

    protected InputDevice hand;

    protected bool grabbed = false;
    protected bool grabEvent = false;
    protected Quaternion previousRotation;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Grab(InputDevice _hand)
    {
        hand = _hand;
        if (!grabbed)
        {
            grabEvent = true;
        }
        grabbed = true;
        this.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Release()
    {
        this.GetComponent<Rigidbody>().isKinematic = false;
        grabbed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (grabbed)
        {
            if (hand.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos))
            {
                if (hand.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot))
                {
                    if (grabEvent)
                    {
                        previousRotation = Quaternion.Inverse(rot)*transform.rotation;
                        grabEvent = false;
                    }
                    transform.position = pos;
                    transform.rotation = rot * previousRotation;
                }
            }
        }
    }
}
