using Flam.Collision;
using Flam.Shapes;
using MoonTools.ECS;
using System.Numerics;
using TopDownShooter.Components;

namespace TopDownShooter.Systems;

public class TileBoundsCollsionResolution : MoonTools.ECS.System
{
    private readonly Filter _movingObjectFilter;
    private readonly Filter _collisionTileFilter;

    public TileBoundsCollsionResolution(World world)
        : base(world)
    {
        _collisionTileFilter =
            FilterBuilder
            .Include<Tile>()
            .Include<RectangleBounds>()
            .Build();

        _movingObjectFilter =
            FilterBuilder
            .Include<Velocity>()
            .Include<Remainder>()
            .Include<RectangleBounds>()
            .Build();
    }


    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _movingObjectFilter.Entities)
        {
            var velocity = Get<Velocity>(entity).Value;
            var remainder = Get<Remainder>(entity).Value;
            var bounds = Get<RectangleBounds>(entity).Value;

            var positionX = HorizontalMovement(entity, remainder, velocity, bounds);
            var positionY = VerticalMovement(entity, remainder, velocity, bounds);

            Set(entity, new RectangleBounds(
                new Rectangle(bounds.Width, bounds.Height, new Vector2(positionX, positionY))));
        }
    }

    private float HorizontalMovement(
        Entity entity,
        Vector2 remainder,
        Vector2 velocity,
        Rectangle bounds)
    {
        remainder.X += velocity.X;
        Set(entity, new Remainder(remainder));

        int move = (int)remainder.X;

        if(move != 0)
        {
            remainder.X -= move;
            int sign = Math.Sign(move);

            while (move != 0)
            {
                var newBounds = new Rectangle(bounds.Width, bounds.Height, bounds.Position + new Vector2(sign, 0));

                if (CheckAgainstTiles(newBounds))
                {
                    remainder.X = 0;
                    break;
                }

                move -= sign;
                bounds.Position += new Vector2(sign, 0);
            }
        }

        Set(entity, new Remainder(remainder));
        return bounds.Position.X;
    }

    private float VerticalMovement(
        Entity entity,
        Vector2 remainder,
        Vector2 velocity,
        Rectangle bounds)
    {
        remainder.Y += velocity.Y;
        int move = (int)remainder.Y;

        if (move < 0)
        {
            remainder.Y -= move;
            while (move != 0)
            {
                var newBounds = new Rectangle(bounds.Width, bounds.Height, bounds.Position + new Vector2(0, -1));

                if (CheckAgainstTiles(newBounds))
                {
                    remainder.Y = 0;
                    break;
                }

                bounds.Position += new Vector2(0, -1);
                move += 1;
            }
        }
        else if (move > 0)
        {
            remainder.Y -= move;

            while (move != 0)
            {
                var newBounds = new Rectangle(bounds.Width, bounds.Height, bounds.Position + new Vector2(0, 1));

                if (CheckAgainstTiles(newBounds))
                {
                    remainder.Y = 0;
                    break;
                }

                move -= 1;
                bounds.Position += new Vector2(0, 1);
            }
        }

        Set(entity, new Remainder(remainder));
        return bounds.Position.Y;
    }

    private bool CheckAgainstTiles(Rectangle bounds)
    {
        foreach (var tile in _collisionTileFilter.Entities)
        {
            var tileBounds = Get<RectangleBounds>(tile).Value;
            if (CollisionDetection.RectangleCollidesRectangle(bounds, tileBounds))
            {
                return true;
            }
        }

        return false;
    }
}
