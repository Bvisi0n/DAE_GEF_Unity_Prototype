using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.SpawnPoint
{
	[RequireComponent(typeof(Health))]
	[RequireComponent(typeof(SquadColor))]
	public sealed class Logic : MonoBehaviour, IObjective
	{
		[SerializeField] private GameObject _unitTemplate;
		[SerializeField, Min(1), Tooltip("Maximum amount of units this spawnpoint can have active at a time.")] private int _maxUnitCount = 1;
		[SerializeField, Min(0F)] private float _spawnInterval = 5F;
		[SerializeField] private Enemy.Tier.Belt _belt = Enemy.Tier.Belt.White;

		private TextMeshPro _textLabel;
		private ObjectiveTracker _objectiveTracker;
		private Health _health;
		private SquadColor _squadColor;
		private VulnerablePulse _visuals;
		private List<Enemy.Logic> _activeUnits;
		private float _spawnTimer;

		public IObjective.ObjectiveType Type => IObjective.ObjectiveType.SpawnPoint;

		private void Awake()
		{
			if (_unitTemplate == null)
			{
				Debug.LogError("Unit Template is not assigned, script disabled.", this);
				enabled = false;
				return;
			}

			_textLabel = GetComponentInChildren<TextMeshPro>();

			if (_textLabel == null)
			{
				Debug.LogWarning("Couldn't find TextMeshPro among children of this gameobject.", this);
			}

			_objectiveTracker = FindAnyObjectByType<ObjectiveTracker>();

			if (_objectiveTracker == null)
			{
				Debug.LogWarning("Couldn't find ObjectiveManager in this scene.", this);
			}

			_visuals = GetComponentInChildren<VulnerablePulse>();

			if (_visuals == null)
			{
				Debug.LogWarning("Couldn't find Visuals script among children of this gameobject.", this);
			}

			_health = GetComponent<Health>();
			_squadColor = GetComponent<SquadColor>();
		}

		private void OnEnable()
		{
			if (_objectiveTracker != null)
			{
				_objectiveTracker.Register(this);
			}
		}

		private void Start()
		{
			_squadColor.SetSquadColor(_belt);
			_activeUnits = new List<Enemy.Logic>(_maxUnitCount);

			for (int i = 0; i < _maxUnitCount; i++)
			{
				SpawnUnit();
			}

			UpdateTextLabel();
		}

		private void Update()
		{
			if (_activeUnits.Count >= _maxUnitCount)
			{
				return;
			}

			_spawnTimer -= Time.deltaTime;

			if (_spawnTimer <= 0F)
			{
				SpawnUnit();
			}
		}

		private void OnDisable()
		{
			if (_objectiveTracker != null)
			{
				_objectiveTracker.Unregister(this);
			}
		}

		private void OnDestroy()
		{
			foreach (var unit in _activeUnits)
			{
				unit.Destroyed -= OnEnemyDied;
			}
		}

		/// <summary>
		/// Spawn a unit above myself, subscribes to it's destruction event, give it my position,
		/// update the unit count under my control, disable my vulnerability and reset the spawn cooldown.
		/// </summary>
		private void SpawnUnit()
		{
			GameObject instance = Instantiate(_unitTemplate, transform.position, transform.rotation);
			_spawnTimer = _spawnInterval;

			if (!instance.TryGetComponent<Enemy.Logic>(out var enemy))
			{
				Debug.LogError("Enemy script not found on spawned GameObject.", this);
				return;
			}

			enemy.Destroyed += OnEnemyDied;

			if (!instance.TryGetComponent<Enemy.Movement>(out var enemyMovement))
			{
				Debug.LogError("Enemy.Movement script not found on spawned GameObject.", this);
				return;
			}

			enemyMovement.SpawnPointPosition = transform.position;
			_activeUnits.Add(enemy);
			_health.CanHealthChange = false;
			_visuals.SetVulnerable(false);
			UpdateTextLabel();

			if (!instance.TryGetComponent<Enemy.Tier>(out var enemyTier))
			{
				Debug.LogError("Enemy.Tier script not found on spawned GameObject.", this);
				return;
			}

			enemyTier.SetBelt(_belt);
		}

		/// <summary>
		/// Unsubscribe from the observed object,
		/// reset the spawn cooldown to prevent an instant respawn,
		/// update the count of units under my control and set my vulnerability accordingly.
		/// </summary>
		private void OnEnemyDied(Enemy.Logic enemy)
		{
			_activeUnits.Remove(enemy);

			if (_activeUnits.Count <= 0)
			{
				_health.CanHealthChange = true;
				_visuals.SetVulnerable(true);
			}

			UpdateTextLabel();
			_spawnTimer = _spawnInterval;
		}

		private void UpdateTextLabel()
		{
			if (_textLabel == null)
			{
				return;
			}

			_textLabel.text = $"{_activeUnits.Count} / {_maxUnitCount}";
		}
	}
}