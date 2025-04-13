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
    private Explode _explode;
    private Spawner _spawner;
    private Collision _collision;
    private ChaseOffset _chaseOffset;
    private SessionTime _sessionTime;
    private DebugRenderer _debugRenderer;
    private PlayerController _playerController;
    private ProjectileCreate _projectileCreate;
    private ChaseOffSetPosition _offsetPosition;
    private SelectChaseOffset _selectChaseOffset;
    private ProjectileController _projectileController;
    private Chase _chase;

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

        _chase = new Chase(_world);
        _freeze = new Freeze(_world);
        _motion = new Motion(_world);
        _explode = new Explode(_world);
        _spawner = new Spawner(_world);
        _destroy = new Destroy(_world);
        _collision = new Collision(_world);
        _chaseOffset = new ChaseOffset(_world);
        _sessionTime = new SessionTime(_world);
        _offsetPosition = new ChaseOffSetPosition(_world);
        _selectChaseOffset = new SelectChaseOffset(_world);
        _projectileController = new ProjectileController(_world);
        _projectileCreate = new ProjectileCreate(_projectileController, _world);
        _playerController = new PlayerController(_projectileController, _game.Inputs, _world);
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
                _world.Set(player, new Chased());
                _world.Set(player, new OffsetChaseTarget());

                var distance = 40.0f;
                var numberOfChasePoints = 10;
                var angleStep = (2 * Math.PI / numberOfChasePoints);

                for (int i = 0; i < numberOfChasePoints; i++)
                {
                    var chaseOffSet = _world.CreateEntity();
                    _world.Set(chaseOffSet, Color.Green);
                    _world.Set(chaseOffSet, new OffsetChase());

                    var angle = i * angleStep;
                    var offsetX = distance * (float)Math.Cos(angle);
                    var offsetY = distance * (float)Math.Sin(angle);
                    var offsetPosition = new Vector2(offsetX, offsetY);
                    _world.Set(chaseOffSet, new OffsetPosition(offsetPosition));
                    _world.Set(chaseOffSet, new ColliderUnion(new Circle(4, Vector2.Zero)));
                }
            }

            if(entity.Identifier == "Rat")
            {
                var rat = _world.CreateEntity();
                var spawnTime = (float)entity.FieldInstances[0].Value;

                _world.Set(rat, new Rat());
                _world.Set(rat, new SpawnTime(spawnTime));
                _world.Set(rat, new Position(new Vector2((float)entity.WorldX, (float)entity.WorldY)));
            }

            if (entity.Identifier == "Dog")
            {
                var rat = _world.CreateEntity();
                var spawnTime = (float)entity.FieldInstances[0].Value;

                _world.Set(rat, new Dog());
                _world.Set(rat, new SpawnTime(spawnTime));
                _world.Set(rat, new Position(new Vector2((float)entity.WorldX, (float)entity.WorldY)));
            }

            if(entity.Identifier == "Turret")
            {
                var turret = _world.CreateEntity();
                var spawnTime = (float)entity.FieldInstances[0].Value;

                _world.Set(turret, new Turret());
                _world.Set(turret, new SpawnTime(spawnTime));
                _world.Set(turret, new Position(new Vector2((float)entity.WorldX, (float)entity.WorldY)));
            }

            if(entity.Identifier == "Blob")
            {
                var blob = _world.CreateEntity();
                var spawnTime = (float)entity.FieldInstances[0].Value;

                _world.Set(blob, new Blob());
                _world.Set(blob, new SpawnTime(spawnTime));
                _world.Set(blob, new Position(new Vector2((float)entity.WorldX, (float)entity.WorldY)));
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
        _projectileController.Update(delta);
        _motion.Update(delta);
        _projectileCreate.Update(delta);
        _collision.Update(delta);
        _freeze.Update(delta);
        _offsetPosition.Update(delta);
        _selectChaseOffset.Update(delta);
        _chaseOffset.Update(delta);
        _chase.Update(delta);
        _explode.Update(delta);
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
