using Flam.Shapes;
using MoonWorks.Graphics;
using System.Numerics;
using TopDownShooter.Components;
using TopDownShooter.Graphics;
using TopDownShooter.Messages;
using TopDownShooter.Systems;
using TopDownShooter.Utility;

namespace TopDownShooter.GameStates;

public class GameplayState : GameState
{
    private MoonTools.ECS.World _world;

    private ShooterGame _game;
    private GameState _transitionState;

    private DebugRenderer _debugRenderer;
    private PlayerController _playerController;
    private BulletController _bulletController;
    private BasicEnemySystem _basicEnemySystem;
    private CollisionBehaviour _collisionBehaviour;


    public GameplayState(ShooterGame game, GameState transitionState)
    {
        _game = game;
        _transitionState = transitionState;
    }

    public override void Start()
    {
        _world = new MoonTools.ECS.World();
        _basicEnemySystem = new BasicEnemySystem(_world);
        _bulletController = new BulletController(_world);
        _collisionBehaviour = new CollisionBehaviour(_world);
        _playerController = new PlayerController(_bulletController, _game.Inputs, _world);
        _debugRenderer = new DebugRenderer(
            _game.MainWindow.Width,
            _game.MainWindow.Height,
            _game.RootTitleStorage,
            _game.MainWindow,
            _game.GraphicsDevice,
            _world);

        var player = _world.CreateEntity();
        _world.Set(player, new Player());
        _world.Set(player, Color.Green);
        _world.Set(player, new Velocity(new Vector2()));
        _world.Set(player, new Accerlation(1.0f * Time.FRAME_RATE));
        _world.Set(player, new MaxAcceleration(2.0f * Time.FRAME_RATE));
        _world.Set(player, new RectangleBounds(new Rectangle(16, 16, new Vector2(0, 0))));

        var basicEnemy = _world.CreateEntity();
        _world.Set(basicEnemy, new BasicEnemy());
        _world.Set(basicEnemy, Color.Red);
        _world.Set(basicEnemy, new RectangleBounds(new Rectangle(16, 16, new Vector2(100, 100))));
        _world.Set(basicEnemy, BasicEnemyState.Wait);
        _world.Set(basicEnemy, new CircleBounds(new Circle(100, new Vector2(100 + 8, 100 + 8))));
        _world.Set(basicEnemy, new Speed(1 * Time.FRAME_RATE));
        _world.Set(basicEnemy, new GameTimer(TimeSpan.FromSeconds(5)));
        _world.Set(basicEnemy, new FreezeTime(TimeSpan.FromSeconds(5)));
    }

    public override void Update(TimeSpan delta)
    {
        _playerController.Update(delta);
        _basicEnemySystem.Update(delta);
        _bulletController.Update(delta);
        _collisionBehaviour.Update(delta);

        if(_world.SomeMessage<EndGame>())
        {
            _world.FinishUpdate();
            _world.Dispose();
            _game.SetState(_transitionState);
        }
    }

    public override void Draw(double alpha)
    {
        _debugRenderer.Render(alpha);
    }

    public override void End()
    {
    }
}
