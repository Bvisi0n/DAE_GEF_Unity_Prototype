using UnityEngine;

namespace Assets.Scripts
{
	public sealed class FloatingAnimation : MonoBehaviour
	{
		[SerializeField] private float _amplitude = 0.15f;
		[SerializeField] private float _frequency = 2.0f;

		private Vector3 _initialPosition;

		private void Start()
		{
			_initialPosition = transform.position;
		}

		private void Update()
		{
			float offset = Mathf.Sin(Time.time * _frequency) * _amplitude;
			transform.position = _initialPosition + Vector3.up * offset;
		}
	}
}
