using MoonTools.ECS;
using TopDownShooter.Components;
using TopDownShooter.Messages;

namespace TopDownShooter.Systems;

public class Collision : MoonTools.ECS.System
{
    public Collision(World world)
        :base(world)
    {
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var collision in ReadMessages<Collided>())
        {
           if(Has<CanBeFrozen>(collision.A) &&
             !Has<MarkedToFreeze>(collision.A) &&
              Has<Freezes>(collision.B))
            {
                var freezeTime = Get<Freezes>(collision.B).Value;
                Set(collision.A, new MarkedToFreeze(freezeTime));
            }
        }
    }
}
