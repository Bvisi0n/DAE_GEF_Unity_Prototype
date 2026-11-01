using System;
using UnityEngine;

namespace Assets.Scripts
{
	public sealed class DestroyedEvent : MonoBehaviour
	{
		public event Action<DestroyedEvent> Destroyed;

		private void OnDestroy()
		{
			Destroyed?.Invoke(this);
		}
	}
}
