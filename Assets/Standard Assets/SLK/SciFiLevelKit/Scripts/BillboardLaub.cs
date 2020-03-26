using UnityEngine;
using System.Collections;

public class BillboardLaub: MonoBehaviour {

	private Transform mainCamTransform;
	private Transform cachedTransform;
	
	
	void Start () {
		mainCamTransform = Camera.main.transform;
		cachedTransform = transform;
	}
	
	void Update(){
		Vector3 v = mainCamTransform.position - cachedTransform.position;
		v.x=v.z=0;
		cachedTransform.LookAt( mainCamTransform.position-v);

	}

}
