using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpawnPointHandler : MonoBehaviour
{
	[SerializeField] float _spawnInterval = 5F;

	[SerializeField, Tooltip("Maximum amount of units this spawnpoint can have active at a time.")]
	byte _maxUnitCount = 1;

	[SerializeField] GameObject _unitTemplate;

	private Collider _collider;
	private bool _canSpawn = true;
	private byte _activeUnits = 0;
	private float _timestampOfLastSpawn = 0F;

	private void Awake()
	{
		if (_unitTemplate == null)
		{
			Debug.LogError("Missing dependencies!", this);
		}

		_collider = GetComponent<Collider>();
	}

	private void Update()
	{
		_canSpawn = _activeUnits < _maxUnitCount;

		if (_canSpawn && Time.time - _timestampOfLastSpawn > _spawnInterval)
		{
			SpawnUnit();
		}

		// TODO: When a unit gets destroyed, _activeUnits should also go down by one.
		// Ideally using events but for now the unit could just call a function in this object upon destruction.
	}

	private void SpawnUnit()
	{
		Vector3 offset = new Vector3(0F, _collider.bounds.max.y + 0.05F, 0F);
		Instantiate(_unitTemplate, transform.position + offset, transform.rotation);
		_activeUnits++;
		_timestampOfLastSpawn = Time.time;
	}
}
