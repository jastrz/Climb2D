using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {

    public Text TimerText;
    public Text AltitudeText;

    float currentTimerValue = 0.0f;
	// Use this for initialization
	void Start () {

        this.UpdateAltitude(0);

	}
	
	// Update is called once per frame
	void Update () {

        currentTimerValue += Time.deltaTime;
        TimerText.text = "Time: " + currentTimerValue;

	}

    public void ResetTimer()
    {
        currentTimerValue = 0.0f;
    }

    public void UpdateAltitude(float value)
    {
        AltitudeText.text = "Altitude: " + value;
    }
}
