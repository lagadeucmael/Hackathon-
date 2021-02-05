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

    protected List<InputDevice> hand;

    protected bool clicMD = false;
    protected bool clicMG = false;

    protected bool kaplaSpawnedAlready = false;

    protected Vector3 pos;
    protected Quaternion rot;


    bool grabbing = false;

    private void Awake()
    {
        hand = new List<InputDevice>();
    }

    bool GrabClosestKapla(InputDevice d, int layer, double threshold)
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
            ck.GetComponent<KaplaBehavior>().Grab(d);
            return true;
        }
        return false;
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
        hand.Clear();

        InputDeviceCharacteristics x = main == Main.DROITE ? InputDeviceCharacteristics.Right : InputDeviceCharacteristics.Left;

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | x, hand);

        if (hand.Count != 0) { 
            if (hand[0].TryGetFeatureValue(CommonUsages.devicePosition, out pos))
            {
                if (hand[0].TryGetFeatureValue(CommonUsages.deviceRotation, out rot))
                {
                    transform.localPosition = pos;
                    transform.localRotation = rot;
                }
            }

            if (hand[0].TryGetFeatureValue(CommonUsages.trigger, out float t) && t > 0.5)
            {
                if (main == Main.DROITE)
                {
                    List<InputDevice> leftHand = new List<InputDevice>();
                    InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, leftHand);
                    if (!kaplaSpawnedAlready && leftHand.Count > 0 && leftHand[0].TryGetFeatureValue(CommonUsages.trigger, out float tl) && tl > 0.5)
                    {
                        GameObject myPrefab = (GameObject)Resources.Load("Kapla");
                        GameObject.Instantiate(myPrefab, pos, rot);
                        kaplaSpawnedAlready = true;
                    }
                }
                GrabClosestKapla(hand[0], KaplasLayer, distanceSaisie);
            }
            else
            {
                ReleaseClosestKapla(hand[0], KaplasLayer, distanceSaisie);
                transform.localScale = 0.05f * Vector3.one;
                if (main == Main.DROITE)
                {
                    kaplaSpawnedAlready = false;
                }
                grabbing = false;
            }
        }
    }
}