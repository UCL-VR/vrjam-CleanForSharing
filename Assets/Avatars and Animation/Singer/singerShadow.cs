using UnityEngine;
using System.Collections;

public class singerShadow : MonoBehaviour {

	float y = 0;
	GameObject singer;


	// Use this for initialization
	void Start () {
		y = this.transform.position.y;
		singer = GameObject.Find("Singer:Bip01 Spine2");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 position = singer.transform.position;
		position.y = y;

		this.transform.position = position;
		this.transform.rotation = Quaternion.identity;

	}
}
