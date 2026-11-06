using UnityEngine;

namespace Assets.Scripts.Enemy
{
	public sealed class Targeting : MonoBehaviour
	{
		private Transform _target;

		public Transform Target => _target;

		private void Awake()
		{
			const string targetTag = "Player";
			GameObject target = GameObject.FindWithTag(targetTag);

			if (target == null)
			{
				Debug.LogWarning($"No GameObject with tag '{targetTag}' found, no target set.", this);
				return;
			}

			_target = target.transform;
		}

		public bool IsTargetWithinRange(float range)
		{
			if (_target == null)
			{
				return false;
			}

			Vector3 offset = _target.position - transform.position;
			return offset.sqrMagnitude <= range * range;
		}
	}
}
