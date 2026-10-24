using System;
using UnityEngine;

// TODO: Feedback - How do I know which unit belongs to wich spawnpoint?

public class EnemyLogic : MonoBehaviour
{
	public event Action<EnemyLogic> Died;

	private void OnDestroy()
	{
		Debug.Log("OnDestroy() is called.", this);
		Died?.Invoke(this);
	}
}
