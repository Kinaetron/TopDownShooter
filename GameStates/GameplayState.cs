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

    private readonly ShooterGame _game;
    private readonly GameState _transitionState;

    private Time _time;
    private Camera _camera;
    private Motion _motion;
    private Freeze _freeze;
    private Destroy _destroy;
    private Spawner _spawner;
    private Collision _collision;
    private SessionTime _sessionTime;
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

        var entitiesLayer = level.LayerInstances[0];
        var collisionTiles = level.LayerInstances[1];

        _world = new MoonTools.ECS.World();
        _time = new Time(_world);

        _freeze = new Freeze(_world);
        _motion = new Motion(_world);
        _spawner = new Spawner(_world);
        _destroy = new Destroy(_world);
        _collision = new Collision(_world);
        _sessionTime = new SessionTime(_world);
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

        var levelSizeX = 0.0f;
        var levelSizeY = 0.0f;

        for (int i = 0; i < collisionTiles.IntGridCsv.Length; i++)
        {
            if (collisionTiles.IntGridCsv[i] == 1)
            {
                var x = (i % (level.PxWid / 16)) * 16 + level.WorldX;
                var y = (i / (level.PxWid / 16)) * 16 + level.WorldY;

                levelSizeX = level.PxWid + level.WorldX;
                levelSizeY = level.PxHei + level.WorldY;

                var tile = _world.CreateEntity();
                _world.Set(tile, new Solid());
                _world.Set(tile, Color.Pink);
                _world.Set(tile, new ColliderUnion(new Rectangle(16, 16, 0, 0)));
                _world.Set(tile, new Position(new Vector2(x, y)));
            }
        }

        foreach (var entity in entitiesLayer.EntityInstances)
        {
            if(entity.Identifier == "Player")
            {
                var player = _world.CreateEntity();
                _world.Set(player, new Player());
                _world.Set(player, Color.Green);
                _world.Set(player, new Velocity(Vector2.Zero));
                _world.Set(player, new Remainder(Vector2.Zero));
                _world.Set(player, new Accerlation(1.0f * Constants.FRAME_RATE));
                _world.Set(player, new MaxSpeed(2.0f * Constants.FRAME_RATE));
                _world.Set(player, new ColliderUnion(new Rectangle(16, 16, 0, 0)));
                _world.Set(player, new Position(new Vector2((float)entity.WorldX, (float)entity.WorldY)));
                _world.Set(player, new CanDieOnHit());
            }

            if(entity.Identifier == "Enemy")
            {
                var basicEnemy = _world.CreateEntity();
                var spawnTime = (float)entity.FieldInstances[0].Value;

                _world.Set(basicEnemy, new BasicEnemy());
                _world.Set(basicEnemy, new SpawnTime(spawnTime));
                _world.Set(basicEnemy, new Position(new Vector2((float)entity.WorldX, (float)entity.WorldY)));
            }
        }

        _camera = new Camera(
            levelSizeX,
            levelSizeY,
            _game.MainWindow.Width,
            _game.MainWindow.Height,
            _world);

        var camera = _world.CreateEntity();
        _world.Set(camera, new Translate(Vector2.Zero));

        var sessionTime = _world.CreateEntity();
        _world.Set(sessionTime, new SessionTimer(0.0f));
    }

    public override void Update(TimeSpan delta)
    {
        _sessionTime.Update(delta);
        _time.Update(delta);
        _spawner.Update(delta);
        _playerController.Update(delta);
        _bulletController.Update(delta);
        _basicEnemySystem.Update(delta);
        _motion.Update(delta);
        _collision.Update(delta);
        _freeze.Update(delta);
        _destroy.Update(delta);
        _camera.Update(delta);

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
