using MoonTools.ECS;
using Timer = TopDownShooter.Components.Timer;

namespace TopDownShooter.Systems;

public class Time : MoonTools.ECS.System
{
    public Filter _timeFilter;

    public Time(World world)
        :base(world)
    {
        _timeFilter =
            FilterBuilder
            .Include<Timer>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _timeFilter.Entities)
        {
            var timer = Get<Timer>(entity);
            var t = timer.Time - (float)delta.TotalSeconds;

            if(t <= 0.0f)
            {
                Destroy(entity);
            }
            else
            {
                Set(entity, timer.Update(t));
            }
        }
    }
}
