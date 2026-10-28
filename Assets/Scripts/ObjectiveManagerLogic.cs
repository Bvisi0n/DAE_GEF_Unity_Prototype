using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class ObjectiveManagerLogic : MonoBehaviour
{
	private int _remainingSpawnPoints;

	/// <remarks>
	/// By default when LoadScene() is called the current scene gets destroyed,
	/// this bool helps prevent LoadScene() from being called recursively.
	/// </remarks>
	private bool _isEnding;

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

		PlayerLogic player = FindAnyObjectByType<PlayerLogic>();

		if (player == null)
		{
			Debug.LogWarning("PlayerLogic script could not be found in this scene.", this);
		}
		else
		{
			player.Destroyed += OnPlayerDestroyed;
		}

		_isEnding = false;
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

	private void OnPlayerDestroyed(PlayerLogic player)
	{
		Debug.Log("The player died.", this);
		player.Destroyed -= OnPlayerDestroyed;
		GameOver();
		
	}

	private void CompleteLevel()
	{
		Debug.Log("Level completed.", this);

		if (_isEnding)
		{
			return;
		}

		_isEnding = true;
		SceneManager.LoadScene(0);
	}

	private void GameOver()
	{
		Debug.Log("Lose condition was triggered.", this);

		if (_isEnding)
		{
			return;
		}

		_isEnding = true;
		SceneManager.LoadScene(0);
	}
}
