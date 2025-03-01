using Flam.Collision;
using Flam.Shapes;
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
            .Include<Rectangle>()
            .Include<BasicEnemyState>()
            .Include<Speed>()
            .Include<GameTimer>()
            .Include<FreezeTime>()
            .Build();

        _playerFilter = FilterBuilder
            .Include<Player>()
            .Include<Rectangle>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        var deltaTime = (float)delta.TotalSeconds;

        foreach (var playerEntity in _playerFilter.Entities)
        {
            var playerBounds = Get<Rectangle>(playerEntity);

            foreach (var entity in _basicEnemyFilter.Entities)
            {
                var state = Get<BasicEnemyState>(entity);
                var speed = Get<Speed>(entity).Value;
                var bounds = Get<Rectangle>(entity);
                var checkBounds = Get<CircleBounds>(entity).Value;

                var timer = Get<GameTimer>(entity).Value;

                switch (state)
                {
                    case BasicEnemyState.Wait:
                        if (CollisionDetection.CircleCollidesRectangle(checkBounds, playerBounds))
                        {
                            state = BasicEnemyState.Chase;
                        }
                        break;
                    case BasicEnemyState.Chase:
                        if (!CollisionDetection.CircleCollidesRectangle(checkBounds, playerBounds))
                        {
                            state = BasicEnemyState.Wait;
                        }
                        break;
                    case BasicEnemyState.Freeze:
                        if(timer <= TimeSpan.Zero)
                        {
                            state = BasicEnemyState.Wait;
                            Set(entity, new GameTimer(Get<FreezeTime>(entity).Value));
                        }
                        break;
                    default:
                        break;
                }

                if (state == BasicEnemyState.Chase)
                {
                    var directionToPlayer = Vector2.Normalize(playerBounds.Position - bounds.Position);

                    var actualSpeed = speed * directionToPlayer * deltaTime;
                    bounds.Position += actualSpeed;
                }

                if(state == BasicEnemyState.Freeze)
                {
                    timer -= delta;
                    Set(entity, new GameTimer(timer));
                }

                Set(entity, state);
                Set(entity, new Rectangle(bounds.Width, bounds.Height, bounds.Position));
                Set(entity, new CircleBounds(new Circle(checkBounds.Radius, new Vector2(bounds.Position.X + 8, bounds.Position.Y + 8))));
            }
        }
    }
}
