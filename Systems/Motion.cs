using Flam.Collision;
using Flam.Shapes;
using MoonTools.ECS;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Utility;

namespace TopDownShooter.Systems;

public class Motion : MoonTools.ECS.System
{
    private Filter _solidFilter;
    private Filter _velocityFilter;

    public Motion(World world)
        :base(world)
    {
        _solidFilter =
            FilterBuilder
            .Include<Solid>()
            .Include<Position>()
            .Include<Rectangle>()
            .Build();

        _velocityFilter =
            FilterBuilder
            .Include<Velocity>()
            .Include<Position>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var velEntity in _velocityFilter.Entities)
        {
            var velocity = Get<Velocity>(velEntity).Value;
            var position = Get<Position>(velEntity).Value;

            position += velocity;

            foreach (var solidEntity in _solidFilter.Entities)
            {
                if(!Has<Rectangle>(velEntity))
                {
                    break;
                }

                var solidCollider = Get<Rectangle>(solidEntity);
                var solidPosition = Get<Position>(solidEntity).Value;
                var solidColliderWorld = Helper.GetWorldRect(solidPosition, solidCollider);

                var velocityCollider = Get<Rectangle>(velEntity);
                var velocityColliderWorld = Helper.GetWorldRect(position, velocityCollider);

                var collisionResult = CollisionDetection.MovingRectangleCollidesRectangle(velocityColliderWorld, solidColliderWorld, velocity);

                if (collisionResult.Hit)
                {
                    float epsilon = 0.01f;
                    position = collisionResult.ContactPoint + collisionResult.Normal * epsilon;

                    var remainingVelocity = velocity - Vector2.Dot(velocity, collisionResult.Normal) * collisionResult.Normal;
                    velocity = remainingVelocity;
                }
            }

            Set(velEntity, new Velocity(velocity));
            Set(velEntity, new Position(position));
        }
    }
}
