using Godot;

namespace cubejuggling.actor;

public partial class Actor : CharacterBody3D
{
    [Export] protected float MoveSpeed;

    [Export] private float _frictionValue = 6f;

    [Export] private float _gravityMultiplier = 1;

    private float _gravity;

    private Vector3 _originalPosition;

    public override void _Ready()
    {
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        _originalPosition = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        // gravity
        if (!IsOnFloor())
        {
            Velocity += Vector3.Down * _gravity * _gravityMultiplier * (float)delta;
        }

        // friction
        if (IsOnFloor())
        {
            float newX = Mathf.Lerp(Velocity.X, 0, _frictionValue * (float)delta);
            float newZ = Mathf.Lerp(Velocity.Z, 0, _frictionValue * (float)delta);
            Velocity = new Vector3(newX, Velocity.Y, newZ);
        }

        MoveAndSlide();

        // check if should reset to original position
        if (GlobalPosition.Y < -5)
        {
            GlobalPosition = _originalPosition;
            Velocity = Vector3.Zero;
        }
    }
}
