using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpawnPointHandler : MonoBehaviour
{
	[SerializeField, Min(0F)] private float _spawnInterval = 5F;

	[SerializeField, Min(1), Tooltip("Maximum amount of units this spawnpoint can have active at a time.")]
	private int _maxUnitCount = 1;

	[SerializeField] private GameObject _unitTemplate;

	private Collider _collider;
	private int _activeUnits;
	private float _timestampOfLastSpawn;

	private void Awake()
	{
		if (_unitTemplate == null)
		{
			Debug.LogError("Unit Template is not assigned, object is disabled.", this);
			enabled = false;
		}

		_collider = GetComponent<Collider>();
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

		// TODO: When a unit gets destroyed, _activeUnits should also go down by one.
		// Events? Some kind of manager object? Instantiate as children?
	}

	private void SpawnUnit()
	{
		Vector3 offset = new Vector3(0F, _collider.bounds.max.y + 0.05F, 0F);
		Instantiate(_unitTemplate, transform.position + offset, transform.rotation);
		_activeUnits++;
		_timestampOfLastSpawn = Time.time;
	}
}
