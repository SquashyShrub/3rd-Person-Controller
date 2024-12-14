using Godot;

namespace D_Controller_3.Empty.Scenes.Player;

public partial class CameraMovement : Node3D
{
	//Variables
	private CharacterBody3D _player;

	private Vector3 _pitchRotation;
	private float _pitch = 0;
	private float _pitchMin = -60;
	private float _pitchMax = 89;
	private float _pitchAcceleration = 15;
	private float _pitchSensitivity = 0.5f;
	
	private Vector3 _yawRotation;
	private float _yaw = 0;
	private float _yawAcceleration = 15;
	private float _yawSensitivity = 0.5f;
	
	public override void _Ready()
	{
		//Getting player node
		_player = GetNode<CharacterBody3D>(".");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			
		}
	}
}