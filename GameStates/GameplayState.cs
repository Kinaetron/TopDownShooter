using Flam.Data;
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

    private Time _time;
    private Motion _motion;
    private Freeze _freeze;
    private Destroy _destroy;
    private Collision _collision;
    private DebugRenderer _debugRenderer;
    private PlayerController _playerController;
    private BulletController _bulletController;
    private BasicEnemySystem _basicEnemySystem;


    public GameplayState(ShooterGame game, GameState transitionState)
    {
        _game = game;
        _transitionState = transitionState;
    }

    public override void Start()
    {
        var levelString = File.ReadAllText("Content/Levels/Stadium.ldtk");
        var worldInformation = LdtkJson.FromJson(levelString);
        var level = worldInformation.Levels[0];

        var collisionTiles = level.LayerInstances[0];

        _world = new MoonTools.ECS.World();
        _time = new Time(_world);
        _freeze = new Freeze(_world);
        _motion = new Motion(_world);
        _destroy = new Destroy(_world);
        _collision = new Collision(_world);
        _basicEnemySystem = new BasicEnemySystem(_world);
        _bulletController = new BulletController(_world);
        _playerController = new PlayerController(_bulletController, _game.Inputs, _world);
        _debugRenderer = new DebugRenderer(
            _game.MainWindow.Width,
            _game.MainWindow.Height,
            _game.RootTitleStorage,
            _game.MainWindow,
            _game.GraphicsDevice,
            _world);

        for (int i = 0; i < collisionTiles.IntGridCsv.Length; i++)
        {
            if (collisionTiles.IntGridCsv[i] == 1)
            {
                var x = (i % (level.PxWid / 16)) * 16 + level.WorldX;
                var y = (i / (level.PxWid / 16)) * 16 + level.WorldY;

                var tile = _world.CreateEntity();
                _world.Set(tile, new Solid());
                _world.Set(tile, Color.Pink);
                _world.Set(tile, new ColliderUnion(new Rectangle(16, 16, 0, 0)));
                _world.Set(tile, new Position(new Vector2(x, y)));
            }
        }

        var testBox = _world.CreateEntity();
        _world.Set(testBox, new Solid());
        _world.Set(testBox, Color.BurlyWood);
        _world.Set(testBox, new ColliderUnion(new Rectangle(100, 100, 250, 100)));
        _world.Set(testBox, new Position(new Vector2(250, 100)));

        var player = _world.CreateEntity();
        _world.Set(player, new Player());
        _world.Set(player, Color.Green);
        _world.Set(player, new Velocity(Vector2.Zero));
        _world.Set(player, new Remainder(Vector2.Zero));
        _world.Set(player, new Accerlation(1.0f * Constants.FRAME_RATE));
        _world.Set(player, new MaxSpeed(2.0f * Constants.FRAME_RATE));
        _world.Set(player, new ColliderUnion(new Rectangle(16, 16, 0, 0)));
        _world.Set(player, new Position(new Vector2(100, 100)));

        var basicEnemy = _world.CreateEntity();
        _world.Set(basicEnemy, new BasicEnemy());
        _world.Set(basicEnemy, Color.Red);
        _world.Set(basicEnemy, new ColliderUnion(new Rectangle(16, 16, 0, 0)));
        _world.Set(basicEnemy, BasicEnemyState.Wait);
        _world.Set(basicEnemy, new DistanceCheck(150));
        _world.Set(basicEnemy, new Velocity(Vector2.Zero));
        _world.Set(basicEnemy, new Accerlation(1 * Constants.FRAME_RATE));
        _world.Set(basicEnemy, new MaxSpeed(1.0f * Constants.FRAME_RATE));
        _world.Set(basicEnemy, new Position(new Vector2(20, 200)));
        _world.Set(basicEnemy, new CanBeFrozen());
    }

    public override void Update(TimeSpan delta)
    {
        _time.Update(delta);
        _playerController.Update(delta);
        _bulletController.Update(delta);
        _basicEnemySystem.Update(delta);
        _motion.Update(delta);
        _collision.Update(delta);
        _freeze.Update(delta);
        _destroy.Update(delta);

        if (_world.SomeMessage<EndGame>())
        {
            _world.FinishUpdate();
            _world.Dispose();
            _game.SetState(_transitionState);
        }

        _world.FinishUpdate();
    }

    public override void Draw(double alpha)
    {
        _debugRenderer.Render(alpha);
    }

    public override void End()
    {
    }
}
