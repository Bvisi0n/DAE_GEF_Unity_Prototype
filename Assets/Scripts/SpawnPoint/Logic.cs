using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.SpawnPoint
{
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(Health))]
	public sealed class Logic : MonoBehaviour
	{
		[SerializeField, Min(0F)] private float _spawnInterval = 5F;

		[SerializeField, Min(1), Tooltip("Maximum amount of units this spawnpoint can have active at a time.")]
		private int _maxUnitCount = 1;

		[SerializeField] private GameObject _unitTemplate;

		private TextMeshPro _textLabel;
		private Collider _collider;
		private Health _health;
		private Visuals _visuals;
		private int _activeUnits;

		// TODO: Bug - Uses Time.time which does not account for pause or lag.
		private float _timestampOfLastSpawn;

		public event Action<Logic> Destroyed;

		private void Awake()
		{
			if (_unitTemplate == null)
			{
				Debug.LogError("Unit Template is not assigned, object is disabled.", this);
				enabled = false;
			}

			_textLabel = GetComponentInChildren<TextMeshPro>();

			if (_textLabel == null)
			{
				Debug.LogWarning("Couldn't find TextMeshPro among children of this gameobject.", this);
			}

			_visuals = GetComponentInChildren<Visuals>();

			if (_visuals == null)
			{
				Debug.LogWarning("Couldn't find Visuals script among children of this gameobject.", this);
			}

			_collider = GetComponent<Collider>();
			_health = GetComponent<Health>();
		}

		private void Start()
		{
			_health.CanHealthChange = false;
			_visuals.SetVulnerable(false);
			UpdateTextLabel();
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
			GameObject enemy = Instantiate(_unitTemplate, transform.position, transform.rotation);

			if (!enemy.TryGetComponent<Enemy>(out var enemyLogic))
			{
				Debug.LogError("EnemyLogic script not found on spawned GameObject.", this);
				return;
			}

			enemyLogic.Destroyed += OnEnemyDied;
			enemyLogic.SpawnPointPosition = transform.position;
			_activeUnits++;
			_health.CanHealthChange = false;
			_visuals.SetVulnerable(false);
			UpdateTextLabel();
			_timestampOfLastSpawn = Time.time;
		}

		/// <summary>
		/// Unsubscribe from the observed object,
		/// reset the spawn cooldown to prevent an instant respawn,
		/// update the count of units under my control and set my vulnerability accordingly.
		/// </summary>
		private void OnEnemyDied(Enemy enemy)
		{
			enemy.Destroyed -= OnEnemyDied;
			_activeUnits--;

			if (_activeUnits <= 0)
			{
				_health.CanHealthChange = true;
				_visuals.SetVulnerable(true);
			}

			UpdateTextLabel();
			_timestampOfLastSpawn = Time.time;
		}

		private void UpdateTextLabel()
		{
			if (_textLabel == null)
			{
				return;
			}

			_textLabel.text = $"{_activeUnits} / {_maxUnitCount}";
		}
	}
}