using UnityEngine;

namespace Assets.Scripts.UI
{
	public sealed class ObjectivePanel : MonoBehaviour
	{
		[SerializeField, Tooltip("The parent of the instantiated prefabs.")] private Transform _objectiveList;
		[SerializeField] private GameObject _objectiveEntryPrefab;

		private ObjectiveTracker _objectiveTracker;
		private ObjectiveEntry _spawnPointObjective;

		private void Awake()
		{
			if (_objectiveList == null)
			{
				Debug.LogError("Objective List not assigned, script disabled.", this);
				enabled = false;
				return;
			}

			if (_objectiveEntryPrefab == null)
			{
				Debug.LogError("Objective Entry Prefab not assigned, script disabled.", this);
				enabled = false;
				return;
			}

			_objectiveTracker = FindAnyObjectByType<ObjectiveTracker>();

			if (_objectiveTracker == null)
			{
				Debug.LogError("Couldn't find ObjectiveTracker in the scene, script disabled.", this);
				enabled = false;
				return;
			}
		}

		private void OnEnable()
		{
			if (_objectiveTracker != null)
			{
				_objectiveTracker.ObjectiveCountChanged += OnObjectiveCountChanged;
			}
		}

		private void Start()
		{
			// Assuming all objectives register themselves in OnEnable().

			if (_objectiveList == null || _objectiveEntryPrefab == null || _objectiveTracker == null)
			{
				enabled = false;
				return;
			}

			int spawnPointCount = _objectiveTracker.GetRemainingObjectives(ObjectiveType.SpawnPoint);

			if (spawnPointCount > 0)
			{
				GameObject instance = Instantiate(_objectiveEntryPrefab, _objectiveList);

				if (!instance.TryGetComponent<ObjectiveEntry>(out _spawnPointObjective))
				{
					Debug.LogWarning("ObjectiveEntry script not found on spawned GameObject.", this);
				}
				else
				{
					_spawnPointObjective.Initialize("Destroy all spawn points", spawnPointCount);
				}
			}

			if (_spawnPointObjective == null)
			{	// If no objectives, add checks when adding more.
				enabled = false;
			}
		}

		private void OnDisable()
		{
			if (_objectiveTracker != null)
			{
				_objectiveTracker.ObjectiveCountChanged -= OnObjectiveCountChanged;
			}
		}

		private void OnObjectiveCountChanged(ObjectiveType type, int remainingCount)
		{
			switch (type)
			{
				case ObjectiveType.SpawnPoint:
					if (_spawnPointObjective != null)
					{
						_spawnPointObjective.UpdateContent(remainingCount);
					}

					break;
			}
		}
	}
}
