using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public sealed class ObjectiveTracker : MonoBehaviour
	{
		private readonly Dictionary<IObjective.ObjectiveType, HashSet<IObjective>> _objectives = new();
		private bool _allObjectivesCompleted;

		public event Action<IObjective.ObjectiveType, int> ObjectiveCountChanged;
		public event Action AllObjectivesCompleted;

		private void Start()
		{
			_allObjectivesCompleted = false;
		}

		public int GetRemainingObjectives(IObjective.ObjectiveType type)
		{
			return _objectives.TryGetValue(type, out var objectives) ? objectives.Count : 0;
		}

		public void Register(IObjective objective)
		{
			if (objective == null)
			{
				return;
			}

			if (!_objectives.TryGetValue(objective.Type, out var objectives))
			{	// First of it's type.
				objectives = new HashSet<IObjective>();
				_objectives.Add(objective.Type, objectives);
			}

			if (!objectives.Add(objective))
			{	// Already registered.
				return;
			}

			ObjectiveCountChanged?.Invoke(objective.Type, GetRemainingObjectives(objective.Type));
		}

		public void Unregister(IObjective objective)
		{
			if (objective == null)
			{
				return;
			}

			if (!_objectives.TryGetValue(objective.Type, out var objectives))
			{	// None of this type registered.
				return;
			}

			if (!objectives.Remove(objective))
			{	// Wasn't registered.
				return;
			}

			ObjectiveCountChanged?.Invoke(objective.Type, GetRemainingObjectives(objective.Type));

			if (objectives.Count == 0)
			{	// It was the last of it's kind.
				_objectives.Remove(objective.Type);
			}

			CheckAllObjectivesCompleted();
		}

		private void CheckAllObjectivesCompleted()
		{
			if (_allObjectivesCompleted || _objectives.Count > 0)
			{
				return;
			}

			_allObjectivesCompleted = true;
			AllObjectivesCompleted?.Invoke();
		}
	}
}
