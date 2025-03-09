using Flam.Graphics;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using MoonWorks.Math;
using MoonWorks.Storage;
using System.Numerics;
using TopDownShooter.Components;

namespace TopDownShooter.Graphics;

public class DebugRenderer : Renderer
{
    private readonly ShapeBatcher _shapeBatcher;

    private readonly MoonTools.ECS.Filter _colliderFilter;

    public DebugRenderer(
        uint resolutionX, 
        uint resolutionY,
        TitleStorage titleStorage,
        Window window,
        GraphicsDevice graphicsDevice,
        World world) : 
        base(world)
    {
        _colliderFilter = FilterBuilder
            .Include<Color>()
            .Include<Position>()
            .Include<ColliderUnion>()
            .Build();

        _shapeBatcher = new ShapeBatcher(
            resolutionX,
            resolutionY,
            titleStorage,
            window,
            graphicsDevice);
    }

    public void Render(double alpha)
    {
        _shapeBatcher.Begin(Color.CornflowerBlue, Matrix4x4.Identity);

        foreach(var entites in _colliderFilter.Entities)
        {
            var color = Get<Color>(entites);
            var collider = Get<ColliderUnion>(entites);
            var position = Get<Position>(entites).Value;

            var colliderWorld = ColliderUnion.GetWorldCollider(position, collider);

            switch (colliderWorld.Type)
            {
                case ColliderUnion.ColliderType.Rectangle:
                    _shapeBatcher.DrawFilledRectangle(colliderWorld.Rectangle, MathHelper.TwoPi, color);
                    break;

                case ColliderUnion.ColliderType.Circle:
                    _shapeBatcher.DrawLineCircle(colliderWorld.Circle, color);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        _shapeBatcher.End();
    }
}
