using System.Collections.Generic;
using System.Linq;
using Godot;

namespace cubejuggling.actor;

public partial class Projectile : Area3D
{
    [Signal]
    public delegate void ExplodedEventHandler();

    [Export] private float _speed;
    [Export] private float _knockbackMultiplier;
    [Export] private float _explosionRadius;

    private Vector3 _direction;
    private Rid _explosionSpherecastRid;
    private ulong _createTime;

    public void Initialize(Vector3 position, Vector3 direction, Actor owner)
    {
        // transform
        GlobalPosition = position;
        _direction = direction.Normalized();
        LookAt(GlobalPosition + _direction);

        // misc
        _explosionSpherecastRid = PhysicsServer3D.SphereShapeCreate();
        PhysicsServer3D.ShapeSetData(_explosionSpherecastRid, _explosionRadius);
        _createTime = Time.GetTicksMsec();

        // collision
        BodyEntered += other =>
        {
            HashSet<ulong> immuneActors = new() { owner.GetInstanceId() };

            // contacting enemy
            if (other is Actor actor)
            {
                if (other is Player)
                    return;

                HitActor(actor, immuneActors, 1);
            }

            // explode, knocking back everything except immuneActors
            // spherecast to get list of nearby enemies
            var spaceState = GetWorld3D().DirectSpaceState;
            PhysicsShapeQueryParameters3D query = new();
            query.ShapeRid = _explosionSpherecastRid;
            query.CollisionMask = 0b10; // only collide w/ actors
            query.Transform = new Transform3D(Basis.Identity, GlobalPosition);
            List<Actor> results = spaceState.IntersectShape(query)
                .Select(dict => (Actor)dict["collider"])
                .ToList();

            // knockback actors in results
            foreach (Actor a in results)
            {
                // check for obstruction
                var ray = PhysicsRayQueryParameters3D.Create(GlobalPosition, a.GlobalPosition, 0b1);
                var res = spaceState.IntersectRay(ray);
                if (res.ContainsKey("position"))
                {
                    continue;
                }

                // damage actor if no obstruction
                HitActor(a, immuneActors, a.GlobalPosition.DistanceTo(GlobalPosition) / _explosionRadius);
            }

            // emit signal
            EmitSignal(SignalName.Exploded);

            // destroy projectile
            QueueFree();
        };
    }

    private void HitActor(Actor actor, HashSet<ulong> immuneActors, float damageMultiplier)
    {
        if (immuneActors.Contains(actor.GetInstanceId()))
            return;

        Vector3 knockback = (actor.GlobalPosition - GlobalPosition).Normalized()
                            * _knockbackMultiplier * damageMultiplier;
        actor.Velocity += knockback;
        immuneActors.Add(actor.GetInstanceId());
    }

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition += _direction * _speed * (float)delta;

        // projectile lifetime
        if (Time.GetTicksMsec() - _createTime > 10000)
            QueueFree();
    }
}
