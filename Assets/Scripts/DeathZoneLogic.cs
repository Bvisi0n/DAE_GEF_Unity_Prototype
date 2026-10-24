using UnityEngine;

public sealed class DeathZoneLogic : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (!other.TryGetComponent<Health>(out var health))
		{
			Debug.LogWarning("Object entering DeathZone doesn't have Health component.", this);
			return;
		}

		health.Die();
	}
}
