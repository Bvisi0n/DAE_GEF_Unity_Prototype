using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
	[RequireComponent(typeof(NavMeshAgent))]
	public sealed class Movement : MonoBehaviour
	{
		[SerializeField, Min(0F), Tooltip("How far the unit will wander from it's spawn point when idle.")]
		private float _idleWanderRange = 5F;

		private NavMeshAgent _navMeshAgent;
		private float _destinationChangeTimer;

		public Vector3 SpawnPointPosition { get; set; }

		private void Awake()
		{
			_navMeshAgent = GetComponent<NavMeshAgent>();
		}

		private void Start()
		{
			WanderAroundSpawnPoint(false);
		}

		public void WanderAroundSpawnPoint(bool useCooldown = true)
		{
			if (!_navMeshAgent.isOnNavMesh)
			{
				Debug.LogWarning("Agent is not on Nav Mesh Surface.", this);
				return;
			}

			_destinationChangeTimer -= Time.deltaTime;

			if (useCooldown && _destinationChangeTimer > 0F)
			{
				return;
			}

			const float k_DestinationChangeCooldown = 5F;
			_destinationChangeTimer = k_DestinationChangeCooldown;
			float randomPoint = Random.Range(-1F, 1F) * _idleWanderRange;
			_navMeshAgent.destination = SpawnPointPosition + new Vector3(randomPoint, 0F, 0F);
		}

		public void ChaseTarget(Transform target)
		{
			if (!_navMeshAgent.isOnNavMesh)
			{
				Debug.LogWarning("Agent is not on Nav Mesh Surface.", this);
				return;
			}

			_destinationChangeTimer -= Time.deltaTime;
			const float k_DestinationChangeCooldown = 0.5F;

			if (_destinationChangeTimer > k_DestinationChangeCooldown || _destinationChangeTimer <= 0F)
			{
				// First condition can be true if it was idle before this call, that would use WanderAroundSpawnPoint and use a larger cooldown of 5.
				_navMeshAgent.destination = target.position;
				_destinationChangeTimer = k_DestinationChangeCooldown;
			}
		}
	}
}
