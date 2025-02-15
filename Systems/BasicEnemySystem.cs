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
            .Include<RectangleBounds>()
            .Include<BasicEnemyState>()
            .Include<Speed>()
            .Build();

        _playerFilter = FilterBuilder
            .Include<Player>()
            .Include<RectangleBounds>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        var deltaTime = (float)delta.TotalSeconds;

        foreach (var playerEntity in _playerFilter.Entities)
        {
            foreach (var entity in _basicEnemyFilter.Entities)
            {
                var state = Get<BasicEnemyState>(entity);
                var speed = Get<Speed>(entity).Value;
                var bounds = Get<RectangleBounds>(entity).Value;
                var checkBounds = Get<CircleBounds>(entity).Value;

                var playerBounds = Get<RectangleBounds>(playerEntity).Value;

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
                    default:
                        break;
                }

                if (state == BasicEnemyState.Chase)
                {
                    var directionToPlayer = Vector2.Normalize(playerBounds.Position - bounds.Position);

                    var actualSpeed = speed * directionToPlayer * deltaTime;
                    bounds.Position += actualSpeed;
                }

                Set(entity, state);
                Set(entity, new RectangleBounds(new Rectangle(bounds.Width, bounds.Height, bounds.Position)));
                Set(entity, new CircleBounds(new Circle(checkBounds.Radius, new Vector2(bounds.Position.X + 8, bounds.Position.Y + 8))));
            }
        }
    }
}
