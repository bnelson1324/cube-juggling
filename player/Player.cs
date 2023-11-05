using Godot;

namespace cubejuggling.player;

public partial class Player : CharacterBody3D
{
    [Export] private float _cameraSensitivity;

    [Export] private float _moveSpeed;

    [Export] private PackedScene _projectile;

    private Camera3D _camera;

    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("Camera3D");
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Process(double delta)
    {
        Vector2 input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Vector3 movement = (Basis * new Vector3(input.X, 0, input.Y) * new Vector3(1, 0, 1)).Normalized();
        Velocity = movement * _moveSpeed * (float)delta;
        MoveAndSlide();
    }

    // TODO: on mouse click fire Projectile

    // TODO: check projectile works

    public void OnMouseMotion(Vector2 mouseInput)
    {
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouse)
        {
            Vector2 mouseInput = eventMouse.Relative;
            Vector3 cameraRotation =
                _camera.Rotation + new Vector3(-mouseInput.Y, -mouseInput.X, 0) * _cameraSensitivity / 1000;
            cameraRotation.X = Mathf.Clamp(cameraRotation.X, -Mathf.Pi / 6, Mathf.Pi / 6);
            _camera.Rotation = cameraRotation;
        }
    }
}
