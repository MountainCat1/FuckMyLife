using System;
using UnityEngine;
using Zenject;

public enum CreatureState
{
    Idle,
    Moving,
    Attacking,
    Utility
}

[RequireComponent(typeof(Rigidbody2D))]
public class Creature : MonoBehaviour
{
    // Events
    public event Action<Vector2> Moved;
    public event Action<CreatureState> StateChanged;

    // Injected Dependencies (using Zenject)
    [Inject] private DiContainer _diContainer;

    // Public Variables
    public Rigidbody2D Rigidbody2D => _rigidbody2D;

    public CreatureState State
    {
        get => _state;
        private set
        {
            if (_state == value)
                return;

            _state = value;
            StateChanged?.Invoke(_state);
        }
        
    }
    private CreatureState _state = CreatureState.Idle;

    // Serialized Private Variables
    [field: Header("Movement")]
    [field: SerializeField]
    public float Drag { get; private set; }

    [field: SerializeField] public float BaseSpeed { get; set; }
    
    // Private Variables

    private Transform _rootTransform;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _moveDirection;
    private Vector2 _momentum;
    private Creature _lastAttackedBy = null;
    private Collider2D _collider;

    private const float MomentumLoss = 2f;

    // Properties

    // Unity Callbacks
    private void Awake()
    {
        _rootTransform = transform;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        UpdateVelocity();
        
        if (_moveDirection.magnitude > 0)
            State = CreatureState.Moving;
        else
            State = CreatureState.Idle;
    }

    // Public Methods
    public void SetMovement(Vector2 direction)
    {
        _moveDirection = direction.normalized;
    }
    
    public static bool IsCreature(GameObject go)
    {
        return go.CompareTag("Player") || go.CompareTag("Creature");
    }
    
    // Virtual Methods

    // Abstract Methods

    // Private Methods
    private float GetSpeed()
    {
        return BaseSpeed;
    }

    private void UpdateVelocity()
    {
        _momentum -= _momentum * (MomentumLoss * Time.fixedDeltaTime);
        if (_momentum.magnitude < 0.1f)
            _momentum = Vector2.zero;

        var change = Vector2.MoveTowards(_rigidbody2D.velocity, _moveDirection * GetSpeed() + _momentum,
            Drag * Time.fixedDeltaTime);
        _rigidbody2D.velocity = change;

        Moved?.Invoke(change);
    }

    public void Push(Vector2 push)
    {
        _momentum = push;
    }
    


    // Event Handlers

}

public struct DeathContext
{
    public Creature Killer { get; set; }
    public Creature Creature { get; set; }
}