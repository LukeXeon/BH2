#pragma strict
var angleX:float = 45.0;
var angleY:float = 45.0;
var angleZ:float = 45.0;

function Update() {
	

	transform.Rotate( new Vector3(1,0,0)*angleX * Time.deltaTime);

}

