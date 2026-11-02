using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
	public sealed class ObjectiveManager : MonoBehaviour
	{
		[SerializeField] private string _nextSceneOnWin;
		[SerializeField] private string _nextSceneOnLose;

		private int _remainingSpawnPoints;

		/// <remarks>
		/// By default when LoadScene() is called the current scene gets destroyed,
		/// this bool helps prevent LoadScene() from being called recursively.
		/// </remarks>
		private bool _isEnding;

		private void Awake()
		{
			SpawnPoint.Logic[] spawnPoints = FindObjectsByType<SpawnPoint.Logic>(FindObjectsSortMode.None);
			_remainingSpawnPoints = spawnPoints.Length;

			if (_remainingSpawnPoints <= 0)
			{
				Debug.LogWarning("This level does not contain any spawn points.", this);
			}

			foreach (var item in spawnPoints)
			{
				item.Destroyed += OnSpawnPointDestroyed;
			}

			Player.Logic player = FindAnyObjectByType<Player.Logic>();

			if (player == null)
			{
				Debug.LogWarning("Player.Logic script could not be found in this scene.", this);
			}
			else
			{
				player.Destroyed += OnPlayerDestroyed;
			}

			_isEnding = false;
		}

		private void OnSpawnPointDestroyed(SpawnPoint.Logic spawnPoint)
		{
			_remainingSpawnPoints--;
			spawnPoint.Destroyed -= OnSpawnPointDestroyed;

			if (_remainingSpawnPoints <= 0)
			{
				CompleteLevel();
			}
		}

		private void OnPlayerDestroyed(Player.Logic player)
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
			SceneManager.LoadScene(_nextSceneOnWin);
		}

		private void GameOver()
		{
			Debug.Log("Lose condition was triggered.", this);

			if (_isEnding)
			{
				return;
			}

			_isEnding = true;
			SceneManager.LoadScene(_nextSceneOnLose);
		}
	}
}
