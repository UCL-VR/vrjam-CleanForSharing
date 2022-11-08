using UnityEngine;
using System.Collections;

public enum ConsentStatus
{
    Consent,
    NoConsent,
    Either,
}

public class QuestionConfig : MonoBehaviour {
	public ConsentStatus consent;
	public bool body = false;
	public string special = null;
}