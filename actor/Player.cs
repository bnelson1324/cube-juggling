using Godot;

namespace cubejuggling.actor;

public partial class Player : Actor
{
    [Export] private float _cameraSensitivity;

    [Export] private PackedScene _projectile;

    private Camera3D _camera;

    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("Camera3D");
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // movement
        Vector2 input = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Vector3 movement = (_camera.Basis * new Vector3(input.X, 0, input.Y) * new Vector3(1, 0, 1)).Normalized();
        Velocity = Velocity * new Vector3(0, 1, 0) + movement * MoveSpeed;
        MoveAndSlide();

        // attack
        if (Input.IsActionJustPressed("attack_projectile"))
        {
            Projectile projectileInstance = _projectile.Instantiate<Projectile>();
            GetTree().Root.AddChild(projectileInstance);
            projectileInstance.Initialize(GlobalPosition, -_camera.GlobalTransform.Basis.Z.Normalized());
        }
        else if (Input.IsActionJustPressed("attack_hitscan"))
        {
            // TODO
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouse)
        {
            Vector2 mouseInput = eventMouse.Relative;
            Vector3 cameraRotation =
                _camera.Rotation + new Vector3(-mouseInput.Y, -mouseInput.X, 0) * _cameraSensitivity / 1000;
            cameraRotation.X = Mathf.Clamp(cameraRotation.X, -Mathf.Pi / 2, Mathf.Pi / 2);
            _camera.Rotation = cameraRotation;
        }
    }
}
