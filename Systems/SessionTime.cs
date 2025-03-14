using MoonTools.ECS;
using TopDownShooter.Components;

namespace TopDownShooter.Systems;

public class SessionTime : MoonTools.ECS.System
{
    public SessionTime(World world)
        :base(world)
    {
    }

    public override void Update(TimeSpan delta)
    {
        var entity = GetSingletonEntity<SessionTimer>();
        var timer = Get<SessionTimer>(entity).Value;

        Set(entity, new SessionTimer(timer += (float)delta.TotalSeconds));
    }
}
