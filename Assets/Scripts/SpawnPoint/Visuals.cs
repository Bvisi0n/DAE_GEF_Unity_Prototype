using UnityEngine;

namespace Assets.Scripts.SpawnPoint
{
	public sealed class Visuals : MonoBehaviour
	{
		[SerializeField] private Material _defaultMaterial;
		[SerializeField] private Material _VulnerableMaterial;

		private Renderer[] _renderers;

		private void Awake()
		{
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
				if (renderer == null)
				{
					return;
				}

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
