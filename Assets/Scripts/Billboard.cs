using UnityEngine;

public sealed class Billboard : MonoBehaviour
{
	private void OnEnable()
	{
		if (Camera.main == null)
		{
			Debug.LogError("Couldn't find a Main Camera, script disabled.", this);
			enabled = false;
			return;
		}
	}

	void LateUpdate()
	{
		if (Camera.main == null)
		{
			return;
		}

		transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
	}
}
