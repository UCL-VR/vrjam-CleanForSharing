using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

 using Ubiq.Logging;

public class Scene1_Control : MonoBehaviour {

	public string version;
    public float			delayBeforeLoad = 1.0f;
	public string			sceneToLoad = "2_ConsentPreQuestionnaire";

	private EventLogger results;

    // Use this for initialization
    void Start() {
		StartCoroutine(AppConfig());
        StartCoroutine(DelayedSceneLoad());
    }
		
	void Log(string loggable) {
		Debug.Log(loggable);
		results.Log(loggable);
    }

    IEnumerator AppConfig() { 
		string unique_id;

		yield return 0;
		yield return 0;

		results = new UserEventLogger(this);

		Log("Scene 1 Start");

#if	UNITY_ANDROID
		try 
		{
			AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
			AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
			unique_id = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
			PlayerPrefs.SetString("AndroidID", unique_id);
			Debug.Log("AndroidID " + unique_id);
		}
		catch (System.Exception e)
		{
			unique_id = "dummy";
			PlayerPrefs.SetString("AndroidID", unique_id);
			Debug.Log("AndroidID " + unique_id);
		}
#elif UNITY_IOS
		unique_id = SystemInfo.deviceUniqueIdentifier;
		PlayerPrefs.SetString("iOSID", unique_id);
		Debug.Log("iOSID " + unique_id);

#else
		try 
		{
			unique_id = SystemInfo.deviceUniqueIdentifier;
			PlayerPrefs.SetString("otherID", unique_id);
			Log("otherID " + unique_id);
		}
		catch (System.Exception)
		{
			unique_id = "dummy";
			PlayerPrefs.SetString("otherID", "unknown");
			Log("otherID " + "unknown");
		}
#endif


		PlayerPrefs.SetString("DeviceModel", SystemInfo.deviceModel);
		Log("DeviceModel " + SystemInfo.deviceModel);

		PlayerPrefs.SetString("AppVersion", version);
		Log("AppVersion " + version);

		string start_time = System.DateTime.Now.ToString("yyyMMdd-hhmmss");
		PlayerPrefs.SetString("StartTime", start_time);
		Log("StartTime " + start_time);

		int condition = Random.Range(0,8);

		PlayerPrefs.SetInt ("Condition", condition);
		Log("Condition " + condition);

		PlayerPrefs.SetInt ("Body", (condition & (0x01)));
		Log("Body " + (condition & (0x01)));

		PlayerPrefs.SetInt ("LookAt", (condition & (0x02)));
		Log("LookAt " + (condition & (0x02)));

		PlayerPrefs.SetInt ("Induction", (condition & (0x04)));
		Log("Inducton " + (condition & (0x04)));

	}


	IEnumerator DelayedSceneLoad()
	{
		// delay one frame to make sure everything has initialized
		yield return 0;
		
		yield return new WaitForSeconds(delayBeforeLoad);

		float startTime = Time.realtimeSinceStartup;
		AsyncOperation async = SceneManager.LoadSceneAsync(sceneToLoad);
		yield return async;
	}
	
	private void OnGUI()
     {
		if (Application.isEditor)
            if (GUILayout.Button("Next Scene")) {
				StopCoroutine(DelayedSceneLoad());
                delayBeforeLoad = 0;
                StartCoroutine(DelayedSceneLoad());
            }
    }
}
