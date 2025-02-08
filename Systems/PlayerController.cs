using Flam.Shapes;
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
            .Include<Velocity>()
            .Include<Accerlation>()
            .Include<MaxAcceleration>()
            .Include<RectangleBounds>()
            .Build();
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var entity in _playerFilter.Entities)
        {
            var velocity = Get<Velocity>(entity).Value;
            var bounds = Get<RectangleBounds>(entity).Value;
            var accerlation = Get<Accerlation>(entity).Value;
            var maxAccerlation = Get<MaxAcceleration>(entity).Value;

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

            if (direction.LengthSquared() > 1)
            {
                direction = Vector2.Normalize(direction);
            }

            if(_inputs.Mouse.LeftButton.IsPressed)
            {
                var shotDirection = Vector2.Normalize(new Vector2(_inputs.Mouse.X, _inputs.Mouse.Y) - bounds.Center);
                _bulletController.SpawnBullet(10 * Time.FRAME_RATE, 5, bounds.Position, shotDirection);
            }

            velocity.X += direction.X * accerlation * deltaTime;
            velocity.Y += direction.Y * accerlation * deltaTime;

            if (direction == Vector2.Zero)
            {
                velocity = Vector2.Zero;
            }


            float maxSpeed = maxAccerlation * deltaTime;
            if (velocity.Length() > maxSpeed)
            {
                velocity = Vector2.Normalize(velocity) * maxSpeed;
            }


            bounds.X += velocity.X;
            bounds.Y += velocity.Y;

            Set(entity, new Velocity(velocity));
            Set(entity, new RectangleBounds(new Rectangle(bounds.Width, bounds.Height, new Vector2(bounds.X, bounds.Y))));
        }
    }
}