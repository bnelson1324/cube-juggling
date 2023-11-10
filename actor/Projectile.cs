using System.Collections.Generic;
using Godot;

namespace cubejuggling.actor;

public partial class Projectile : Area3D
{
    [Export] private float _speed;
    [Export] private float _knockbackMultiplier;

    private Vector3 _direction;

    public void Initialize(Vector3 position, Vector3 direction)
    {
        GlobalPosition = position;
        _direction = direction.Normalized();
        LookAt(GlobalPosition + _direction);

        // collision
        BodyEntered += other =>
        {
            HashSet<ulong> hitEnemies = new();

            // contacting enemy
            if (other is Actor actor)
            {
                if (other is Player)
                    return;

                Vector3 knockback = (actor.GlobalPosition - GlobalPosition).Normalized() * _knockbackMultiplier;
                actor.Velocity += knockback;

                hitEnemies.Add(other.GetInstanceId());
                // TODO explode and knockback everything nearby (except the same enemy)
            }
            
            // explode, damaging everything except hitEnemies
            // TODO
            
            QueueFree();
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += _direction * _speed * (float)delta;
    }
}
