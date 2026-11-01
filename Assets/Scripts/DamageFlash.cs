using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
	[RequireComponent(typeof(Health))]
	public sealed class DamageFlash : MonoBehaviour
	{
		private static readonly int k_BaseColorId = Shader.PropertyToID("_BaseColor");

		[SerializeField] private Color _flashColor = Color.white;
		[SerializeField, Min(0F)] private float _flashDuration = 0.1F;

		private Health _health;

		// https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/renderer
		// Renderer is a component of a gameobject.
		private Renderer[] _renderers;

		// https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/materialpropertyblock
		private MaterialPropertyBlock[] _originalBlocks;
		private MaterialPropertyBlock _workingBlock;

		// https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/coroutine
		private Coroutine _flashRoutine;

		private void Awake()
		{
			_renderers = GetComponentsInChildren<Renderer>(true);

			if (_renderers.Length == 0)
			{
				Debug.LogWarning("No renderers found for the damage flash.", this);
				enabled = false;
				return;
			}

			_originalBlocks = new MaterialPropertyBlock[_renderers.Length];
			_workingBlock = new MaterialPropertyBlock();

			for (int index = 0; index < _renderers.Length; index++)
			{
				_originalBlocks[index] = new MaterialPropertyBlock();
			}

			_health = GetComponent<Health>();
		}

		private void OnEnable()
		{
			_health.Damaged += HandleDamaged;
		}

		private void OnDisable()
		{
			_health.Damaged -= HandleDamaged;

			if (_flashRoutine != null)
			{
				StopCoroutine(_flashRoutine);
				_flashRoutine = null;
			}

			RestoreOriginalBlocks();
		}

		private void HandleDamaged(int damageTaken)
		{
			if (_flashRoutine != null)
			{
				StopCoroutine(_flashRoutine);
				_flashRoutine = null;
				RestoreOriginalBlocks();
			}

			_flashRoutine = StartCoroutine(FlashRoutine());
		}

		private IEnumerator FlashRoutine()
		{
			ApplyFlashColor();
			yield return new WaitForSeconds(_flashDuration);
			RestoreOriginalBlocks();
			_flashRoutine = null;
		}

		/// <summary>
		/// Applies the flash color to all renderer components in this gameobject and its children.
		/// </summary>
		private void ApplyFlashColor()
		{
			for (int index = 0; index < _renderers.Length; index++)
			{
				Renderer targetRenderer = _renderers[index];

				// Store original block
				targetRenderer.GetPropertyBlock(_originalBlocks[index]);

				// Create and set flash block
				targetRenderer.GetPropertyBlock(_workingBlock);
				_workingBlock.SetColor(k_BaseColorId, _flashColor);
				targetRenderer.SetPropertyBlock(_workingBlock);
			}
		}

		/// <summary>
		/// Restores the original color to all renderer components in this gameobject and its children.
		/// </summary>
		private void RestoreOriginalBlocks()
		{
			if (_renderers == null || _originalBlocks == null)
			{
				return;
			}

			for (int index = 0; index < _renderers.Length; index++)
			{
				if (_renderers[index] != null)
				{
					_renderers[index].SetPropertyBlock(_originalBlocks[index]);
				}
			}
		}
	}
}
