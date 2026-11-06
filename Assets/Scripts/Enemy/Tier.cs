using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
	/// <summary>
	/// <br>Represents the difficulty of the unit.</br>
	/// <br>The belts are inspired by the belts in martial arts.</br>
	/// </summary>
	public enum Belt { White, Yellow, Orange, Purple, Brown, Black }

	[RequireComponent(typeof(Attack))]
	[RequireComponent(typeof(Health))]
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof(SquadColor))]
	public sealed class Tier : MonoBehaviour
	{
		private Attack _attack;
		private Health _health;
		private NavMeshAgent _navMeshAgent;
		private SquadColor _squadColor;


		private void Awake()
		{
			_attack = GetComponent<Attack>();
			_health = GetComponent<Health>();
			_navMeshAgent = GetComponent<NavMeshAgent>();
			_squadColor = GetComponent<SquadColor>();
		}

		/// <summary>
		/// <br>Sets the difficulty of the unit.</br>
		/// <br>The belts are inspired by the belts in martial arts.</br>
		/// </summary>
		public void SetBelt(Belt belt)
		{
			// TODO: Refactor - Could this be improved with "ScriptableObject"?
			// Research needed...
			switch (belt)
			{
				case Belt.White:
					_squadColor.SetSquadColor(belt);
					_navMeshAgent.speed = 2.5F;
					_navMeshAgent.acceleration = 8F;
					_health.SetMaxHealth(1);
					_attack.SetStats(1, 2F);
					break;
				case Belt.Yellow:
					_squadColor.SetSquadColor(belt);
					_navMeshAgent.speed = 3F;
					_navMeshAgent.acceleration = 9F;
					_health.SetMaxHealth(2);
					_attack.SetStats(1, 1.8F);
					break;
				case Belt.Orange:
					_squadColor.SetSquadColor(belt);
					_navMeshAgent.speed = 3.5F;
					_navMeshAgent.acceleration = 10F;
					_health.SetMaxHealth(3);
					_attack.SetStats(2, 1.6F);
					break;
				case Belt.Purple:
					_squadColor.SetSquadColor(belt);
					_navMeshAgent.speed = 4F;
					_navMeshAgent.acceleration = 11F;
					_health.SetMaxHealth(5);
					_attack.SetStats(2, 1.4F);
					break;
				case Belt.Brown:
					_squadColor.SetSquadColor(belt);
					_navMeshAgent.speed = 4.5F;
					_navMeshAgent.acceleration = 12F;
					_health.SetMaxHealth(7);
					_attack.SetStats(3, 1.2F);
					break;
				case Belt.Black:
					_squadColor.SetSquadColor(belt);
					_navMeshAgent.speed = 5F;
					_navMeshAgent.acceleration = 14F;
					_health.SetMaxHealth(10);
					_attack.SetStats(4, 1F);
					break;
			}
		}
	}
}
