using MoonTools.ECS;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Utility;

namespace TopDownShooter.Systems;

public class BasicEnemySystem : MoonTools.ECS.System
{
    private readonly Filter _playerFilter;
    private readonly Filter _basicEnemyFilter;

    public BasicEnemySystem(World world)
        :base(world)
    {
        _basicEnemyFilter = FilterBuilder
            .Include<BasicEnemy>()
            .Include<ColliderUnion>()
            .Include<BasicEnemyState>()
            .Include<Velocity>()
            .Include<MaxSpeed>()
            .Include<DistanceCheck>()
            .Build();

        _playerFilter = FilterBuilder
            .Include<Player>()
            .Include<Position>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        var deltaTime = (float)delta.TotalSeconds;

        foreach (var playerEntity in _playerFilter.Entities)
        {
            var playerPosition = Get<Position>(playerEntity).Value;

            foreach (var enemyEntity in _basicEnemyFilter.Entities)
            {
                var distanceAway = Get<DistanceCheck>(enemyEntity).Value;
                var enemyPosition = Get<Position>(enemyEntity).Value;
                var enemyToPlayerDistance = (enemyPosition - playerPosition).Length();

                var state = Get<BasicEnemyState>(enemyEntity);
                var velocity = Get<Velocity>(enemyEntity).Value;

                switch (state)
                {
                    case BasicEnemyState.Wait:
                        if (enemyToPlayerDistance <= distanceAway)
                        {
                            state = BasicEnemyState.Chase;
                        }
                        break;
                    case BasicEnemyState.Chase:
                        if (enemyToPlayerDistance > distanceAway)
                        {
                            state = BasicEnemyState.Wait;
                        }
                        break;
                    default:
                        break;
                }

                if (state == BasicEnemyState.Chase)
                {
                    var maxSpeed = Get<MaxSpeed>(enemyEntity).Value;
                    var directionToPlayer = Vector2.Normalize(playerPosition - enemyPosition);
                    velocity = maxSpeed * directionToPlayer * deltaTime;
                }
                else if(state == BasicEnemyState.Wait)
                {
                    velocity = Vector2.Zero;
                }

                    Set(enemyEntity, state);
                Set(enemyEntity, new Velocity(velocity));
            }
        }
    }
}
