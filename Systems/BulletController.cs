using Flam.Shapes;
using MoonTools.ECS;
using MoonWorks.Graphics;
using System.Numerics;
using TopDownShooter.Components;

namespace TopDownShooter.Systems;

public class BulletController : MoonTools.ECS.System
{
    private readonly MoonTools.ECS.Filter _bulletFilter;

    public BulletController(World world)
        :base(world)
    {
        _bulletFilter =
            FilterBuilder
            .Include<Velocity>()
            .Include<Direction>()
            .Build();
    }

    public void SpawnBullet(
        float speed, 
        float radius, 
        Vector2 position, 
        Vector2 direction)
    {
        var bullet = CreateEntity();
        Set(bullet, Color.Red);
        Set(bullet, new MaxSpeed(speed));
        Set(bullet, new ColliderUnion(new Circle(radius, Vector2.Zero)));
        Set(bullet, new Velocity(Vector2.Zero));
        Set(bullet, new Direction(direction));
        Set(bullet, new Position(position));
        Set(bullet, new Freezes(1.0f));
        Set(bullet, new DestroyOnHit());
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _bulletFilter.Entities)
        {
            var maxSpeed = Get<MaxSpeed>(entity).Value;
            var direction = Get<Direction>(entity).Value;

            var deltaTime = (float)delta.TotalSeconds;

            var velocity = direction * maxSpeed * deltaTime;

            Set(entity, new Velocity(velocity));
        }
    }
}
