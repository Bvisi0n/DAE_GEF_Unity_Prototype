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
		private int _activeUnits;

		// TODO: Bug - Uses Time.time which does not account for pause or lag.
		private float _timestampOfLastSpawn;

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
			_health.CanHealthChange = false;
			_visuals.SetVulnerable(false);
			_squadColor.SetSquadColor(_belt);
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

		private void OnDisable()
		{
			if (_objectiveTracker != null)
			{
				_objectiveTracker.Unregister(this);
			}
		}

		/// <summary>
		/// Spawn a unit above myself, subscribes to it's destruction event, give it my position,
		/// update the unit count under my control, disable my vulnerability and reset the spawn cooldown.
		/// </summary>
		private void SpawnUnit()
		{
			GameObject instance = Instantiate(_unitTemplate, transform.position, transform.rotation);

			if (!instance.TryGetComponent<Enemy.Enemy>(out var enemy))
			{
				Debug.LogError("Enemy script not found on spawned GameObject.", this);
				return;
			}

			// TODO: Bug - Cache all subscriptions and unsub from them upon self-destruction.
			enemy.Destroyed += OnEnemyDied;
			enemy.SpawnPointPosition = transform.position;

			_activeUnits++;
			_health.CanHealthChange = false;
			_visuals.SetVulnerable(false);
			UpdateTextLabel();
			_timestampOfLastSpawn = Time.time;

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
		private void OnEnemyDied()
		{
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