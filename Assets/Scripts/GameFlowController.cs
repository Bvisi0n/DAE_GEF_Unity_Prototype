using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
	public sealed class GameFlowController : MonoBehaviour
	{
		[SerializeField] private GameObject _winPanel;
		[SerializeField] private GameObject _losePanel;
		[SerializeField] private ObjectiveTracker _objectiveTracker;
		[SerializeField] private string _nextLevel;

		private enum GameState { Playing, Won, Lost }
		private GameState _gameState;

		private Player.DestroyedEvent _player;

		private void Awake()
		{
			// TODO: Refactor - Make these logs dry and uniform by abstracting them. (in every script)
			if (_winPanel == null)
			{
				Debug.LogError("Win Panel not assigned, script disabled.", this);
				enabled = false;
				return;
			}

			if (_losePanel == null)
			{
				Debug.LogError("Win Panel not assigned, script disabled.", this);
				enabled = false;
				return;
			}

			if (_objectiveTracker == null)
			{
				Debug.LogError("ObjectiveTracker not assigned, script disabled.", this);
				enabled = false;
				return;
			}

			if (String.IsNullOrEmpty(_nextLevel))
			{
				Debug.LogError("Next Level not assigned, script disabled.", this);
				enabled = false;
				return;
			}

			const string targetTag = "Player";
			GameObject player = GameObject.FindWithTag(targetTag);

			if (player == null)
			{
				Debug.LogError($"No GameObject with tag '{targetTag}' found, script disabled.", this);
				enabled = false;
				return;
			}

			_player = player.GetComponent<Player.DestroyedEvent>();

			if (_player == null)
			{
				Debug.LogWarning("Player object doesn't have a Player.Logic component.", this);
			}
		}

		private void OnEnable()
		{
			if (_player != null)
			{
				_player.Destroyed += OnPlayerDestroyed;
			}

			if (_objectiveTracker != null)
			{
				_objectiveTracker.AllObjectivesCompleted += OnAllObjectivesCompleted;
			}
		}

		private void Start()
		{
			_gameState = GameState.Playing;
			Time.timeScale = 1F;
		}

		private void OnDisable()
		{
			if (_player != null)
			{
				_player.Destroyed -= OnPlayerDestroyed;
			}

			if (_objectiveTracker != null)
			{
				_objectiveTracker.AllObjectivesCompleted -= OnAllObjectivesCompleted;
			}
		}

		private void Win()
		{
			if (_winPanel == null || _gameState != GameState.Playing)
			{
				return;
			}

			_gameState = GameState.Won;
			Time.timeScale = 0F;
			_winPanel.SetActive(true);
		}

		private void Lose()
		{
			if (_losePanel == null || _gameState != GameState.Playing)
			{
				return;
			}

			_gameState = GameState.Lost;
			Time.timeScale = 0F;
			_losePanel.SetActive(true);
		}

		private void OnPlayerDestroyed()
		{
			Lose();
		}

		private void OnAllObjectivesCompleted()
		{
			Win();
		}

		public void OnContinueButtonClicked()
		{
			Time.timeScale = 1F;

			if (String.IsNullOrEmpty(_nextLevel))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}

			switch (_gameState)
			{
				case GameState.Won:
					SceneManager.LoadScene(_nextLevel);
					break;
				case GameState.Lost:
					SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
					break;
			}
		}

		public void OnQuitButtonClicked()
		{
			Application.Quit();
		}
	}
}
