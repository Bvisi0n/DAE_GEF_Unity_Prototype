using System;
using UnityEngine;
using UnityEngine.AI;

// TODO: Feedback - How can the player know which unit belongs to wich spawnpoint?
// TODO: Refactor - Split this logic up in seperate specialized scripts.

[System.Serializable]
public enum State { Idle, Chase, Attack }

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyLogic : MonoBehaviour
{
	[SerializeField, Min(0F)] private int _attackDamage = 1;
	[SerializeField, Min(0F)] private float _attackCooldown = 1F;

	[SerializeField, Min(0F), Tooltip("The range of the attack ability.")]
	private float _attackRange = 1F;

	[SerializeField, Min(0F), Tooltip("The range at wich this unit will start chasing it's target.")]
	private float _spotRange = 10F;

	[SerializeField, Min(0F), Tooltip("How far the unit will wander from it's spawn point when idle.")]
	private float _idleWanderRange = 5F;

	private GameObject _attackTarget;
	private NavMeshAgent _navMeshAgent;
	private Vector3 _spawnPointPosition;
	private State _currentState;

	// TODO: Bug - Uses Time.time which does not account for pause or lag.
	private float _timestampOfLastDestinationChange;
	private float _attackTimer;

	public Vector3 SpawnPointPosition
	{
		// TODO: Refactor - SpawnPointPosition isn't properly encapsulated.
		get { return _spawnPointPosition; }
		set { _spawnPointPosition = value; }
	}

	public event Action<EnemyLogic> Died;

	private void Awake()
	{
		const string targetTag = "Player";
		_attackTarget = GameObject.FindWithTag(targetTag);

		if (_attackTarget == null)
		{
			Debug.LogWarning($"No GameObject with tag '{targetTag}' found. No target set.");
		}

		_navMeshAgent = GetComponent<NavMeshAgent>();
	}

	private void Start()
	{
		_currentState = State.Idle;
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
		Debug.Log("OnDestroy() is called.", this);
		Died?.Invoke(this);
	}

	private void HandleIdleState()
	{
		if (IsPlayerInRange(_spotRange))
		{
			_currentState = State.Chase;
			Debug.Log($"{name} changed to chase state.", this);
		}
		else
		{
			WanderAroundSpawnPoint();
		}
	}

	private void HandleChaseState()
	{
		if (_attackTarget == null)
		{
			_currentState = State.Idle;
			Debug.Log($"{name} lost its target (null), changed to idle state.", this);
			return;
		}

		if (IsPlayerInRange(_attackRange))
		{
			_currentState = State.Attack;
			Debug.Log($"{name} changed to attack state.", this);
		}
		else if (!IsPlayerInRange(_spotRange))
		{
			_currentState = State.Idle;
			Debug.Log($"{name} lost its target (out of range), changed to idle state.", this);
		}
		else
		{
			ChaseTarget();
		}
	}

	private void HandleAttackState()
	{
		if (_attackTarget == null)
		{
			_currentState = State.Idle;
			Debug.Log($"{name} lost its target, changed to idle state.", this);
			return;
		}

		if (IsPlayerInRange(_attackRange))
		{
			_attackTimer -= Time.deltaTime;

			if (_attackTimer <= 0.0F)
			{
				AttackTarget();
			}
		}
		else
		{
			_currentState = State.Chase;
			Debug.Log($"{name} changed to Chase state.", this);
		}
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

	private void WanderAroundSpawnPoint()
	{
		if (!_navMeshAgent.isOnNavMesh)
		{
			Debug.LogWarning("Agent is not on Nav Mesh Surface.", this);
			return;
		}

		float destinationChangeCooldown = 5F;
		if (Time.time - _timestampOfLastDestinationChange > destinationChangeCooldown)
		{
			_timestampOfLastDestinationChange = Time.time;
			Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * _idleWanderRange;
			_navMeshAgent.destination = _spawnPointPosition + new Vector3(randomPoint.x, 0F, 0F);
		}
	}

	private void ChaseTarget()
	{
		// TODO: Bug - Enemies can push player inside other objects.
		if (!_navMeshAgent.isOnNavMesh)
		{
			Debug.LogWarning("Agent is not on Nav Mesh Surface.", this);
			return;
		}

		float destinationChangeCooldown = 0.5F;
		if (Time.time - _timestampOfLastDestinationChange > destinationChangeCooldown)
		{
			_timestampOfLastDestinationChange = Time.time;
			_navMeshAgent.destination = _attackTarget.transform.position;
		}
	}

	private void AttackTarget()
	{
		if (_attackTarget == null)
		{
			return;
		}

		_attackTimer += _attackCooldown;
		Health targetHealth = _attackTarget.GetComponent<Health>();

		if (targetHealth == null)
		{
			Debug.LogWarning("Target has no health to deal damage too.", this);
			return;
		}

		targetHealth.ChangeHealth(-_attackDamage);
	}
}
