using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveManagerLogic : MonoBehaviour
{
	private int _remainingSpawnPoints;

	private void Awake()
	{
		SpawnPointLogic[] spawnPoints = FindObjectsByType<SpawnPointLogic>(FindObjectsSortMode.None);
		_remainingSpawnPoints = spawnPoints.Length;

		if (_remainingSpawnPoints <= 0)
		{
			Debug.LogWarning("This level does not contain any spawn points.", this);
		}

		foreach (var item in spawnPoints)
		{
			item.Destroyed += OnSpawnPointDestroyed;
		}
	}

	private void OnSpawnPointDestroyed(SpawnPointLogic spawnPoint)
	{
		_remainingSpawnPoints--;
		spawnPoint.Destroyed -= OnSpawnPointDestroyed;

		if (_remainingSpawnPoints <= 0)
		{
			CompleteLevel();
		}
	}

	private void CompleteLevel()
	{
		Debug.Log("Level completed.", this);
		SceneManager.LoadScene(0);
	}
}
