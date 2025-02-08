using Flam.Shapes;
using MoonTools.ECS;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Utility;

namespace TopDownShooter.Systems;

public class BulletController : MoonTools.ECS.System
{
    private readonly Filter _bulletFilter;

    public BulletController(World world)
        :base(world)
    {
        _bulletFilter =
            FilterBuilder
            .Include<Bullet>()
            .Include<CircleBounds>()
            .Include<Speed>()
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
        Set(bullet, new Bullet());
        Set(bullet, new CircleBounds(new Circle(radius, position)));
        Set(bullet, new Speed(speed));
        Set(bullet, new Direction(direction));
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _bulletFilter.Entities)
        {
            var speed = Get<Speed>(entity).Value;
            var direction = Get<Direction>(entity).Value;
            var bounds = Get<CircleBounds>(entity).Value;

            var deltaTime = (float)delta.TotalSeconds;

            bounds.Position += direction * speed * deltaTime;
            Set(entity, new CircleBounds(new Circle(bounds.Radius, bounds.Position)));
        }
    }
}
