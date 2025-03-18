using MoonTools.ECS;
using System.Numerics;
using TopDownShooter.Components;

namespace TopDownShooter.Systems;

public class Camera : MoonTools.ECS.System
{
    private readonly Vector2 _levelSize;
    private readonly Vector2 _screenSize;

    private readonly Filter _cameraFilter;
    private readonly Filter _playerFilter;

    public Camera(
        float levelSizeX,
        float levelSizeY,
        float screenSizeX,
        float screenSizeY,
        World world
        )
        :base(world)
    {
        _levelSize = new Vector2(levelSizeX, levelSizeY);
        _screenSize = new Vector2(screenSizeX, screenSizeY);

        _cameraFilter =
            FilterBuilder
            .Include<Translate>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        var posEntity = GetSingletonEntity<Player>();
        var position = Get<Position>(posEntity).Value;

        foreach (var cameraEntity in _cameraFilter.Entities)
        {
            var translate = position;
            translate.X = float.Clamp(translate.X, _screenSize.X / 2, _levelSize.X - _screenSize.X / 2);
            translate.Y = float.Clamp(translate.Y, _screenSize.Y / 2, _levelSize.Y - _screenSize.Y / 2);

            translate -= new Vector2(_screenSize.X / 2, _screenSize.Y / 2);

            Set(cameraEntity, new Translate(translate));
        }
    }
}
