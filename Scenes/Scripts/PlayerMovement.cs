using Godot;

namespace D_Controller_3.Empty.Scenes.Scripts;

public partial class PlayerMovement : CharacterBody3D
{
    //Basic Variables
    [Export] private float _movementSpeed = 5.0f;
    [Export] private float _sprintMultiplier = 2.0f;
    [Export] private float _jumpStrength = 5.0f;
    [Export] private float _highJumpMultiplier = 2.0f;
    
    //Node Variables
    private AnimationPlayer _animationPlayer;
    private Node3D _visuals;
    
    //Variable
    private bool _isLocked = false;
    
    //World
    private float _gravity = -9.81f;
    private Vector3 _velocity = Vector3.Zero;

    //Camera Movement Variables
    private Node3D _cameraMount;
    [Export] private float _pitchSensitivity = 0.5f;
    [Export] private float _yawSensitivity = 0.5f;
    
    public override void _Ready()
    {
        //Get Camera Mount Node
        _cameraMount = GetNode<Node3D>("CameraMount");
        
        //Get AnimationPlayer and Visuals Nodes
        _animationPlayer = GetNode<AnimationPlayer>("CollisionShape3D/Visuals/AuxScene/AnimationPlayer");
        _visuals = GetNode<Node3D>("CollisionShape3D/Visuals");
        
        //Capture the mouse
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            RotateY(Mathf.DegToRad(-eventMouseMotion.Relative.X * _pitchSensitivity));
            _visuals.RotateY(Mathf.DegToRad(eventMouseMotion.Relative.X * _pitchSensitivity)); //Makes player turn the opposite direction of camera to make look static
            _cameraMount.RotateX(-Mathf.DegToRad(eventMouseMotion.Relative.Y * _yawSensitivity));
        }
    }
    
    public override void _PhysicsProcess(double delta)
    {
        //Movement
        if (!Input.IsActionPressed("sprint"))
            HandleWalkingMovement((float)delta);
        if (Input.IsActionPressed("sprint"))
            HandleSprintingMovement((float)delta);
        
        //Attack
        if (!_animationPlayer.IsPlaying())
            _isLocked = false;
        UpdateAttackAnimations();
    }
    
    //Movement
    private Vector3 GetInputDirection()
    {
        //Reset the horizontal velocity
        Vector3 inputDirection = Vector3.Zero;
		
        //Capture player input
        if (Input.IsActionPressed("move_forward"))
            inputDirection -= Transform.Basis.Z;
        if (Input.IsActionPressed("move_backward"))
            inputDirection += Transform.Basis.Z;
        if (Input.IsActionPressed("move_left"))
            inputDirection -= Transform.Basis.X;
        if (Input.IsActionPressed("move_right"))
            inputDirection += Transform.Basis.X;
        
        //Normalize direction and return it
        return inputDirection.Normalized();
    }
    private void HandleWalkingMovement(float delta)
    {
        Vector3 inputDirection = GetInputDirection();
        
        if (!_isLocked)
            _visuals.LookAt(Position + inputDirection); //Points player in direction of movement
        
        _velocity.X = inputDirection.X * _movementSpeed;
        _velocity.Z = inputDirection.Z * _movementSpeed;
        
        HandleJumpAndGravity(delta);

        Velocity = _velocity;
        UpdateMovementAnimations();
        if (!_isLocked)
            MoveAndSlide();
    }
    private void HandleSprintingMovement(float delta)
    {
        Vector3 inputDirection = GetInputDirection();
        
        if (!_isLocked)
            _visuals.LookAt(Position + inputDirection); //Points player in direction of movement
        
        _velocity.X = inputDirection.X * _movementSpeed * _sprintMultiplier;
        _velocity.Z = inputDirection.Z * _movementSpeed * _sprintMultiplier;
        
        HandleJumpAndGravity(delta);
        
        Velocity = _velocity;
        UpdateMovementAnimations();
        if (!_isLocked)
            MoveAndSlide();
    }
    private void HandleJumpAndGravity(float delta)
    {
        if (!IsOnFloor())
            _velocity.Y += _gravity * delta;
        else
        {
            if (Input.IsActionJustPressed("jump"))
                _velocity.Y = _jumpStrength;
            if (Input.IsActionJustPressed("high_jump"))
                _velocity.Y = _jumpStrength * _highJumpMultiplier;
        }
    }
    
    //Attacks
    private void HandleBasicAttack()
    {
        //if (Input.IsActionJustPressed("punch"))
            //attack and damage logic
    }
    
    //Animations
    private void UpdateMovementAnimations()
    {
        if (_isLocked) return;
        if (!IsOnFloor())
        {
            _animationPlayer.Play("Jumping");
        }
        else if (Mathf.Abs(Velocity.X) > 0.01f || Mathf.Abs(Velocity.Z) > 0.01f)
        {
            _animationPlayer.Play(Input.IsActionPressed("sprint") ? "FastRun" : "Walking");
        }
        else
        {
            _animationPlayer.Play("Idle");
        }
    }
    private void UpdateAttackAnimations()
    {
        if (Input.IsActionJustPressed("punch"))
        {
            _animationPlayer.Play("Punching");
            _isLocked = true;
        }
    }
}