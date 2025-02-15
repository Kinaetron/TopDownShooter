using Flam.Collision;
using MoonTools.ECS;
using TopDownShooter.Components;
using TopDownShooter.Messages;

namespace TopDownShooter.Systems;

public class CollisionBehaviour : MoonTools.ECS.System
{
    private readonly Filter _playerFilter;
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
            }
        }
    }
}
