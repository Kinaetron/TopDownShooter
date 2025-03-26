using Flam.Shapes;
using MoonTools.ECS;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Messages;
using Timer = TopDownShooter.Components.Timer;

namespace TopDownShooter.Systems;

public class Explode : MoonTools.ECS.System
{
    public Explode(World world)
        :base(world)
    {
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var initialize in ReadMessages<InitializeExplosion>())
        {
            var position = Get<Position>(initialize.Entity).Value;

            Remove<Chaser>(initialize.Entity);
            Remove<ExplosionTrigger>(initialize.Entity);
            Set(initialize.Entity, new Exploding());
            Set(initialize.Entity, new Velocity(Vector2.Zero));
            Set(initialize.Entity, new ExplosionRadius(new Circle(75.0f, position)));

            var timer = CreateEntity();
            Set(timer, new Timer(1.0f));
            Relate(timer, initialize.Entity, new ExplosionCountDown());
        }

        foreach (var exploded in ReadMessages<Exploded>())
        {
            if(exploded.MarkDeath)
            {
                Send(new EndGame());
            }
            else
            {
                Set(exploded.Entity, new MarkedToDestroy());
            }
        }

    }
}
