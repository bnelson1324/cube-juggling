using Godot;

namespace cubejuggling.actor;

public partial class Projectile : Area3D
{
    [Export] private float _speed;

    private Vector3 _direction;

    public void Initialize(Vector3 position, Vector3 direction)
    {
        GlobalPosition = position;
        _direction = direction.Normalized();
        LookAt(GlobalPosition + _direction);
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += _direction * _speed * (float)delta;
    }
}
