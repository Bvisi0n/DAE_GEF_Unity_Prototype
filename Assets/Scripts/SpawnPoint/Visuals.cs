using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace Assets.Scripts.SpawnPoint
{
	public sealed class Visuals : MonoBehaviour
	{
		[SerializeField] private GameObject _Visual;
		[SerializeField] private Material _defaultMaterial;
		[SerializeField] private Material _VulnerableMaterial;

		private Renderer[] _renderers;

		private void Awake()
		{
			if (_Visual == null)
			{
				Debug.LogError("No Visual assigned, script disabled.", this);
				enabled = false;
				return;
			}

			if (_defaultMaterial == null)
			{
				Debug.LogError("No Default Material assigned, script disabled.", this);
				enabled = false;
				return;
			}

			if (_VulnerableMaterial == null)
			{
				Debug.LogError("No Vulnerable Material assigned, script disabled.", this);
				enabled = false;
				return;
			}

			_renderers = GetComponentsInChildren<MeshRenderer>(true);

			if (_renderers.Length == 0)
			{
				Debug.LogWarning("No renderers found for the damage flash.", this);
				enabled = false;
				return;
			}
		}

		public void SetVulnerable(bool isVulnerable)
		{
			foreach (var renderer in _renderers)
			{
				if (isVulnerable)
				{
					renderer.material = _VulnerableMaterial;
				}
				else
				{
					renderer.material = _defaultMaterial;
				}
			}
		}
	}
}
