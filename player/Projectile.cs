using Godot;

namespace cubejuggling.player;

public partial class Projectile : Area3D
{
    [Export] private Vector3 _speed;

    private Vector3 _velocity;

    public void Initialize(Vector3 velocity)
    {
        _velocity = velocity;
        LookAt(GlobalPosition + _velocity);
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += _velocity * _speed * (float)delta;
    }
}
