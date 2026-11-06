using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
	[RequireComponent(typeof(Attack))]
	[RequireComponent(typeof(Movement))]
	public sealed class InputHandler : MonoBehaviour
	{
		[SerializeField] private InputActionReference _movementAction;
		[SerializeField] private InputActionReference _jumpAction;
		[SerializeField] private InputActionReference _shootAction;

		private Attack _playerAttack;
		private Movement _playerMovement;

		/// <summary>
		/// Indicates whether the dependencies of the component are all available.
		/// </summary>
		private bool _isConfigured;

		private void Awake()
		{
			// TODO: Refactor - Refine input configuration safeguards with many LogError's to easily pinpoint the issue incase it breaks.
			_isConfigured
				= _movementAction != null
				&& _movementAction.action != null
				&& _jumpAction != null
				&& _jumpAction.action != null
				&& _shootAction != null
				&& _shootAction.action != null;

			if (!_isConfigured)
			{
				Debug.LogError("Incomplete input configuration, script disabled.", this);
				enabled = false;
				return;
			}

			if (Camera.main == null)
			{
				Debug.LogWarning("No MainCamera was found. Aiming input will be skipped.", this);
			}

			_playerAttack = GetComponent<Attack>();
			_playerMovement = GetComponent<Movement>();
		}

		private void OnEnable()
		{
			if (!_isConfigured)
			{
				return;
			}

			_jumpAction.action.performed += HandleJumpInput;
			_movementAction.action.Enable();
			_jumpAction.action.Enable();
			_shootAction.action.Enable();
		}

		private void OnDisable()
		{
			if (!_isConfigured)
			{
				return;
			}

			_jumpAction.action.performed -= HandleJumpInput;
			_movementAction.action.Disable();
			_jumpAction.action.Disable();
			_shootAction.action.Disable();
		}

		private void Update()
		{
			HandleAimingInput();
			HandleShootInput();
			HandleMovementInput();
		}

		private void HandleAimingInput()
		{
			if (Mouse.current == null || Camera.main == null)
			{
				return;
			}

			Vector3 mousePosition = Mouse.current.position.ReadValue();
			mousePosition.z = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
			Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
			worldMousePosition.z = 0F;
			_playerMovement.DesiredLookAtPoint = worldMousePosition;
		}

		private void HandleShootInput()
		{
			if (_playerAttack != null && _shootAction.action.IsPressed())
			{
				_playerAttack.TryAttack();
			}
		}

		private void HandleMovementInput()
		{
			if (_playerMovement == null || _movementAction == null)
			{
				return;
			}

			float movementInput = _movementAction.action.ReadValue<float>();
			Vector3 movement = movementInput * Vector3.right;
			_playerMovement.DesiredMovementDirection = movement;
		}

		private void HandleJumpInput(InputAction.CallbackContext context)
		{
			if (_playerMovement != null)
			{
				_playerMovement.Jump();
			}
		}
	}
}
