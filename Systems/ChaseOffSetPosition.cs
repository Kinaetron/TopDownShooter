using MoonTools.ECS;
using TopDownShooter.Components;

namespace TopDownShooter.Systems;

public class ChaseOffSetPosition : MoonTools.ECS.System
{

    private readonly Filter _offSetChaseFilter;
    private readonly Filter _offSetChaseTargetFilter;
    private readonly Filter _offsetChaseAssignedFilter;

    public ChaseOffSetPosition(World world)
        :base(world)
    {
        _offSetChaseTargetFilter = FilterBuilder
            .Include<OffsetChaseTarget>()
            .Build();

        _offSetChaseFilter = FilterBuilder
            .Include<OffsetPosition>()
            .Build();

        _offsetChaseAssignedFilter = FilterBuilder
            .Include<ColliderUnion>()
            .Include<OffsetPosition>()
            .Include<Position>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var targetEntity in _offSetChaseTargetFilter.Entities)
        {
            var targetPosition = Get<Position>(targetEntity).Value;
            var targetCollider = Get<ColliderUnion>(targetEntity);

            var center = ColliderUnion.GetWorldCollider(targetPosition, targetCollider).Rectangle.Center;

            foreach (var entity in _offSetChaseFilter.Entities)
            {
                var offsetPosition = Get<OffsetPosition>(entity).Value;
                var position = center + offsetPosition;
                Set(entity, new Position(position));
            }
        }
    }
}
