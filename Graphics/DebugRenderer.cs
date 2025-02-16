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
            .Include<RectangleBounds>()
            .Build();

        _circleBoundsFilter = FilterBuilder
            .Include<Color>()
            .Include<CircleBounds>()
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
            var bounds = Get<RectangleBounds>(rectangleEntites).Value;

            _shapeBatcher.DrawFilledRectangle(bounds, MathHelper.TwoPi, color);
        }

        foreach (var circleEntites in _circleBoundsFilter.Entities)
        {
            var bounds = Get<CircleBounds>(circleEntites).Value;
            _shapeBatcher.DrawLineCircle(bounds, Color.Red);
        }

        _shapeBatcher.End();
    }
}
