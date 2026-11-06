using System;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
	public enum State { Idle, Chase, Attack }

	[RequireComponent(typeof(Attack))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(StateIndicator))]
	public sealed class Logic : MonoBehaviour, IObjective
	{
		[SerializeField, Min(0F), Tooltip("The range of the attack ability.")]
		private float _attackRange = 1F;

		[SerializeField, Min(0F), Tooltip("The range at which this unit will start chasing it's target.")]
		private float _spotRange = 10F;

		private Transform _attackTarget;
		private Attack _attack;
		private Movement _movement;
		private StateIndicator _stateIndicator;
		private State _currentState;

		public IObjective.ObjectiveType Type => IObjective.ObjectiveType.Enemy;

		public event Action<Logic> Destroyed;

		private void Awake()
		{
			const string targetTag = "Player";
			GameObject attackTarget = GameObject.FindWithTag(targetTag);

			if (attackTarget == null)
			{
				Debug.LogWarning($"No GameObject with tag '{targetTag}' found. No target set.", this);
			}
			else
			{
				_attackTarget = attackTarget.transform;
			}

			_attack = GetComponent<Attack>();
			_movement = GetComponent<Movement>();
			_stateIndicator = GetComponent<StateIndicator>();
		}

		private void Start()
		{
			SetState(State.Idle);
		}

		private void Update()
		{
			switch (_currentState)
			{
				case State.Idle:
					HandleIdleState();
					break;
				case State.Chase:
					HandleChaseState();
					break;
				case State.Attack:
					HandleAttackState();
					break;
			}
		}

		private void OnDestroy()
		{
			Destroyed?.Invoke(this);
		}

		private void HandleIdleState()
		{
			if (IsPlayerInRange(_spotRange))
			{
				SetState(State.Chase);
				Debug.Log($"{name} changed to chase state.", this);
			}
			else
			{
				_movement.WanderAroundSpawnPoint();
			}
		}

		private void HandleChaseState()
		{
			if (_attackTarget == null)
			{
				SetState(State.Idle);
				Debug.Log($"{name} lost its target (null), changed to idle state.", this);
				return;
			}

			if (IsPlayerInRange(_attackRange))
			{
				SetState(State.Attack);
				Debug.Log($"{name} changed to attack state.", this);
			}
			else if (!IsPlayerInRange(_spotRange))
			{
				SetState(State.Idle);
				Debug.Log($"{name} lost its target (out of range), changed to idle state.", this);
			}
			else
			{
				_movement.ChaseTarget(_attackTarget.transform);
			}
		}

		private void HandleAttackState()
		{
			if (_attackTarget == null)
			{
				SetState(State.Idle);
				Debug.Log($"{name} lost its target, changed to idle state.", this);
				return;
			}

			if (IsPlayerInRange(_attackRange))
			{
				_attack.TryAttack(_attackTarget);
			}
			else
			{
				SetState(State.Chase);
				Debug.Log($"{name} changed to Chase state.", this);
			}
		}

		private void SetState(State newState)
		{
			if (_currentState == newState)
			{
				return;
			}

			_currentState = newState;
			_stateIndicator.SetIndicator(_currentState);
		}

		private bool IsPlayerInRange(float range)
		{
			if (_attackTarget == null)
			{
				return false;
			}

			Vector3 distance = _attackTarget.transform.position - transform.position;
			return distance.sqrMagnitude <= range * range;
		}
	}
}
