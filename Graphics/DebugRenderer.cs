using Flam.Graphics;
using Flam.Shapes;
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

    private readonly MoonTools.ECS.Filter _lineFilter;
    private readonly MoonTools.ECS.Filter _circleBoundsFilter;
    private readonly MoonTools.ECS.Filter _rectangleBoundsFilter;

    public DebugRenderer(
        uint resolutionX, 
        uint resolutionY,
        TitleStorage titleStorage,
        Window window,
        GraphicsDevice graphicsDevice,
        World world) : 
        base(world)
    {
        _rectangleBoundsFilter = FilterBuilder
            .Include<Color>()
            .Include<Rectangle>()
            .Build();

        _circleBoundsFilter = FilterBuilder
            .Include<Color>()
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

        foreach(var rectangleEntites in _rectangleBoundsFilter.Entities)
        {
            var color = Get<Color>(rectangleEntites);
            var bounds = Get<Rectangle>(rectangleEntites);

            _shapeBatcher.DrawFilledRectangle(bounds, MathHelper.TwoPi, color);
        }

        foreach (var circleEntites in _circleBoundsFilter.Entities)
        {
            var bounds = Get<CircleBounds>(circleEntites).Value;
            _shapeBatcher.DrawLineCircle(bounds, Color.Red);
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
