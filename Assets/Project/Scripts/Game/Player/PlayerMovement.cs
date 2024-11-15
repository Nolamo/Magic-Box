using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PlayerInputs))]
public class PlayerMovement : MonoBehaviour, IPlayerComponent
{
    // input
    private PlayerInputs _playerInputs;
    private bool _inputsAreSubscribed;

    private Rigidbody _rb;
    private CapsuleCollider _capsule;

    [Header("Camera")]
    [Tooltip("Camera offset from top of the capsule collider")]
    [SerializeField] private float _eyeLevelOffset;
    [SerializeField] private Transform _lookOrientation;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _lookSensitivity;
    [SerializeField] private Interactor _interactor;

    private Vector2 _mouseInput;
    private bool _isCameraFrozen;

    [Header("Grounded")]
    [SerializeField] private float _groundAngle;
    [SerializeField] private LayerMask _whatIsGround;
    private bool _isGrounded;
    [SerializeField] private float _springTargetDistance = 0.15f;
    [SerializeField] private float _springDistance = 0.3f;
    [SerializeField] private float _springStrength;
    [SerializeField] private float _springDamper;
    private float _currentSpringDistance;

    // defaults are close to realistic human movement metrics

    [Header("Locomotion")]
    [SerializeField] private float _walkSpeed = 1.6f;
    [SerializeField] private float _sprintSpeed = 3.2f;
    [SerializeField] private float _acceleration = 20;
    [SerializeField] private float _airControl = 0.1f;
    private bool _isSprinting;
    private float _movementSpeed;

    private Vector2 _movementInput;

    [Header("Crouching")]
    [SerializeField] private float _crouchSpeed = 0.55f;
    [SerializeField] private float _crouchHeight;
    private float _standingHeight;
    private bool _isTryingToCrouch;
    private bool _isCrouching;

    private Vector3 _crouchingCapsuleCenter;
    private Vector3 _standingCapsuleCenter;

    [Header("Jumping")]
    [SerializeField] private float _jumpHeight = 0.7f;
    [SerializeField] private float _groundCancelDuration = 0.1f;
    private float _groundCancelDelay;

    void OnEnable()
    {
        SubscribeInputs();
    }

    void OnDisable()
    {
        UnsubscribeInputs();
    }

    public void SubscribeInputs()
    {
        if (_playerInputs == null) _playerInputs = GetComponent<PlayerInputs>();
        if (_inputsAreSubscribed) return;
        _playerInputs.inputActions.Player.Move.performed += OnMove;
        _playerInputs.inputActions.Player.Move.canceled += OnMove;

        _playerInputs.inputActions.Player.Look.performed += OnLook;
        _playerInputs.inputActions.Player.Look.canceled += OnLook;

        _playerInputs.inputActions.Player.Jump.performed += OnJump;
        _playerInputs.inputActions.Player.Jump.canceled += OnJump;

        _playerInputs.inputActions.Player.Sprint.performed += OnSprint;
        _playerInputs.inputActions.Player.Sprint.canceled += OnSprint;

        _playerInputs.inputActions.Player.Crouch.performed += OnCrouch;
        _playerInputs.inputActions.Player.Crouch.canceled += OnCrouch;

        _playerInputs.inputActions.Player.Rotate.performed += OnRotate;
        _playerInputs.inputActions.Player.Rotate.canceled += OnRotate;

    }

    public void UnsubscribeInputs()
    {
        if (!_inputsAreSubscribed) return;
        _playerInputs.inputActions.Player.Move.performed -= OnMove;
        _playerInputs.inputActions.Player.Move.canceled -= OnMove;
        _playerInputs.inputActions.Player.Look.performed -= OnLook;
        _playerInputs.inputActions.Player.Look.canceled -= OnLook;
        _playerInputs.inputActions.Player.Jump.performed -= OnJump;
        _playerInputs.inputActions.Player.Jump.canceled -= OnJump;
        _playerInputs.inputActions.Player.Sprint.performed -= OnSprint;
        _playerInputs.inputActions.Player.Sprint.canceled -= OnSprint;
        _playerInputs.inputActions.Player.Crouch.performed -= OnCrouch;
        _playerInputs.inputActions.Player.Crouch.canceled -= OnCrouch;

        _playerInputs.inputActions.Player.Rotate.performed -= OnRotate;
        _playerInputs.inputActions.Player.Rotate.canceled -= OnRotate;
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.performed == true)
        {
            if (_interactor == null) return;
            if(_interactor.grabbedObject != null) { _isCameraFrozen = true; }
        }
        else if (context.canceled == true)
        {
            _isCameraFrozen = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        _movementInput = value;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        _mouseInput = value;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isGrounded == false) return;
            Jump(_jumpHeight);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (_isCrouching) return;
        if (context.performed)
        {
            _movementSpeed = _sprintSpeed;
        }
        else if (context.canceled)
        {
            _movementSpeed = _walkSpeed;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isTryingToCrouch = true;
        }
        else if (context.canceled)
        {
            _isTryingToCrouch = false;
        }
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _capsule = GetComponent<CapsuleCollider>();
        _movementSpeed = _walkSpeed;

        _standingHeight = _capsule.height - _springTargetDistance;
        _capsule.height = _standingHeight;
        _crouchHeight = _crouchHeight - _springTargetDistance;

        _crouchingCapsuleCenter = Vector3.up * _crouchHeight / 2;
        _standingCapsuleCenter = Vector3.up * _standingHeight / 2;
        _capsule.center = _standingCapsuleCenter;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    private void Jump(float height)
    {
        _groundCancelDelay = _groundCancelDuration;

        // cancel current y velocity
        _rb.AddForce(Vector3.up * -_rb.velocity.y, ForceMode.VelocityChange);

        // Calculate the required jump velocity to reach the desired height
        Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * height);
        jumpVelocity *= _rb.mass;

        _rb.AddForce(jumpVelocity, ForceMode.Impulse);
    }

    void Update()
    {
        Look();
    }

    private void FixedUpdate()
    {
        GroundCheck();
        Move(_movementInput, _movementSpeed);

        if (_groundCancelDelay > 0) _groundCancelDelay -= Time.fixedDeltaTime;

        if( _isTryingToCrouch)
        {
            if (_isCrouching == false) Crouch();
        }
        else
        {
            //bool canUncrouch;
            if (_isCrouching)
            {
                AttemptUnCrouch();
            }
        }
    }

    public void Move(Vector2 direction, float maxSpeed)
    {
        direction = direction.normalized;

        Vector3 _movementVector = (_lookOrientation.forward * direction.y) + (_lookOrientation.right * direction.x);

        float acceleration = _acceleration * _rb.mass;
        if(_isGrounded == false)
        {
            acceleration *= _airControl;
        }


        Vector3 flatVelocity = Vector3.Scale(_rb.velocity, new Vector3(1, 0, 1));
        float speed = flatVelocity.magnitude;

        // slow down when not moving and grounded
        if (_movementVector.magnitude < 0.01f && _isGrounded)
        {
            _rb.AddForce(-flatVelocity * acceleration);
            return;
        }

        Vector3 force = _movementVector * acceleration;
        
        _rb.AddForce(force, ForceMode.Force);

        // add counter force
        if(flatVelocity.magnitude > maxSpeed)
        {
            Vector3 excessVelocity = flatVelocity.normalized * (speed - maxSpeed);
            _rb.AddForce(-excessVelocity * acceleration);
        }
    }

    private void Look()
    {
        if (_isCameraFrozen) return;

        Vector2 lookVector = _mouseInput * _lookSensitivity;

        Vector3 orientationRotation = _lookOrientation.rotation.eulerAngles + new Vector3(0, lookVector.x, 0);
        _lookOrientation.rotation = Quaternion.Euler(orientationRotation);

        Vector3 lookRotation = _cameraTransform.rotation.eulerAngles + new Vector3(-lookVector.y, 0, 0);
        lookRotation.x = ClampAngle(lookRotation.x, -90, 90);
        _cameraTransform.rotation = Quaternion.Euler(lookRotation);
    }

    private float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    private void GroundSpring(RaycastHit groundHit)
    {
        // calculate spring force to push the player off of the ground with

        float totalDistance = groundHit.distance - _capsule.center.y;

        _currentSpringDistance = totalDistance;

        Rigidbody hitRB = groundHit.collider.GetComponent<Rigidbody>();

        float displacement = _springTargetDistance - totalDistance;
        float springForce = displacement * _springStrength;

        float dampForce = _rb.velocity.y * _springDamper;

        Vector3 force = Vector3.up * (springForce - dampForce);
        
        _rb.AddForce(force * _rb.mass);

        // add an equivalent force to the rigidbody the player might be standing on
        if(hitRB != null)
        {
            hitRB.AddForceAtPosition(-force, groundHit.point);
        }
    }

    private void GroundCheck()
    {
        if (_groundCancelDelay > 0)
            return;

        // check for valid ground by casting a sphere downwards

        Ray ray = new Ray(_rb.position + _capsule.center, Vector3.down);
        RaycastHit hit;

        if (Physics.SphereCast(ray, _capsule.radius - 0.01f, out hit, _springDistance + _capsule.center.y, _whatIsGround, QueryTriggerInteraction.Ignore))
        {
            if (IsGround(hit.normal) == false)
                return;
        }
        else 
            return;

        _isGrounded = true;

        GroundSpring(hit);

        CancelInvoke("StopGrounded");
        Invoke("StopGrounded", 0.1f);
    }

    /* - COLLISION-BASED GROUNDED DETECTION METHOD-
     * replaced by capsule-based which is above this
     
    void GroundCheck(Collision collision)
    {
        if ((_whatIsGround & (1 << collision.gameObject.layer)) == 0)
            // bad layer
            return;

        bool validAngle = false;

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (IsGround(collision.GetContact(i).normal))
            {
                validAngle = true;
                break;
            }
        }
        if (validAngle == false)
            // bad angle
            return;

        _isGrounded = true;

        CancelInvoke("StopGrounded");
        Invoke("StopGrounded", 0.1f);
    }
    */

    // add some shit to prevent sliding down slopes

    private bool IsGround(Vector3 normal)
    {
        return Vector3.Angle(normal, Vector3.up) < _groundAngle;
    }

    private void StopGrounded()
    {
        _isGrounded = false;
    }

    private void Crouch()
    {
        _isCrouching = true;
        _movementSpeed = _crouchSpeed;

        _capsule.center = _crouchingCapsuleCenter + (Vector3.up * _springTargetDistance);

        _capsule.height = _crouchHeight;
        _cameraTransform.localPosition = Vector3.up * (_capsule.height + _eyeLevelOffset);

        if (_isGrounded == false)
        {
            float crouchBoostHeight = _standingHeight - _crouchHeight;

            _rb.position += Vector3.up * crouchBoostHeight;
        }

        // todo: animate camera moving up/down
    }

    private void AttemptUnCrouch()
    {
        Vector3 targetPosition = _rb.position;
        RaycastHit hit;

        bool _hitGround = false;

        if (_isGrounded == false)
        {
            float crouchBoostHeight = _standingHeight - _crouchHeight;
            float rayDistance = (_crouchHeight / 2) + crouchBoostHeight;

            Debug.DrawLine(_rb.position + _crouchingCapsuleCenter, (_rb.position + _crouchingCapsuleCenter) + (Vector3.down * rayDistance), Color.red, 1);

            targetPosition = _rb.position - Vector3.up * crouchBoostHeight;

            Ray bottomCapsuleRay = new Ray(_rb.position + _crouchingCapsuleCenter, Vector3.down);

            if (Physics.SphereCast(bottomCapsuleRay, _capsule.radius - 0.01f, out hit, rayDistance, _whatIsGround))
            {
                targetPosition = new Vector3(_rb.position.x, hit.point.y, _rb.position.z);
                _hitGround = true;
            }
        }

        Ray topCapsuleRay = new Ray(targetPosition, Vector3.up);
        if(Physics.SphereCast(topCapsuleRay, _capsule.radius - 0.01f, _standingHeight, _whatIsGround))
        {
            // uncrouch failure!
            return;
        }

        if (_hitGround) { _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.y); }

        // uncrouch is successful!

        _rb.position = targetPosition;
        _capsule.center = _standingCapsuleCenter;

        _capsule.height = _standingHeight;
        _cameraTransform.localPosition = Vector3.up * (_capsule.height + _eyeLevelOffset);

        _isCrouching = false;
        if (_isSprinting)
        {
            _movementSpeed = _sprintSpeed;
        }
        else
        {
            _movementSpeed = _walkSpeed;
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_rb.position + Vector3.down * (_capsule.radius - _springTargetDistance), _capsule.radius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_rb.position + Vector3.down * (_capsule.radius - _springDistance), _capsule.radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_rb.position + Vector3.down * _currentSpringDistance, _capsule.radius);

    }
}
