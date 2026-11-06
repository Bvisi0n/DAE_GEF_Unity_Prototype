using UnityEngine;

// TODO: Rework - Movement doesn't feel quite right.
// Research required:
// GDC Festival of Gaming: Math for Game Programmers: Building a Better Jump
// https://youtu.be/hG9SzQxaCm8?si=HR7YLmNufOJKsG6R

namespace Assets.Scripts.Player
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public sealed class Movement : MonoBehaviour
	{
		private const float k_GroundCheckDistance = 0.2F;

		[SerializeField] private GameObject _shoulderSocket;
		[SerializeField] private float _movementSpeed = 7F;
		[SerializeField] private LayerMask _jumpableLayers;
		[SerializeField] private float _jumpLaunchSpeed = 11F;
		[SerializeField] private float _ascentGravityMultiplier = 2F;
		[SerializeField] private float _fallGravityMultiplier = 2F;
		[SerializeField] private float _apexGravityMultiplier = 0.5F;
		[SerializeField] private float _apexVelocityThreshold = 0.5F;

		private Collider _collider;
		private Rigidbody _rigidBody;

		private bool _isGrounded;
		private bool _jumpPhaseActive;

		public Vector3 DesiredLookAtPoint { get; set; }
		public Vector3 DesiredMovementDirection { get; set; }

		private void Awake()
		{
			if (_shoulderSocket == null)
			{
				Debug.LogError("Shoulder Socket is not assigned.", this);
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

			if (_jumpPhaseActive && Mathf.Abs(velocity.y) <= _apexVelocityThreshold)
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
			Vector3 movement = DesiredMovementDirection.normalized;
			movement *= _movementSpeed;
			movement.y = _rigidBody.linearVelocity.y;
			_rigidBody.linearVelocity = movement;
		}

		/// <summary>
		/// Checks wheter the object is touching a surface and sets the bool _grounded accordingly.
		/// </summary>
		private void CheckGrounded()
		{
			float epsilon = 0.05F;
			Bounds bounds = _collider.bounds;
			Vector3 origin = new(bounds.center.x, bounds.min.y + epsilon, bounds.center.z);
			_isGrounded = Physics.Raycast(origin, Vector3.down, k_GroundCheckDistance, _jumpableLayers);
		}

		/// <summary>
		/// Aims the "gun" at the DesiredLookAtPoint, usually the mouse.
		/// </summary>
		private void HandleLookAt()
		{
			if (_shoulderSocket == null)
			{
				return;
			}

			// TODO: Bug - Player can aim inside itself.
			_shoulderSocket.transform.LookAt(DesiredLookAtPoint);
		}

		private void StartJumpPhase()
		{
			_jumpPhaseActive = true;
			_isGrounded = false;

			Vector3 velocity = _rigidBody.linearVelocity;
			velocity.y = _jumpLaunchSpeed;
			_rigidBody.linearVelocity = velocity;
		}
	}
}
