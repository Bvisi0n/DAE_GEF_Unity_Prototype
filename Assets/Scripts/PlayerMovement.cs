using UnityEngine;

// FEEDBACK: 26/10/14 9AM by Arene Six
// Movement feels stiff, use forces instead of setting velocity directly.
// Play some action platformers like Celeste and Hollow Knight to get a feel.
// You'll notice the character glides a accelerates and decelerates, keep it snappy tho!
// GDC Festival of Gaming: Math for Game Programmers: Building a Better Jump
// https://youtu.be/hG9SzQxaCm8?si=HR7YLmNufOJKsG6R

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public sealed class PlayerMovement : MonoBehaviour
{
	private const float k_GroundCheckDistance = 0.2F;
	private const string k_GroundLayer = "Ground";

	[Header("References")]
	[SerializeField] private GameObject _shoulderSocket;

	[Header("Movement")]
	[SerializeField] private float _movementSpeed = 7F;
	[SerializeField] private float _jumpLaunchSpeed = 11F;
	[SerializeField] private float _ascentGravityMultiplier = 2F;
	[SerializeField] private float _fallGravityMultiplier = 2F;
	[SerializeField] private float _apexGravityMultiplier = 0.5F;
	[SerializeField] private float _apexVelocityThreshold = 0.5F;

	private Collider _collider;
	private Rigidbody _rigidBody;

	private Vector3 _desiredLookAtPoint;
	private Vector3 _desiredMovementDirection;
	
	private bool _isGrounded;
	private int _groundLayerMask;
	private bool _jumpPhaseActive;
	private float _jumpStartHeight;

	public Vector3 DesiredLookAtPoint
	{
		get { return _desiredLookAtPoint; }
		set { _desiredLookAtPoint = value; }
	}
	public Vector3 DesiredMovementDirection
	{
		get { return _desiredMovementDirection; }
		set { _desiredMovementDirection = value; }
	}

	private void Awake()
	{
		if (_shoulderSocket == null)
		{
			Debug.LogError("Shoulder Socket is not assigned.", this);
		}

		_groundLayerMask = LayerMask.GetMask(k_GroundLayer);

		if (_groundLayerMask == 0)
		{
			Debug.LogError($"Layer '{k_GroundLayer}' does not exist or is not configured.", this);
		}

		_collider = GetComponent<Collider>();
		_rigidBody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		ApplyJumpGravity();
		HandleMovement();
		CheckGrounded();
	}

	private void Update()
	{
		HandleLookAt();
	}

	public void Jump()
	{
		Debug.Log($"Called the method Jump(), _isGrounded is {_isGrounded}.", this);

		if (!_isGrounded)
		{
			return;
		}

		StartJumpPhase();
	}

	private void ApplyJumpGravity()
	{
		Vector3 velocity = _rigidBody.linearVelocity;
		_jumpPhaseActive = velocity.y < -_apexVelocityThreshold;
		float gravityMultiplier;

		if (_jumpPhaseActive && Mathf.Abs( velocity.y) <= _apexVelocityThreshold)
		{
			gravityMultiplier = _apexGravityMultiplier;
		}
		else if (velocity.y > 0F)
		{
			gravityMultiplier = _ascentGravityMultiplier;
		}
		else
		{
			gravityMultiplier = _fallGravityMultiplier;
		}

		_rigidBody.AddForce(Physics.gravity * (gravityMultiplier - 1F), ForceMode.Acceleration);
	}

	private void HandleMovement()
	{
		Vector3 movement = _desiredMovementDirection.normalized;
		movement *= _movementSpeed;
		movement.y = _rigidBody.linearVelocity.y;
		_rigidBody.linearVelocity = movement;
	}

	private void CheckGrounded()
	{
		float epsilon = 0.05F;
		Bounds bounds = _collider.bounds;
		Vector3 origin = new(bounds.center.x, bounds.min.y + epsilon, bounds.center.z);
		_isGrounded = Physics.Raycast(origin, Vector3.down, k_GroundCheckDistance, _groundLayerMask);
		// BUG: Grounded only if actually standing on an object from the "Ground" layer.
	}

	private void HandleLookAt()
	{
		if (_shoulderSocket == null)
		{
			return;
		}

		_shoulderSocket.transform.LookAt(_desiredLookAtPoint);
	}

	private void StartJumpPhase()
	{
		_jumpStartHeight = _rigidBody.position.y;
		_jumpPhaseActive = true;
		_isGrounded = false;

		Vector3 velocity = _rigidBody.linearVelocity;
		velocity.y = _jumpLaunchSpeed;
		_rigidBody.linearVelocity = velocity;
	}
}
