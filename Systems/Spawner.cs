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

            if(sesstionTime >= spawnTime)
            {
                if (Has<Rat>(entity))
                {
                    Set(entity, Color.Red);
                    Set(entity, new ColliderUnion(new Rectangle(8, 8, 0, 0)));
                    Set(entity, new Chaser());
                    Set(entity, new DistanceCheck(150));
                    Set(entity, new Velocity(Vector2.Zero));
                    Set(entity, new Accerlation(1 * Constants.FRAME_RATE));
                    Set(entity, new MaxSpeed(1.5f * Constants.FRAME_RATE));
                    Set(entity, new CanBeFrozen());
                    Set(entity, new CanKillOnHit());
                    Set(entity, new OffsetChaser());
                    Remove<SpawnTime>(entity);
                }

                if (Has<Dog>(entity))
                {
                    Set(entity, Color.Beige);
                    Set(entity, new ColliderUnion(new Rectangle(16, 16, 0, 0)));
                    Set(entity, new Chaser());
                    Set(entity, new DistanceCheck(200));
                    Set(entity, new Velocity(Vector2.Zero));
                    Set(entity, new Accerlation(1 * Constants.FRAME_RATE));
                    Set(entity, new MaxSpeed(2.0f * Constants.FRAME_RATE));
                    Set(entity, new CanBeFrozen());
                    Set(entity, new CanKillOnHit());
                    Set(entity, new OffsetChaser());
                    Remove<SpawnTime>(entity);
                }

                if (Has<Turret>(entity))
                {
                    var collider = new ColliderUnion(new Rectangle(16, 16, 0, 0));
                    var position = Get<Position>(entity).Value;

                    var center = ColliderUnion.GetWorldCollider(position, collider).Rectangle.Center;

                    Set(entity, Color.Yellow);
                    Set(entity, collider);
                    Set(entity, new DistanceCheck(300));
                    Set(entity, new Velocity(Vector2.Zero));
                    Set(entity, new CanBeFrozen());
                    Set(entity, new Solid());
                    Set(entity, new ProjectileCreator());
                    Set(entity, new Radius(5.0f));
                    Set(entity, new MaxSpeed(5.0f * Constants.FRAME_RATE));
                    Set(entity, new DisableTime(2.0f));
                    Set(entity, new ShootAwayFromPosition(15));
                    Set(entity, new Center(center));
                    Set(entity, new CanBeFrozen());
                    Set(entity, new CreatesBullets());
                    Remove<SpawnTime>(entity);
                }

                if(Has<Blob>(entity))
                {
                    Set(entity, Color.DarkOliveGreen);
                    Set(entity, new ColliderUnion(new Rectangle(16, 16, 0, 0)));
                    Set(entity, new Chaser());
                    Set(entity, new DistanceCheck(200));
                    Set(entity, new Velocity(Vector2.Zero));
                    Set(entity, new Accerlation(0.5f * Constants.FRAME_RATE));
                    Set(entity, new MaxSpeed(0.5f * Constants.FRAME_RATE));
                    Set(entity, new CanBeFrozen());
                    Set(entity, new CanKillOnHit());
                    Set(entity, new ExplosionTrigger(50.0f));
                    Set(entity, new OffsetChaser());
                    Remove<SpawnTime>(entity);
                }
            }
        }
    }
}
