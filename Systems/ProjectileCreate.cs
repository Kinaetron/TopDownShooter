using MoonTools.ECS;
using TopDownShooter.Components;
using TopDownShooter.Messages;
using Timer = TopDownShooter.Components.Timer;

namespace TopDownShooter.Systems;

public class ProjectileCreate : MoonTools.ECS.System
{
    private readonly ProjectileController _projectileController;

    public ProjectileCreate(ProjectileController projectileController, World world)
        :base(world)
    {
        _projectileController = projectileController;
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

            var disableTime = Get<DisableTime>(message.Entity).Value;

            var position = Get<Position>(message.Entity).Value;
            var collider = Get<ColliderUnion>(message.Entity);

            var center = ColliderUnion.GetWorldCollider(position, collider).Rectangle.Center;
            var shootAwayFromPosition = Get<ShootAwayFromPosition>(message.Entity).Value;

            var shotPosition = center + message.Direction * shootAwayFromPosition;

            if(Has<CreatesBullets>(message.Entity))
            {
                _projectileController.SpawnHitBullet(shotPosition, message.Direction);
            }

            var timer = CreateEntity();
            Set(timer, new Timer(disableTime));
            Relate(timer, message.Entity, new DisableShoot());
        }
    }
}
