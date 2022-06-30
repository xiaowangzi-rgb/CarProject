using UnityEngine;
using System.Collections;

public class FreeCamera : MonoBehaviour {

	private float 
		DistanceCam1,
		DistanceCam=2,
		rotX,
		rotY;

	public float
		MouseSpeed=4,
		MouseScrollSpeed=2,
		MinDistance=1,
		MaxDistance=2;

	public Transform target;

	void Update () {
		if (target == null) return;
		if (Input.GetKey (KeyCode.Mouse1)) {
			rotX += Input.GetAxis ("Mouse X") * MouseSpeed;
			rotY -= Input.GetAxis ("Mouse Y") * MouseSpeed;
		}

		DistanceCam -= Input.GetAxis ("Mouse ScrollWheel") * MouseScrollSpeed;
		DistanceCam = Mathf.Clamp (DistanceCam, MinDistance, MaxDistance);
		DistanceCam1 = Mathf.Lerp (DistanceCam1, DistanceCam, 10 * Time.deltaTime);
		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (rotY, rotX, 0),Time.deltaTime * 10);
		transform.position = target.position + transform.rotation * new Vector3 (0, 0, -DistanceCam1);

	}
}
