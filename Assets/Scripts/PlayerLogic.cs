using System;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
	public event Action<PlayerLogic> Destroyed;

	private void OnDestroy()
	{
		Destroyed?.Invoke(this);
	}
}
