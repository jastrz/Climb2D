using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public GameObject Body;
    public GameObject LeftHand;
    public GameObject RightHand;

    public Dictionary<string, GameObject> Hands;
    
    public void Awake () 
    {
        Hands = new Dictionary<string, GameObject>
        {
            { "Left", LeftHand },
            { "Right", RightHand }
        };
    }

    // returns number of grabbing hands
    public int HandleHandGrabInputs()
    {
        var numHandsGrabbing = 0;
        foreach (GameObject hand in Hands.Values)
        {
            var handGrabResult = hand.GetComponent<Hand>().HandleGrab();
            if (handGrabResult)
            {
                numHandsGrabbing++;
            }
        }
        return numHandsGrabbing;
    }
}
