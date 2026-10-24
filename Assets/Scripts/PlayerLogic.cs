using System;
using UnityEngine;

public sealed class PlayerLogic : MonoBehaviour
{
	public event Action<PlayerLogic> Destroyed;

	private void OnDestroy()
	{
		Destroyed?.Invoke(this);
	}
}
