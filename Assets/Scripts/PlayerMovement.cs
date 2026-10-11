using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public sealed class PlayerMovement : MonoBehaviour
{
	private const float k_GroundCheckDistance = 0.2f;
	private const string k_GroundLayer = "Ground";

	[SerializeField] private float _movementSpeed = 7f;
	[SerializeField] private float _jumpStrength = 7f;

	private Collider _collider;
	private Rigidbody _rigidBody;
	private Vector3 _desiredMovementDirection;
	private bool _grounded;

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
		HandleMovement();
		CheckGrounded();
	}

	public void Jump()
	{
		if (!_grounded)
		{
			return;
		}

		_rigidBody.AddForce(Vector3.up * _jumpStrength, ForceMode.Impulse);
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
	}
}
