using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
	public sealed class Logic : MonoBehaviour
	{
		public event Action<Logic> Destroyed;

		private void OnDestroy()
		{
			Destroyed?.Invoke(this);
		}
	}
}
