using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ubiq.Logging;

public class Scene4_Control : MonoBehaviour
{
    public QuestionnaireControl questionnaire;

    public float			delayBeforeLoad = 1.0f;
	public string			sceneToLoad = "5_Results";

    public bool forceConsent=false;
    public int overrideMode=-1;

    private EventLogger results;

    void Start()
    {
        results = new UserEventLogger(this);
		results.Log("Scene 4 Start");

        questionnaire.OnComplete.AddListener(QuestionnaireClose);

 #if UNITY_EDITOR
        if (forceConsent) PlayerPrefs.SetInt ("Consent", 1);
		if (overrideMode >=0)
        {
            PlayerPrefs.SetInt ("Body", ((overrideMode & 1) > 0)? 1: 0);		

        }
#endif
    }

    // Update is called once per frame
    void Update()
    {


        
    }

    private void QuestionnaireClose(QuestionnaireControl qc) {
        Debug.Log("Questionnaire completed");
        // Pause and load next scene
        StartCoroutine(DelayedSceneLoad());
    }

	IEnumerator DelayedSceneLoad()
	{
		// delay one frame to make sure everything has initialized
		yield return 0;
		
		yield return new WaitForSeconds(delayBeforeLoad);

		Debug.Log( "[LoadLevel] " + sceneToLoad + " Time: " + Time.time );

		float startTime = Time.realtimeSinceStartup;
		AsyncOperation async = SceneManager.LoadSceneAsync(sceneToLoad);
		yield return async;

        Debug.Log( "[SceneLoad] Completed: " + (Time.realtimeSinceStartup - startTime).ToString("F2") + " sec(s)");
	}
    
}
