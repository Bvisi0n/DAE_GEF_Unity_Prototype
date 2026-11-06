using UnityEngine;

namespace Assets.Scripts.Enemy
{
	public sealed class StateIndicator : MonoBehaviour
	{
		[SerializeField] private GameObject _attackIndicator;
		[SerializeField] private GameObject _chaseIndicator;

		private void Awake()
		{
			if (_attackIndicator == null)
			{
				Debug.LogWarning("Attack Indicator not assigned.", this);
			}

			if (_chaseIndicator == null)
			{
				Debug.LogWarning("Chase Indicator not assigned.", this);
			}
		}

		public void SetIndicator(State state)
		{
			if (_attackIndicator != null)
			{
				_attackIndicator.SetActive(state == State.Attack);
			}

			if (_chaseIndicator != null)
			{
				_chaseIndicator.SetActive(state == State.Chase);
			}
		}
	}
}
