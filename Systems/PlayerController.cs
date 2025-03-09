using MoonTools.ECS;
using MoonWorks.Input;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Utility;

namespace TopDownShooter.Systems;

public class PlayerController : MoonTools.ECS.System
{
    private readonly Inputs _inputs;
    private readonly BulletController _bulletController;

    private readonly Filter _playerFilter;

    public PlayerController(BulletController bulletController, Inputs inputs, World world)
        :base(world)
    {
        _inputs = inputs;
        _bulletController = bulletController;

        _playerFilter =
            FilterBuilder
            .Include<Player>()
            .Include<Position>()
            .Include<MaxSpeed>()
            .Include<Velocity>()
            .Include<Accerlation>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _playerFilter.Entities)
        {
            var position = Get<Position>(entity).Value;
            var velocity = Get<Velocity>(entity).Value;
            var maxSpeed = Get<MaxSpeed>(entity).Value;
            var accerlation = Get<Accerlation>(entity).Value;

            var deltaTime = (float)delta.TotalSeconds;
            var direction = new Vector2();

            if (_inputs.Keyboard.IsDown(KeyCode.A))
            {
                direction.X = -1;
            }
            if (_inputs.Keyboard.IsDown(KeyCode.D))
            {
                direction.X = 1;
            }
            if (_inputs.Keyboard.IsDown(KeyCode.W))
            {
                direction.Y = -1;
            }
            if(_inputs.Keyboard.IsDown(KeyCode.S))
            {
                direction.Y = 1;
            }

            velocity += direction * accerlation * deltaTime;

            if(velocity.Length() > maxSpeed * deltaTime)
            {
                velocity = Vector2.Normalize(velocity) * maxSpeed * deltaTime;
            }

            if(direction.LengthSquared() <= 0)
            {
                velocity = Vector2.Zero;
            }

            Set(entity, new Velocity(velocity));

            if (_inputs.Mouse.LeftButton.IsPressed)
            {
                var shotDirection = Vector2.Normalize(new Vector2(_inputs.Mouse.X, _inputs.Mouse.Y) - position);
                _bulletController.SpawnBullet(10 * Constants.FRAME_RATE, 5, position, shotDirection);
            }
        }
    }
}