using UnityEngine;

public class DeathZoneLogic : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		Health health = other.GetComponent<Health>();

		if (health == null)
		{
			Debug.LogWarning("Object entering DeathZone doesn't have Health component.", this);
			return;
		}

		health.Die();
	}
}
