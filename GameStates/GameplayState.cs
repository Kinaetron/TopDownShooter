using Flam.Shapes;
using MoonWorks.Graphics;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Graphics;
using TopDownShooter.Systems;
using TopDownShooter.Utility;

namespace TopDownShooter.GameStates;

public class GameplayState : GameState
{
    private MoonTools.ECS.World _world;

    private ShooterGame _game;
    private readonly DebugRenderer _debugRenderer;
    private readonly PlayerController _playerController;
    private readonly BulletController _bulletController;

    public GameplayState(ShooterGame game)
    {
        _world = new MoonTools.ECS.World();
        _game = game;
        _bulletController = new BulletController(_world);
        _playerController = new PlayerController(_bulletController, _game.Inputs, _world);
        _debugRenderer = new DebugRenderer(
            _game.MainWindow.Width,
            _game.MainWindow.Height,
            _game.RootTitleStorage,
            _game.MainWindow,
            _game.GraphicsDevice,
            _world);
    }

    public override void Start()
    {
        var player = _world.CreateEntity();
        _world.Set(player, new Player());
        _world.Set(player, Color.Red);
        _world.Set(player, new Velocity(new Vector2()));
        _world.Set(player, new Accerlation(1.0f * Time.FRAME_RATE));
        _world.Set(player, new MaxAcceleration(2.0f * Time.FRAME_RATE));
        _world.Set(player, new RectangleBounds(new Rectangle(16, 16, new Vector2(0, 0))));
    }

    public override void Update(TimeSpan delta)
    {
        _playerController.Update(delta);
        _bulletController.Update(delta);
    }

    public override void Draw(double alpha)
    {
        _debugRenderer.Render(alpha);
    }

    public override void End()
    {
    }
}
