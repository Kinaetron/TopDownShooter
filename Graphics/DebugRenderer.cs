using Flam.Graphics;
using Flam.Shapes;
using MoonTools.ECS;
using MoonWorks;
using MoonWorks.Graphics;
using MoonWorks.Math;
using MoonWorks.Storage;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Utility;

namespace TopDownShooter.Graphics;

public class DebugRenderer : Renderer
{
    private readonly ShapeBatcher _shapeBatcher;

    private readonly MoonTools.ECS.Filter _lineFilter;
    private readonly MoonTools.ECS.Filter _circleBoundsFilter;
    private readonly MoonTools.ECS.Filter _rectangleColliderFilter;

    public DebugRenderer(
        uint resolutionX, 
        uint resolutionY,
        TitleStorage titleStorage,
        Window window,
        GraphicsDevice graphicsDevice,
        World world) : 
        base(world)
    {
        _rectangleColliderFilter = FilterBuilder
            .Include<Color>()
            .Include<Position>()
            .Include<Rectangle>()
            .Build();

        _circleBoundsFilter = FilterBuilder
            .Include<Color>()
            .Include<Position>()
            .Include<CircleBounds>()
            .Build();

        _lineFilter = FilterBuilder
            .Include<Color>()
            .Include<LineSegment>()
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

        foreach(var rectangleEntites in _rectangleColliderFilter.Entities)
        {
            var color = Get<Color>(rectangleEntites);
            var collider = Get<Rectangle>(rectangleEntites);
            var position = Get<Position>(rectangleEntites).Value;

            var colliderWorld = Helper.GetWorldRect(position, collider);

            _shapeBatcher.DrawFilledRectangle(colliderWorld, MathHelper.TwoPi, color);
        }

        foreach (var circleEntites in _circleBoundsFilter.Entities)
        {
            var collider = Get<CircleBounds>(circleEntites).Value;
            var position = Get<Position>(circleEntites).Value;

            var colliderWorld = Helper.GetWorldCircle(position, collider);

            _shapeBatcher.DrawLineCircle(colliderWorld, Color.Red);
        }

        foreach (var lineEntity in _lineFilter.Entities)
        {
            var color = Get<Color>(lineEntity);
            var line = Get<LineSegment>(lineEntity);
            _shapeBatcher.DrawLineSegment(line, color);
        }

        _shapeBatcher.End();
    }
}
