﻿using Flam.Shapes;
using System.Numerics;

namespace TopDownShooter.Components;

public readonly record struct Player();
public readonly record struct Bullet();
public readonly record struct Speed(float Value);
public readonly record struct Velocity(Vector2 Value);
public readonly record struct Accerlation(float Value);
public readonly record struct Direction(Vector2 Value);
public readonly record struct CircleBounds(Circle Value);
public readonly record struct MaxAcceleration(float Value);
public readonly record struct RectangleBounds(Rectangle Value);