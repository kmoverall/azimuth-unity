using UnityEngine;
using System.Collections;

public class BarrelPush : MonoBehaviour {

	public float pushForce = 1.0f;
	public int characterLayer;
	public float sleepVelocity = 1.0f;

	// Use this for initialization
	void OnCollisionEnter(Collision info) {
		if (Mathf.Abs (GetComponent<Rigidbody>().velocity.x) < sleepVelocity  && info.gameObject.layer == characterLayer) {
			if (info.gameObject.transform.position.x > transform.position.x) {
				GetComponent<Rigidbody>().AddForce(-1 * pushForce, 0, 0, ForceMode.VelocityChange);
			} else {
				GetComponent<Rigidbody>().AddForce(pushForce, 0, 0, ForceMode.VelocityChange);
			}
		}
	}
}
