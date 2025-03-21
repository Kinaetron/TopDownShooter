﻿using MoonTools.ECS;
using MoonWorks.Input;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Utility;
using Timer = TopDownShooter.Components.Timer;

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
            .Include<ColliderUnion>()
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

            if(_inputs.Mouse.LeftButton.IsPressed || _inputs.Mouse.RightButton.IsPressed)
            {
                if (HasInRelation<DisableShoot>(entity))
                {
                    continue;
                }

                var cameraTranslation = GetSingleton<Translate>().Value;

                var collider = Get<ColliderUnion>(entity);
                var colliderRect = ColliderUnion.GetWorldCollider(position, collider).Rectangle;

                var worldMousePosition = new Vector2(_inputs.Mouse.X, _inputs.Mouse.Y) + cameraTranslation;
                var shotDirection = Vector2.Normalize(new Vector2(worldMousePosition.X, worldMousePosition.Y) - position);

                if(_inputs.Mouse.LeftButton.IsPressed)
                {
                    var shotPosition = colliderRect.Center + shotDirection * 10;
                    _bulletController.SpawnBullet(10 * Constants.FRAME_RATE, 5, 1.0f, shotPosition, shotDirection);

                    var timer = CreateEntity();
                    Set(timer, new Timer(0.15f));
                    Relate(timer, entity, new DisableShoot());
                }

                if(_inputs.Mouse.RightButton.IsPressed)
                {
                    var shotPosition = colliderRect.Center + shotDirection * 20;
                    _bulletController.SpawnBullet(7 * Constants.FRAME_RATE, 7, 5.0f, shotPosition, shotDirection);

                    var timer = CreateEntity();
                    Set(timer, new Timer(2.0f));
                    Relate(timer, entity, new DisableShoot());
                }
            }
        }
    }
}