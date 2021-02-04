using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Controller : MonoBehaviour
{
    public enum Main
    {
        DROITE,
        GAUCHE
    }

    public int KaplasLayer = 8;

    public Main main;
    public double distanceSaisie = 0.1;

    protected List<InputDevice> devices;
    protected List<InputDevice> hmd;



    private void Awake()
    {
        devices = new List<InputDevice>();
    }

    void GrabClosestKapla(InputDevice d, int layer, double threshold)
    {
        var allGOs = FindObjectsOfType<GameObject>();
        var kaplas = new List<GameObject>();
        for (var i = 0; i < allGOs.Length; i++)
        {
            if (allGOs[i].layer == layer)
            {
                kaplas.Add(allGOs[i]);
            }
        }
        var tab = kaplas.ToArray();
        double mag = 100.0;
        int closestIndex = 0;
        Vector3 v;
        for (var i = 0; i < tab.Length; i++)
        {
            if (d.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos))
            {
                v = tab[i].transform.position - pos;
                if (v.magnitude < mag)
                {
                    closestIndex = i;
                    mag = v.magnitude;
                }
            }
        }
        if (mag < threshold)
        {
            GameObject ck = tab[closestIndex];
            ck.GetComponent<KaplaBehavior>().Grab((d.characteristics & InputDeviceCharacteristics.Right) != 0 ? 1 : 0);
        }
        kaplas = null;
    }

    void ReleaseClosestKapla(InputDevice d, int layer, double threshold)
    {
        var allGOs = FindObjectsOfType<GameObject>();
        var kaplas = new List<GameObject>();
        for (var i = 0; i < allGOs.Length; i++)
        {
            if (allGOs[i].layer == layer)
            {
                kaplas.Add(allGOs[i]);
            }
        }
        var tab = kaplas.ToArray();
        double mag = 100.0;
        int closestIndex = 0;
        Vector3 v;
        for (var i = 0; i < tab.Length; i++)
        {
            if (d.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos))
            {
                v = tab[i].transform.position - pos;
                if (v.magnitude < mag)
                {
                    closestIndex = i;
                    mag = v.magnitude;
                }
            }
        }
        if (mag < threshold)
        {
            GameObject ck = tab[closestIndex];
            ck.GetComponent<KaplaBehavior>().Release();
        }
        kaplas = null;
    }

    void Update()
    {
        devices.Clear();


        InputDeviceCharacteristics x = main == Main.DROITE ? InputDeviceCharacteristics.Right : InputDeviceCharacteristics.Left;

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | x, devices);

        foreach (InputDevice d in devices)
        {
            if (d.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos))
            {
                if (d.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot))
                {
                    transform.localPosition = pos;
                    transform.localRotation = rot;
                }
            }



            if (d.TryGetFeatureValue(CommonUsages.trigger, out float t) && t > 0.5)
            {
                GrabClosestKapla(d, KaplasLayer, distanceSaisie);
                transform.localScale = 0.1f * Vector3.one;
            }
            else
            {
                ReleaseClosestKapla(d, KaplasLayer, distanceSaisie);
                transform.localScale = 0.05f * Vector3.one;
            }
        }
    }
}