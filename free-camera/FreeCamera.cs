using Godot;
using System;

public partial class FreeCamera : Camera3D
{
	Vector3 _velocity = Vector3.Zero;
	float _rotationX = 0;
	float _rotationY = 0;
	float _rotationXDelta = 0;
	float _rotationYDelta = 0;

	[Export]
	// In m/s
	float _maxMovementSpeed = 1.0f;
	[Export]
	// in rad/s
	float _maxRotationSpeed = 1.0f;

	[ExportGroup("Mouse")]
	[Export]
	bool _useMouse = true;
	[Export]
	float _mouseSensitivity = 0.5f;
	[Export]
	Key _mouseRotateKey = Key.None;

	[ExportGroup("Keyboard")]
	[Export]
	bool _useKeyboardMovement = true;
	[Export]
	Key _keyboardLeftKey = Key.A;
	[Export]
	Key _keyboardRightKey = Key.D;
	[Export]
	Key _keyboardForwardKey = Key.W;
	[Export]
	Key _keyboardBackwardKey = Key.S;
	bool _keyboardLeftIsPressed = false;
	bool _keyboardRightIsPressed = false;
	bool _keyboardForwardIsPressed = false;
	bool _keyboardBackwardIsPressed = false;

	[ExportGroup("Gamepad")]
	[Export]
	bool UseGamepad = true;
	[Export]
	string _gamepadMoveLeft = "move_axis_left";
	[Export]
	string _gamepadMoveRight = "move_axis_right";
	[Export]
	string _gamepadMoveForward = "move_axis_forward";
	[Export]
	string _gamepadMoveBackward = "move_axis_backward";
	[Export]
	string _gamepadLookLeft = "look_axis_left";
	[Export]
	string _gamepadLookRight = "look_axis_right";
	[Export]
	string _gamepadLookUp = "look_axis_up";
	[Export]
	string _gamepadLookDown = "look_axis_down";


	public override void _Ready() {
		UseGamepad = InputMap.HasAction(_gamepadLookLeft)
		&& InputMap.HasAction(_gamepadLookRight)
		&& InputMap.HasAction(_gamepadMoveForward)
		&& InputMap.HasAction(_gamepadMoveBackward)
		&& InputMap.HasAction(_gamepadLookLeft)
		&& InputMap.HasAction(_gamepadLookRight)
		&& InputMap.HasAction(_gamepadLookUp)
		&& InputMap.HasAction(_gamepadLookDown);
	}

	public override void _Process(double delta) {
		if (_useMouse) {
			_rotationY += (_rotationYDelta * (float) delta * _mouseSensitivity);
			_rotationX += (_rotationXDelta * (float) delta * _mouseSensitivity);
		}
		_rotationYDelta = 0;
		_rotationXDelta = 0;

		Vector3 movementVec = Vector3.Zero;

		// Gamepad Look and Movement
		if (UseGamepad) {
			Vector2 gamepadMove = Input.GetVector(_gamepadMoveLeft, _gamepadMoveRight, _gamepadMoveForward, _gamepadMoveBackward);
			Vector2 gamepadLook = Input.GetVector(_gamepadLookRight, _gamepadLookLeft, _gamepadLookDown, _gamepadLookUp);
			_rotationY += gamepadLook.Y * _maxRotationSpeed * (float) delta;
			_rotationX += gamepadLook.X * _maxRotationSpeed * (float) delta;
			movementVec.X += gamepadMove.X * _maxMovementSpeed * (float) delta;
			movementVec.Z += gamepadMove.Y * _maxMovementSpeed * (float) delta;
		}

		// Keyboard movement
		if (_useKeyboardMovement) {
			movementVec += new Vector3(
				Convert.ToByte(_keyboardRightIsPressed) - Convert.ToByte(_keyboardLeftIsPressed),
				0,
				Convert.ToByte(_keyboardBackwardIsPressed) - Convert.ToByte(_keyboardForwardIsPressed)
			) * _maxMovementSpeed * (float) delta;
		}

		if (movementVec.Length() > _maxMovementSpeed) {
			movementVec = movementVec.Normalized() * _maxMovementSpeed;
		}
		Translate(movementVec);

		// Apply rotation and translation
		Transform3D transform = Transform;
		transform.Basis = Basis.Identity;
		Transform = transform;
		_rotationY = Mathf.Clamp(_rotationY, -Mathf.Pi / 2, Mathf.Pi / 2);
		RotateObjectLocal(Vector3.Up, _rotationX); // first rotate about Y
		RotateObjectLocal(Vector3.Right, _rotationY); // then rotate about X
	}

	public override void _Input(InputEvent @event) {
		if (_useMouse && @event is InputEventMouseMotion mouseEvent && Input.MouseMode == Input.MouseModeEnum.Captured) {		
			_rotationYDelta += mouseEvent.Relative.Y;
			_rotationXDelta += mouseEvent.Relative.X;
		}

		if (_useKeyboardMovement && @event is InputEventKey keyEvent) {
			if (keyEvent.Keycode == _keyboardForwardKey) _keyboardForwardIsPressed = keyEvent.Pressed;
			else if (keyEvent.Keycode == _keyboardBackwardKey) _keyboardBackwardIsPressed = keyEvent.Pressed;
			else if (keyEvent.Keycode == _keyboardLeftKey) _keyboardLeftIsPressed = keyEvent.Pressed;
			else if (keyEvent.Keycode == _keyboardRightKey) _keyboardRightIsPressed = keyEvent.Pressed;

			if (keyEvent.Keycode == Key.Ctrl) {
				Input.MouseMode = keyEvent.Pressed ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
			}
		}
	}

	public void UpdateLook() {

	}
}
