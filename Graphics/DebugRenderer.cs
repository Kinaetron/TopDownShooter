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

    private readonly MoonTools.ECS.Filter _playerFilter;
    private readonly MoonTools.ECS.Filter _bulletFilter;
    private readonly MoonTools.ECS.Filter _basicEnemyFilter;

    public DebugRenderer(
        uint resolutionX, 
        uint resolutionY,
        TitleStorage titleStorage,
        Window window,
        GraphicsDevice graphicsDevice,
        World world) : 
        base(world)
    {
        _playerFilter = FilterBuilder
            .Include<Player>()
            .Include<Color>()
            .Include<RectangleBounds>()
            .Build();

        _bulletFilter = FilterBuilder
            .Include<Bullet>()
            .Include<CircleBounds>()
            .Build();

        _basicEnemyFilter = FilterBuilder
            .Include<BasicEnemy>()
            .Include<Color>()
            .Include<RectangleBounds>()
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

        foreach(var playerEntity in _playerFilter.Entities)
        {
            var color = Get<Color>(playerEntity);
            var bounds = Get<RectangleBounds>(playerEntity).Value;

            _shapeBatcher.DrawFilledRectangle(bounds, MathHelper.TwoPi, color);
        }

        foreach (var basicEnemyEntity in _basicEnemyFilter.Entities)
        {
            var color = Get<Color>(basicEnemyEntity);
            var bounds = Get<RectangleBounds>(basicEnemyEntity).Value;
            var circleBounds = Get<CircleBounds>(basicEnemyEntity).Value;

            _shapeBatcher.DrawFilledRectangle(bounds, MathHelper.TwoPi, color);
            _shapeBatcher.DrawLineCircle(circleBounds, color);
        }

        foreach (var bulletEntity in _bulletFilter.Entities)
        {
            var bounds = Get<CircleBounds>(bulletEntity).Value;
            _shapeBatcher.DrawLineCircle(bounds, Color.Red);
        }

        _shapeBatcher.End();
    }
}
