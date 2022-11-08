using System.Collections;
using UnityEngine;

using TMPro;

public class Scene5_Control : MonoBehaviour
{
    public QuestionnaireControl questionnaire;

    public float delayBeforeQuit = 5.0f;
	public int overrideMode=-1;


    void Start()
    {
        questionnaire.OnComplete.AddListener(QuestionnaireClose);


#if UNITY_EDITOR
		if (overrideMode >=0)
        {
            PlayerPrefs.SetInt ("Body", ((overrideMode & 1) > 0)? 1: 0);		
            PlayerPrefs.SetInt ("LookAt", ((overrideMode & 2) > 0) ? 1 : 0);
            PlayerPrefs.SetInt ("Induction", ((overrideMode & 4) > 0) ? 1 : 0);
        }
#endif
		// Reset the question text with the condition

		 StartCoroutine(ChangeText());
    }


	IEnumerator ChangeText() {

		yield return 0; // Wait until first frame just to make sure questionnaire is loaded

		TextMeshPro text = questionnaire.transform.Find("Questions/Question1/QuestionPanel/QuestionText").GetComponent<TextMeshPro>();
		if (PlayerPrefs.GetInt("Body") > 0)
			text.text = text.text + "\n you had a self avatar,";
		else
			text.text = text.text + "\n you had no self avatar,";

		if (PlayerPrefs.GetInt("LookAt") > 0)
			text.text = text.text + "\n you were looked at by the singer,";
		else
			text.text = text.text + "\n you were ignored by the singer,";

		if (PlayerPrefs.GetInt("Induction") > 0)
			text.text = text.text + "\n and you received induction.";
		else
			text.text = text.text + "\n and you did not receive induction.";
	}

    // Update is called once per frame
    void Update()
    {


        
    }

    private void QuestionnaireClose(QuestionnaireControl qc) {
        Debug.Log("Questionnaire completed");

        StartCoroutine(DelayedQuit());
    }

	IEnumerator DelayedQuit()
	{
		// delay one frame to make sure everything has initialized
		yield return 0;
		
		yield return new WaitForSeconds(delayBeforeQuit);

        Application.Quit();
	}    
}
