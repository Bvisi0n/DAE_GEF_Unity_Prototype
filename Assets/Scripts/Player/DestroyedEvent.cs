using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
	public sealed class DestroyedEvent : MonoBehaviour
	{
		public event Action Destroyed;

		private void OnDestroy()
		{
			Destroyed?.Invoke();
		}
	}
}
