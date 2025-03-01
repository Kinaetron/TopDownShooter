using Flam.Collision;
using Flam.Shapes;
using MoonTools.ECS;
using MoonWorks.Graphics;
using MoonWorks.Input;
using System.Numerics;
using TopDownShooter.Components;

namespace TopDownShooter.Systems;

public class Motion : MoonTools.ECS.System
{
    private MoonTools.ECS.Filter _solidFilter;
    private MoonTools.ECS.Filter _velocityFilter;

    public Motion(World world)
        :base(world)
    {
        _solidFilter =
            FilterBuilder
            .Include<Solid>()
            .Include<Rectangle>()
            .Build();

        _velocityFilter =
            FilterBuilder
            .Include<Velocity>()
            .Include<Rectangle>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var velEntity in _velocityFilter.Entities)
        {
            var velocity = Get<Velocity>(velEntity).Value;
            var velocityBounds = Get<Rectangle>(velEntity);

            velocityBounds.Position += velocity;

            foreach (var solidEntity in _solidFilter.Entities)
            {
                var solidBounds = Get<Rectangle>(solidEntity);
                var collisionResult = CollisionDetection.MovingRectangleCollidesRectangle(velocityBounds, solidBounds, velocity);

                if (collisionResult.Hit)
                {
                    float epsilon = 0.01f;
                    velocityBounds.Position = collisionResult.ContactPoint + collisionResult.Normal * epsilon;

                    var remainingVelocity = velocity - Vector2.Dot(velocity, collisionResult.Normal) * collisionResult.Normal;
                    velocity = remainingVelocity;

                    Set(solidEntity, Color.Pink);
                }
                else
                {
                    Set(solidEntity, Color.White);
                }
            }

            Set(velEntity, velocity);
            Set(velEntity, velocityBounds);
        }
    }
}
