using System;
using UnityEngine;

// TODO: Feedback -  How many units are under the control of this spawnpoint?

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Health))]
public class SpawnPointLogic : MonoBehaviour
{
	[SerializeField, Min(0F)] private float _spawnInterval = 5F;

	[SerializeField, Min(1), Tooltip("Maximum amount of units this spawnpoint can have active at a time.")]
	private int _maxUnitCount = 1;

	[SerializeField] private GameObject _unitTemplate;

	private Collider _collider;
	private Health _health;
	private int _activeUnits;
	private float _timestampOfLastSpawn;

	public event Action<SpawnPointLogic> Destroyed;

	private void Awake()
	{
		if (_unitTemplate == null)
		{
			Debug.LogError("Unit Template is not assigned, object is disabled.", this);
			enabled = false;
		}

		_collider = GetComponent<Collider>();
		_health = GetComponent<Health>();
	}

	private void Start()
	{
		_health.CanTakeDamage = false;
	}

	private void Update()
	{
		if (_activeUnits >= _maxUnitCount)
		{
			return;
		}

		if (Time.time - _timestampOfLastSpawn < _spawnInterval)
		{
			return;
		}

		SpawnUnit();
	}

	/// <summary>
	/// Notify observers about my destruction.
	/// </summary>
	private void OnDestroy()
	{
		Destroyed?.Invoke(this);
	}

	/// <summary>
	/// Spawn a unit above myself, subscribes to it's destruction event, give it my position,
	/// update the unit count under my control, disable my vulnerability and reset the spawn cooldown.
	/// </summary>
	private void SpawnUnit()
	{
		Vector3 offset = new(0F, _collider.bounds.max.y + 0.05F, 0F);
		
		GameObject enemy = Instantiate(_unitTemplate, transform.position + offset, transform.rotation);
		EnemyLogic enemyLogic = enemy.GetComponent<EnemyLogic>();

		if (enemyLogic == null)
		{
			Debug.LogError("EnemyLogic script not found on spawned GameObject.", this);
			return;
		}

		enemyLogic.Died += OnEnemyDied;
		enemyLogic.SpawnPointPosition = transform.position;
		_activeUnits++;
		_health.CanTakeDamage = false;
		_timestampOfLastSpawn = Time.time;
	}

	/// <summary>
	/// Unsubscribe from the observed object,
	/// reset the spawn cooldown to prevent an instant respawn,
	/// update the count of units under my control and set my vulnerability accordingly.
	/// </summary>
	private void OnEnemyDied(EnemyLogic enemyLogic)
	{
		enemyLogic.Died -= OnEnemyDied;
		_activeUnits--;

		if (_activeUnits <= 0)
		{
			_health.CanTakeDamage = true;
		}

		_timestampOfLastSpawn = Time.time;
	}
}
