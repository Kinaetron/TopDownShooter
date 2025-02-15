using Flam.Collision;
using MoonTools.ECS;
using TopDownShooter.Components;
using TopDownShooter.Messages;
using TopDownShooter.Utility;

namespace TopDownShooter.Systems;

public class CollisionBehaviour : MoonTools.ECS.System
{
    private readonly Filter _playerFilter;
    private readonly Filter _bullterFilter;
    private readonly Filter _basicEnemyFilter;


    public CollisionBehaviour(World world)
        :base(world)
    {
        _playerFilter = FilterBuilder
            .Include<Player>()
            .Include<RectangleBounds>()
            .Build();

        _basicEnemyFilter = FilterBuilder
            .Include<BasicEnemy>()
            .Include<RectangleBounds>()
            .Include<BasicEnemyState>()
            .Build();

        _bullterFilter = FilterBuilder
            .Include<Bullet>()
            .Include<CircleBounds>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var playerEntity in _playerFilter.Entities)
        {
            var playerBounds = Get<RectangleBounds>(playerEntity).Value;

            foreach (var basicEnemyEntity in _basicEnemyFilter.Entities)
            {
                var basicEnemyBounds = Get<RectangleBounds>(basicEnemyEntity).Value;

                if(CollisionDetection
                    .RectangleCollidesRectangle(playerBounds, basicEnemyBounds))
                {
                    Send(new EndGame());
                }

                foreach (var bulletEnemy in _bullterFilter.Entities)
                {
                    var bulletBounds = Get<CircleBounds>(bulletEnemy).Value;

                    if (CollisionDetection
                        .CircleCollidesRectangle(bulletBounds, basicEnemyBounds))
                    {
                        Set(basicEnemyEntity, BasicEnemyState.Freeze);
                        Destroy(bulletEnemy);
                    }
                }
            }
        }
    }
}
