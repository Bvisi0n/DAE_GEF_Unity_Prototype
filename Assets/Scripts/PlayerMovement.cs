using UnityEngine;

// FEEDBACK: 26/10/14 9AM by Arene Six
// Movement feels stiff, use forces instead of setting velocity directly.
// Play some action platformers like Celeste and Hollow Knight to get a feel.
// You'll notice the character glides a accelerates and decelerates, keep it snappy tho!

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public sealed class PlayerMovement : MonoBehaviour
{
	private const float k_GroundCheckDistance = 0.2F;
	private const string k_GroundLayer = "Ground";

	[SerializeField] private float _movementSpeed = 7F;
	[SerializeField] private float _jumpLaunchSpeed = 11F;
	[SerializeField] private float _ascentGravityMultiplier = 2F;
	[SerializeField] private float _fallGravityMultiplier = 2F;
	[SerializeField] private float _apexGravityMultiplier = 0.5F;
	[SerializeField] private float _apexVelocityThreshold = 0.5F;

	private Collider _collider;
	private Rigidbody _rigidBody;

	private Vector3 _desiredMovementDirection;
	
	private bool _grounded;
	private bool _jumpPhaseActive;
	private float _jumpStartHeight;

	public Vector3 DesiredMovementDirection
	{
		get { return _desiredMovementDirection; }
		set { _desiredMovementDirection = value; }
	}

	private void Awake()
	{
		_collider = GetComponent<Collider>();
		_rigidBody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		ApplyJumpGravity();
		HandleMovement();
		CheckGrounded();
	}

	public void Jump()
	{
		if (!_grounded)
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
		int layerMask = LayerMask.GetMask(k_GroundLayer);
		_grounded = Physics.Raycast(origin, Vector3.down, k_GroundCheckDistance, layerMask);
		// BUG: Grounded only if actually standing on an object from the "Ground" layer.
	}

	private void StartJumpPhase()
	{
		_jumpStartHeight = _rigidBody.position.y;
		_jumpPhaseActive = true;
		_grounded = false;

		Vector3 velocity = _rigidBody.linearVelocity;
		velocity.y = _jumpLaunchSpeed;
		_rigidBody.linearVelocity = velocity;
	}
}
