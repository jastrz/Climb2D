using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {

    public Text TimerText;
    public Text AltitudeText;

    private float currentTimerValue = 0.0f;
    
    public void Start () 
    {
        this.UpdateAltitude(0);
	}
    
    public void Update () 
    {
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
