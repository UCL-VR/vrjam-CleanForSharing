using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Ubiq.Rooms;
using Ubiq.Messaging;
using Ubiq.XR;
using Ubiq.Logging;
using System.Xml;

using System.IO;
using TMPro;

public class QuestionnaireControl : MonoBehaviour {
    public TextAsset QuestionsXml;
    public bool forceConsent = false;
	public List<GameObject> questions = new List<GameObject>();
	public List<GameObject> audioTracks =  new List<GameObject>();

    public GameObject questionTemplate=null;
    public GameObject likert7Template=null;
    public GameObject buttonTemplate=null;
    public GameObject audioTrackTemplate=null;

    public int selected = 0;

    public List<string> audioTrackNames = new List<string>();
    private string lastString;  

    EventLogger results;


    private void Start() {
        results = new UserEventLogger(this);
    }

    private void Awake() {
        if (forceConsent) PlayerPrefs.SetInt ("Consent", 1);
        ReadQuestionnaire();
    }

    private void OnDestroy() {
    }

    [Serializable]
    public class QuestionnaireEvent : UnityEvent<QuestionnaireControl> { }
    public QuestionnaireEvent OnAnswer;
    public QuestionnaireEvent OnComplete;
    public QuestionnaireEvent OnBack;

    private void ReadQuestionnaire() {
        lastString = null;

        if (QuestionsXml != null) {
            XmlTextReader reader = new XmlTextReader(new StringReader(QuestionsXml.text));

            while (reader.Read()) {
                if (reader.Name == "Audio") { // AUDIO
                    GameObject newAudio = Instantiate(audioTrackTemplate) as GameObject;
                    newAudio.transform.name = reader.GetAttribute("AudioFile");
                    newAudio.SetActive(true);
                    newAudio.GetComponent<AudioSource>().clip = Resources.Load(reader.GetAttribute("AudioFile")) as AudioClip;
                    newAudio.transform.parent = transform.Find("AudioTracks");
                    
                    audioTracks.Add(newAudio);
                    lastString = newAudio.transform.name;
                }

                if (reader.Name == "Question") { // QUESTION
                    GameObject newQuestion = Instantiate(questionTemplate) as GameObject;
                    newQuestion.transform.name = string.Concat("Question", questions.Count+1);

                    audioTrackNames.Add(lastString);
                    lastString = null; 

                    newQuestion.GetComponent<QuestionConfig>().special = reader.GetAttribute("Special");

                    if (reader.GetAttribute("Consent") == "yes")
                        newQuestion.GetComponent<QuestionConfig>().consent = ConsentStatus.Consent;
                    else if (reader.GetAttribute("Consent") == "no")
                        newQuestion.GetComponent<QuestionConfig>().consent = ConsentStatus.NoConsent;
                    else
                        newQuestion.GetComponent<QuestionConfig>().consent = ConsentStatus.Either;

                    if (reader.GetAttribute("Avatar") == "yes")
                        newQuestion.GetComponent<QuestionConfig>().body = true;
                    newQuestion.transform.Find("QuestionPanel/QuestionText").GetComponent<TextMeshPro>().text = reader.GetAttribute("QuestionText");
                    newQuestion.SetActive(false);


                    if (reader.GetAttribute("Answer0") == "Likert7") {
                        // Has a Likert answer with 7 buttons
                        GameObject newLikert = Instantiate(likert7Template) as GameObject;
                        newLikert.transform.name = "Likert7";
                        newLikert.transform.SetParent(newQuestion.transform.Find("AnswerPanel/Likert").transform, false);
                        newLikert.SetActive(true);
                    }
                    else {
                        // Has a multi-choice answer
                        for (int j = 0; j <= reader.AttributeCount; j++) { // There can't be more answers than attributes.
                            string answer = reader.GetAttribute(string.Concat("Answer", j.ToString()));
                            if (!string.IsNullOrWhiteSpace(answer)) {
                                GameObject newButton = Instantiate(buttonTemplate) as GameObject;
                                newButton.transform.name = string.Concat("btn", j.ToString());
                                newButton.transform.Find("Text").GetComponent<TextMeshPro>().text = answer;
                                newButton.transform.SetParent(newQuestion.transform.Find("AnswerPanel/MultipleChoice").transform, false);
                                newButton.SetActive(true); // Activate all buttons
                            }
                        }
                    }

                    if (questions.Count == 0) {
                        newQuestion.transform.Find("ControlPanel/BackButton").gameObject.SetActive(false);
                    }

                    newQuestion.transform.SetParent(transform.Find("Questions"), false);
                    questions.Add(newQuestion);
                }
            }

            reader.Close();

            if (questions.Count > 0) questions[0].SetActive(true); // Activate first poster

            if (audioTrackNames[0] != null && GameObject.Find(audioTrackNames[0]) != null) { 
                GameObject.Find(audioTrackNames[0]).GetComponent<AudioSource>().Play();
            }
        }
    }

    void Log(string loggable) {
		Debug.Log(loggable);
        if (PlayerPrefs.GetInt("Consent") > 0) {
            results.Log(loggable);
        }
    }

    public void Back() {
        Log("Back");
        PreviousPoster();
    }

    public void Answer(GameObject label) {
        string answer = label.GetComponent<TextMeshPro>().text;

        Log("Question: " + selected + " Answer: " + answer);

        // Special cases
        string special = questions[selected].GetComponent<QuestionConfig>().special;
        if (!string.IsNullOrWhiteSpace(special)) {
            if (special == "Consent") PlayerPrefs.SetInt("Consent", (answer == "Yes")?1:0);
            if (special == "Gender") {
                if (answer=="Male") PlayerPrefs.SetInt("Gender",0);
                else if (answer=="Female") PlayerPrefs.SetInt("Gender",1);
                else PlayerPrefs.SetInt("Gender", UnityEngine.Random.Range(0, 1));
            }
        }

        // Go to next poster
        NextPoster();
    }

    private void PreviousPoster() { // Deactivate current poster and activate previous
        StopAllAudio();

        // Note the complexity here is to do with the fact that the questionnaires might vary depending on conditions of consent and condition
        questions[selected].SetActive(false);

        bool prev = selected > 0;
        while (prev) { // Attempt to go backward
            prev = false;
            selected--;
            if (selected == 0) break;

            if (PlayerPrefs.GetInt("Consent") == 0)
                prev = questions[selected].GetComponent<QuestionConfig>().consent == ConsentStatus.Consent;
            else
                prev = questions[selected].GetComponent<QuestionConfig>().consent == ConsentStatus.NoConsent;

            if (!prev)
                if (PlayerPrefs.GetInt("Body") == 0) // If user has no body
                    prev = questions[selected].GetComponent<QuestionConfig>().body == true;
        }

        questions[selected].SetActive(true);
    }

    private void NextPoster(){ // Deactivate current poster and activate next

        StopAllAudio();

        // Note the complexity here is to do with the fact that the questionnaires might vary depending on conditions of consent and condition
        questions[selected].SetActive(false);

        bool next = true;

        while (next) { // Attempt to go forward
            next = false;
            selected++;
            if (selected > (questions.Count - 1)) {
                OnComplete.Invoke(this);
                return;
            }

            if (PlayerPrefs.GetInt("Consent") == 0) // If user has not given consent
                next = (questions[selected].GetComponent<QuestionConfig>().consent == ConsentStatus.Consent);
            else
                next = questions[selected].GetComponent<QuestionConfig>().consent == ConsentStatus.NoConsent;

            if (!next)
                if (PlayerPrefs.GetInt("Body") == 0) // If user has no body
                    next = questions[selected].GetComponent<QuestionConfig>().body;
        }

        // Otherwise play audio if we find it
        if (audioTrackNames[selected] != null && GameObject.Find(audioTrackNames[selected]) != null) {
            GameObject.Find(audioTrackNames[selected]).GetComponent<AudioSource>().Play();
        }

        // Make the next question available`
		questions[selected].SetActive(true);
	}
	
    void StopAllAudio() {
        foreach (AudioSource audioS in FindObjectsOfType (typeof(AudioSource)) as AudioSource[]) {
			audioS.Stop ();
		}
	}
}
