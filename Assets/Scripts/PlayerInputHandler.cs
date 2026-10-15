using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerMovement))]
public sealed class PlayerInputHandler : MonoBehaviour
{
	[SerializeField] private InputActionAsset _inputAsset;
	[SerializeField] private InputActionReference _movementAction;
	[SerializeField] private InputActionReference _jumpAction;
	[SerializeField] private InputActionReference _shootAction;

	private PlayerAttack _playerAttack;
	private PlayerMovement _playerMovement;
	private bool _isConfigured;

	private void Awake()
	{
		_playerAttack = GetComponent<PlayerAttack>();
		_playerMovement = GetComponent<PlayerMovement>();
		
		_isConfigured
			= _inputAsset != null
			&& _movementAction != null
			&& _movementAction.action != null
			&& _jumpAction != null
			&& _jumpAction.action != null
			&& _shootAction != null
			&& _shootAction.action != null;

		if (!_isConfigured)
		{
			Debug.LogError("Incomplete input configuration.", this);
		}

		if (Camera.main == null)
		{
			Debug.LogWarning("No MainCamera was found. Aiming input will be skipped.", this);
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
			_playerAttack.Fire();
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
