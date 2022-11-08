using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;
using Ubiq.Logging;

public class Scene3_Control : MonoBehaviour {


	public AudioClip Mix_Induction;
	public AudioClip Mix_NoInduction;

	public RuntimeAnimatorController SingerController_Induction;
	public RuntimeAnimatorController SingerController_NoInduction;

	public float DelayToAudio=5;
	public float TimeToSwitch;
	public string nextLevel;
	public string nextLevelNoConsent;
    public int overrideMode = -1; // Male body = 1, Female body = 9
    public float delayBeforeLoad = 1.0f;


    /* Get the animators in order to force sync to the audio */

    Animator singerAnimator;

	IList<Animator> animatorsToSync = new List<Animator>();
			
	private AudioSource audioSource;

    private GameObject selfAvatar = null;

	private EventLogger results;

    // Use this for initialization
    void Start () {

		results = new UserEventLogger(this);
		results.Log("Scene 3 Start");

		//Find the resources
#if UNITY_EDITOR
		if (overrideMode >=0)
        {
            PlayerPrefs.SetInt ("Body", ((overrideMode & 1) > 0)? 1: 0);		
            PlayerPrefs.SetInt ("LookAt", ((overrideMode & 2) > 0) ? 1 : 0);
            PlayerPrefs.SetInt ("Induction", ((overrideMode & 4) > 0) ? 1 : 0);
            PlayerPrefs.SetInt ("Gender", ((overrideMode & 8) > 0) ? 1 : 0);

        }
#endif

		audioSource = GameObject.Find("Singer Audio Source").GetComponent<AudioSource>();

		//get the animators
		singerAnimator = GameObject.Find("Singer").GetComponent<Animator>();

		animatorsToSync.Add(singerAnimator);

		GameObject maleSelfAvatar = GameObject.Find("MaleAvatar");
		GameObject femaleSelfAvatar = GameObject.Find("FemaleAvatar");

		// Body Condition
		if (true)
		{
			if (PlayerPrefs.GetInt("Body")>0)
			{
				if (PlayerPrefs.GetInt("Gender")>0)
			    {
					// Female self avatar
					GameObject.Destroy(maleSelfAvatar);
					selfAvatar = femaleSelfAvatar;
				}
				else
				{
					// Male self avatar
					GameObject.Destroy(femaleSelfAvatar);
					selfAvatar = maleSelfAvatar;
				}

				animatorsToSync.Add(selfAvatar.GetComponent<Animator>());
			}
			else
			{
				// No self avatar
				GameObject.Destroy(maleSelfAvatar);
				GameObject.Destroy(femaleSelfAvatar);
			}
		}

		// Singer looks at player?
		if (PlayerPrefs.GetInt("LookAt")>0)
	    {
			// Singer looks at player
		}
		else
		{
			// Singer look into space
			GameObject.Find("Singer_Parent").transform.Rotate(0,-40,0);
		}

		// Induction animation
		if (PlayerPrefs.GetInt ("Induction")>0)
		{
			// Trigger induction audio and animation
			audioSource.clip = Mix_Induction;
			singerAnimator.runtimeAnimatorController = SingerController_Induction;
		}
		else
		{
			//Trigger non-induction audio
			audioSource.clip = Mix_NoInduction;
			singerAnimator.runtimeAnimatorController = SingerController_NoInduction;

			//No animation on self-avatar
			if(selfAvatar)
			{
				selfAvatar.GetComponent<Animator>().runtimeAnimatorController = null;
			}
		}


		//get remaining animators

		animatorsToSync.Add(GameObject.Find("Table - Animated").GetComponent<Animator>());
		animatorsToSync.Add(GameObject.Find("Box - Animated").GetComponent<Animator>());
		animatorsToSync.Add(GameObject.Find("MIB - Animated").GetComponent<Animator>());

		audioSource.Play();
        //StartCoroutine("StartAudio");

        StartCoroutine("ExitBar");
    }


    IEnumerator StartAudio()
    {
        yield return new WaitForSeconds(DelayToAudio);
        audioSource.Play();
    }

    IEnumerator ExitBar()
	{
		yield return new WaitForSeconds(TimeToSwitch);

		yield return new WaitForSeconds(delayBeforeLoad);
		if (PlayerPrefs.GetInt ("Consent")==1)
		{
		    SceneManager.LoadScene(nextLevel);
		}
		else
		{
			SceneManager.LoadScene(nextLevelNoConsent);
		}

	}

	float lastAudioTime = 0;

	// Update is called once per frame
	void Update () {
		if(this.audioSource.time > lastAudioTime){
			lastAudioTime = this.audioSource.time;

			foreach(var a in animatorsToSync)
			{
				AnimatorClipInfo[] clips = a.GetCurrentAnimatorClipInfo(0);
				if (clips.Length>0)
				{
					a.Play(0,0, this.audioSource.time/clips[0].clip.length);
				}
			}
		}
	}


}
