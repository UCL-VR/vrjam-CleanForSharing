using UnityEngine;
using System.Collections;
using System.IO;
using Ubiq.Logging;

public class BodyLogger : MonoBehaviour {
	private GameObject head;
	private GameObject leftHand;
	private GameObject rightHand;
	public bool forceConsent=false;
	public float frequency = 0.1f;
	private EventLogger results;

	// Use this for initialization
	void Start () {

		if (forceConsent) PlayerPrefs.SetInt ("Consent", 1);		

		try
		{
			if (PlayerPrefs.GetInt("Consent")>0)
			{
				results = new UserEventLogger(this);
				head = GameObject.Find("Main Camera");
				leftHand = GameObject.Find("Left Hand");
				rightHand = GameObject.Find("Right Hand");

				InvokeRepeating("LogBody", 0.0f, frequency);
				Debug.Log ("Started logging");
			}
			else
			{
				Debug.Log ("Logging disabled (no consent)");
			}
		}
		catch (System.Exception)
		{
			Debug.Log ("Logging disabled (consent check not applied)");
		}
	}
	
	void LogBody() {
		// Log CenterEyeAnchor
		results.Log("Time: " + Time.timeSinceLevelLoad);
		results.Log("Head: " + head.transform.position + ", " + head.transform.eulerAngles);
		results.Log("LH: " + leftHand.transform.position + ", " + leftHand.transform.eulerAngles);
		results.Log("RH: " + rightHand.transform.position + ", " + rightHand.transform.eulerAngles);
	}

	void OnDestroy () {

		if (PlayerPrefs.GetInt("Consent")>0)
		{
			CancelInvoke("LogBody");
		}
	}
}
