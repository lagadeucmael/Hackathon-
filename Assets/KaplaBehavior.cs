using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class KaplaBehavior : MonoBehaviour
{
    protected bool rightHand;

    protected List<InputDevice> hand;

    // Start is called before the first frame update
    void Start()
    {
        isBeingHandled = false;
        hand = new List<InputDevice>();
    }

    protected bool isBeingHandled;

    public void Grab(int _rightHand)
    {
        isBeingHandled = true;
        rightHand = _rightHand == 1;
        this.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Release()
    {
        isBeingHandled = false;
        this.GetComponent<Rigidbody>().isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBeingHandled)
        {
            InputDeviceCharacteristics x = rightHand ? InputDeviceCharacteristics.Right : InputDeviceCharacteristics.Left;

            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | x, hand);

            if (hand[0].TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos))
            {
                if (hand[0].TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot))
                {
                    transform.localPosition = pos;
                    transform.localRotation = rot;
                }
            }
        }
    }
}
