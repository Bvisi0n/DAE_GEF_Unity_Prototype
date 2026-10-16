using System;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
	public event Action<EnemyLogic> Died;

	private void OnDestroy()
	{
		Debug.Log("OnDestroy() is called.", this);
		Died?.Invoke(this);
	}
}
