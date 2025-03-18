using Flam.Shapes;
using MoonTools.ECS;
using MoonWorks.Graphics;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Utility;

namespace TopDownShooter.Systems;

public class Spawner : MoonTools.ECS.System
{
    private readonly MoonTools.ECS.Filter _spawnFilter;

    public Spawner(World world)
        :base(world)
    {
        _spawnFilter = FilterBuilder
            .Include<SpawnTime>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        var sesstionTime = GetSingleton<SessionTimer>().Value;

        foreach (var entity in _spawnFilter.Entities)
        {
            var spawnTime = Get<SpawnTime>(entity).Value;

            if (Has<Rat>(entity) && sesstionTime >= spawnTime)
            {
                Set(entity, Color.Red);
                Set(entity, new ColliderUnion(new Rectangle(8, 8, 0, 0)));
                Set(entity, new Chaser());
                Set(entity, new DistanceCheck(150));
                Set(entity, new Velocity(Vector2.Zero));
                Set(entity, new Accerlation(1 * Constants.FRAME_RATE));
                Set(entity, new MaxSpeed(1.0f * Constants.FRAME_RATE));
                Set(entity, new CanBeFrozen());
                Set(entity, new CanKillOnHit());
                Remove<SpawnTime>(entity);
            }
        }
    }
}
