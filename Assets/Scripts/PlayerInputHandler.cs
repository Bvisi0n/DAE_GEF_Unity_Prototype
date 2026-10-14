using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public sealed class PlayerInputHandler : MonoBehaviour
{
	[SerializeField] private InputActionAsset _inputAsset;
	[SerializeField] private InputActionReference _movementAction;
	[SerializeField] private InputActionReference _jumpAction;

	private PlayerMovement _playerMovement;
	private bool _isConfigured;

	private void Awake()
	{
		_playerMovement = GetComponent<PlayerMovement>();
		_isConfigured = _movementAction != null && _jumpAction != null;

		if (!_isConfigured)
		{
			Debug.LogError( "InputActionAsset is not properly configured.", this );
		}
	}

	private void OnEnable()
	{
		if (!_isConfigured)
		{
			return;
		}

		_jumpAction.action.performed += HandleJumpInput;
		_inputAsset.Enable();
	}

	private void OnDisable()
	{
		if (!_isConfigured)
		{
			return;
		}

		_jumpAction.action.performed -= HandleJumpInput;
		_inputAsset.Disable();
	}

	private void Update()
	{
		HandleMovementInput();
	}

	private void HandleJumpInput(InputAction.CallbackContext context)
	{
		if (_playerMovement == null)
		{
			return;
		}

		_playerMovement.Jump();
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
}
