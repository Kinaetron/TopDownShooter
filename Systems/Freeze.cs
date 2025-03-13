using MoonTools.ECS;
using TopDownShooter.Components;
using Timer = TopDownShooter.Components.Timer;

namespace TopDownShooter.Systems;

public class Freeze : MoonTools.ECS.System
{
    private readonly Filter _markFreezeFilter;

    public Freeze(World world)
        :base(world)
    {
        _markFreezeFilter =
            FilterBuilder
            .Include<MarkedToFreeze>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _markFreezeFilter.Entities)
        {
            if(!Has<Frozen>(entity))
            {
                var freezeTime = Get<MarkedToFreeze>(entity).Value;
                var timer = CreateEntity();
                Set(timer, new Timer(freezeTime));
                Remove<MarkedToFreeze>(entity);
                Relate(timer, entity, new Frozen());
            }
        }
    }
}
