using MoonTools.ECS;
using TopDownShooter.Components;
using TopDownShooter.Messages;

namespace TopDownShooter.Systems;

public class SelectChaseOffset : MoonTools.ECS.System
{
    private readonly Filter _offsetChaseAssignedFilter;

    public SelectChaseOffset(World world)
        :base(world)
    {
        _offsetChaseAssignedFilter = FilterBuilder
          .Include<ColliderUnion>()
          .Include<OffsetChase>()
          .Include<Position>()
          .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var selectEntity in ReadMessages<SelectChaseOffSet>())
        {
            var position = Get<Position>(selectEntity.Value).Value;
            var collider = Get<ColliderUnion>(selectEntity.Value);

            var center = ColliderUnion.GetWorldCollider(position, collider).Rectangle.Center;
            float closestDistanceSqr = float.MaxValue;
            Entity closestTarget = CreateEntity();

            foreach (var offsetEntity in _offsetChaseAssignedFilter.Entities)
            {
                if (Has<OffsetOccupied>(offsetEntity))
                {
                    continue;
                }

                var offsetPosition = Get<Position>(offsetEntity).Value;
                var difference = center - offsetPosition;
                float distanceSqr = difference.LengthSquared();

                if (distanceSqr < closestDistanceSqr)
                {
                    closestTarget = offsetEntity;
                    closestDistanceSqr = distanceSqr;
                }
            }


            if (closestDistanceSqr != float.MaxValue && !HasOutRelation<ChasingOffSet>(selectEntity.Value))
            {
                Set(closestTarget, new OffsetOccupied());
                Relate(selectEntity.Value, closestTarget, new ChasingOffSet());
            }
        }
    }
}
