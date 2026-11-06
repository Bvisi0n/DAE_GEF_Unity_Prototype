using System;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
	public enum State { Idle, Chase, Attack }

	[RequireComponent(typeof(Attack))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(StateIndicator))]
	[RequireComponent(typeof(Targeting))]
	public sealed class StateHandler : MonoBehaviour, IObjective
	{
		[SerializeField, Min(0F), Tooltip("The range of the attack ability.")]
		private float _attackRange = 1F;

		[SerializeField, Min(0F), Tooltip("The range at which this unit will start chasing it's target.")]
		private float _spotRange = 10F;

		private Attack _attack;
		private Movement _movement;
		private StateIndicator _stateIndicator;
		private Targeting _targeting;
		private State _currentState;

		public IObjective.ObjectiveType Type => IObjective.ObjectiveType.Enemy;

		private void Awake()
		{
			_targeting = GetComponent<Targeting>();
			_attack = GetComponent<Attack>();
			_movement = GetComponent<Movement>();
			_stateIndicator = GetComponent<StateIndicator>();
		}

		private void Start()
		{
			_stateIndicator.SetIndicator(_currentState);
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

		private void HandleIdleState()
		{
			if (_targeting.IsTargetWithinRange(_spotRange))
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
			if (_targeting.Target == null)
			{
				SetState(State.Idle);
				Debug.Log($"{name} lost its target (null), changed to idle state.", this);
				return;
			}

			if (_targeting.IsTargetWithinRange(_attackRange))
			{
				SetState(State.Attack);
				Debug.Log($"{name} changed to attack state.", this);
			}
			else if (!_targeting.IsTargetWithinRange(_spotRange))
			{
				SetState(State.Idle);
				Debug.Log($"{name} lost its target (out of range), changed to idle state.", this);
			}
			else
			{
				_movement.ChaseTarget(_targeting.Target);
			}
		}

		private void HandleAttackState()
		{
			if (_targeting.Target == null)
			{
				SetState(State.Idle);
				Debug.Log($"{name} lost its target, changed to idle state.", this);
				return;
			}

			if (_targeting.IsTargetWithinRange(_attackRange))
			{
				_attack.TryAttack(_targeting.Target);
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
	}
}
