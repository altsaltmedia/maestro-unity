using UnityEngine;
using System.Collections;

public class ShowCollider : MonoBehaviour {
	void OnDrawGizmos() {
		Gizmos.color = new Color(1, 0, 0, 1F);
		Gizmos.DrawWireCube (transform.position, new Vector3 (transform.localScale.x, transform.localScale.y, transform.localScale.z));
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = new Color(0, 1, 0, 1F);
		Gizmos.DrawWireCube (transform.position, new Vector3 (transform.localScale.x, transform.localScale.y, transform.localScale.z));
	}

}