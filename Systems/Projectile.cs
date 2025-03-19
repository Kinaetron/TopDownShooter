using Flam.Shapes;
using MoonTools.ECS;
using MoonWorks.Graphics;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Messages;
using Timer = TopDownShooter.Components.Timer;

namespace TopDownShooter.Systems;

public class Projectile : MoonTools.ECS.System
{
    public Projectile(World world)
        :base(world)
    {
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var message in ReadMessages<CreateProjectile>())
        {
            if (HasInRelation<DisableShoot>(message.Entity) || 
                HasInRelation<Frozen>(message.Entity))
            {
                continue;
            }

            var radius = Get<Radius>(message.Entity).Value;
            var maxSpeed = Get<MaxSpeed>(message.Entity).Value;
            var disableTime = Get<DisableTime>(message.Entity).Value;
            var center = Get<Center>(message.Entity).Value;
            var shootAwayFromPosition = Get<ShootAwayFromPosition>(message.Entity).Value;

            var shotPosition = center + message.Direction * shootAwayFromPosition;

            var bullet = CreateEntity();
            Set(bullet, Color.Red);
            Set(bullet, new MaxSpeed(maxSpeed));
            Set(bullet, new ColliderUnion(new Circle(radius, Vector2.Zero)));
            Set(bullet, new Velocity(Vector2.Zero));
            Set(bullet, new Direction(message.Direction));
            Set(bullet, new Position(shotPosition));
            Set(bullet, new CanKillOnHit());

            var timer = CreateEntity();
            Set(timer, new Timer(disableTime));
            Relate(timer, message.Entity, new DisableShoot());
        }
    }
}
