using Flam.Shapes;
using MoonTools.ECS;
using MoonWorks.Graphics;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Utility;

namespace TopDownShooter.Systems;

public class ProjectileController : MoonTools.ECS.System
{
    private readonly float _normalBulletRadius = 5.0f;
    private readonly float _normalBulletFreezeTime = 1.0f;
    private readonly float _normalBulletSpeed = 10 * Constants.FRAME_RATE;

    private readonly float _specialBulletRadius = 7.0f;
    private readonly float _specialBulletFreezeTime = 5.0f;
    private readonly float _specialBulletSpeed = 7 * Constants.FRAME_RATE;

    private readonly float _discRadius = 10.0f;
    private readonly float _discSpeed = 3 * Constants.FRAME_RATE;

    private readonly MoonTools.ECS.Filter _projectileFilter;

    public ProjectileController(World world)
        :base(world)
    {
        _projectileFilter =
            FilterBuilder
            .Include<Velocity>()
            .Include<Direction>()
            .Build();
    }

    public void SpawnNormalBullet(
        Vector2 position, 
        Vector2 direction)
    {
        var bullet = CreateEntity();
        Set(bullet, Color.Red);
        Set(bullet, new MaxSpeed(_normalBulletSpeed));
        Set(bullet, new ColliderUnion(new Circle(_normalBulletRadius, Vector2.Zero)));
        Set(bullet, new Velocity(Vector2.Zero));
        Set(bullet, new Direction(direction));
        Set(bullet, new Position(position));
        Set(bullet, new Freezes(_normalBulletFreezeTime));
        Set(bullet, new DestroyOnHit());
    }

    public void SpawnHitBullet(
        Vector2 position,
        Vector2 direction)
    {
        var bullet = CreateEntity();
        Set(bullet, Color.Red);
        Set(bullet, new MaxSpeed(_normalBulletSpeed));
        Set(bullet, new ColliderUnion(new Circle(_normalBulletRadius, Vector2.Zero)));
        Set(bullet, new Velocity(Vector2.Zero));
        Set(bullet, new Direction(direction));
        Set(bullet, new Position(position));
        Set(bullet, new Freezes(_normalBulletFreezeTime));
        Set(bullet, new CanKillOnHit());
        Set(bullet, new DestroyOnHit());
    }

    public void SpawnSpecialBullet(
         Vector2 position,
         Vector2 direction)
    {
        var specialBullet = CreateEntity();
        Set(specialBullet, Color.Red);
        Set(specialBullet, new MaxSpeed(_specialBulletSpeed));
        Set(specialBullet, new ColliderUnion(new Circle(_specialBulletRadius, Vector2.Zero)));
        Set(specialBullet, new Velocity(Vector2.Zero));
        Set(specialBullet, new Direction(direction));
        Set(specialBullet, new Position(position));
        Set(specialBullet, new Freezes(_specialBulletFreezeTime));
        Set(specialBullet, new DestroyOnHit());
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _projectileFilter.Entities)
        {
            var maxSpeed = Get<MaxSpeed>(entity).Value;
            var direction = Get<Direction>(entity).Value;

            var deltaTime = (float)delta.TotalSeconds;

            var velocity = direction * maxSpeed * deltaTime;

            Set(entity, new Velocity(velocity));
        }
    }
}
