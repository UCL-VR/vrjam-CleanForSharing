using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ubiq.Logging;

public class Scene2_Control : MonoBehaviour
{
    public QuestionnaireControl questionnaire;

    public float			delayBeforeLoad = 1.0f;
	public string			sceneToLoad = "3_BarScene";
    // Start is called before the first frame update

    private EventLogger results;

    void Start()
    {
        results = new UserEventLogger(this);
		results.Log("Scene 2 Start");
		Debug.Log("Scene 2 Start");
        questionnaire.OnComplete.AddListener(QuestionnaireClose);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Log(string loggable) {
		Debug.Log(loggable);
		results.Log(loggable);
    }

    private void QuestionnaireClose(QuestionnaireControl qc) {
        Log("Questionnaire completed");
        // Pause and load next scene
        StartCoroutine(DelayedSceneLoad());
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
    
}
