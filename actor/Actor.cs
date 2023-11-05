using Godot;

namespace cubejuggling.actor;

public partial class Actor : CharacterBody3D
{
    [Export] protected float MoveSpeed;

    [Export] private float _gravityMultiplier = 1;

    private float _gravity;

    public override void _Ready()
    {
        _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
    }

    public override void _PhysicsProcess(double delta)
    {
        // TODO: gravity
        if (!IsOnFloor())
        {
            Velocity += Vector3.Down * _gravity * (float)delta;
        }
    }
}
