using MoonWorks.Graphics.Font;
using MoonWorks.Graphics;
using System.Numerics;
using MoonWorks.Input;

namespace TopDownShooter.GameStates;

public class DeathState : GameState
{
    private GameState _transitionState;

    private Font _sofiaSans;
    private ShooterGame _game;
    private TextBatch _textBatch;
    private GraphicsPipeline _fontPipeline;

    public DeathState(ShooterGame game, GameState transitionState)
    {
        _game = game;
        _transitionState = transitionState;
    }

    public override void Start()
    {
        _sofiaSans = Font.Load(_game.GraphicsDevice, _game.RootTitleStorage, "Content/Fonts/SofiaSans.ttf");
        _textBatch = new TextBatch(_game.GraphicsDevice);
        var fontPipelineCreateInfo = new GraphicsPipelineCreateInfo
        {
            VertexShader = _game.GraphicsDevice.TextVertexShader,
            FragmentShader = _game.GraphicsDevice.TextFragmentShader,
            VertexInputState = _game.GraphicsDevice.TextVertexInputState,
            PrimitiveType = PrimitiveType.TriangleList,
            RasterizerState = RasterizerState.CCW_CullNone,
            MultisampleState = MultisampleState.None,
            DepthStencilState = DepthStencilState.Disable,
            TargetInfo = new GraphicsPipelineTargetInfo
            {
                ColorTargetDescriptions = [
                  new ColorTargetDescription
                    {
                        Format = _game.MainWindow.SwapchainFormat,
                        BlendState = ColorTargetBlendState.Opaque
                    }
              ]
            }
        };

        _fontPipeline = GraphicsPipeline.Create(_game.GraphicsDevice, fontPipelineCreateInfo);
    }

    public void SetTransitionState(GameState transitionState)
    {
        _transitionState = transitionState; 
    }

    public override void Update(TimeSpan delta)
    {
        if(_game.Inputs.Keyboard.IsPressed(KeyCode.Q))
        {
            _game.SetState(_transitionState);
            return;
        }
    }

    public override void Draw(double alpha)
    {
        var cmdbuf = _game.GraphicsDevice.AcquireCommandBuffer();
        var swapchainTexture = cmdbuf.AcquireSwapchainTexture(_game.MainWindow);

        if (swapchainTexture != null)
        {
            Matrix4x4 proj = Matrix4x4.CreateOrthographicOffCenter(
                0,
                640,
                360,
                0,
                0,
                -1
            );

            Matrix4x4 model =
                Matrix4x4.CreateTranslation(320, 180, 0);

            _textBatch.Start();

            _textBatch.Add(
                _sofiaSans,
                "YOU DEAD DUMB DUMB",
            64,
                model,
                Color.White,
                HorizontalAlignment.Center,
                VerticalAlignment.Middle
            );
            _textBatch.UploadBufferData(cmdbuf);

            var renderPass = cmdbuf.BeginRenderPass(
                new ColorTargetInfo(swapchainTexture, Color.Black)
            );

            renderPass.BindGraphicsPipeline(_fontPipeline);
            _textBatch.Render(renderPass, proj);
            cmdbuf.EndRenderPass(renderPass);
        }

        _game.GraphicsDevice.Submit(cmdbuf);
    }

    public override void End()
    {
        _textBatch.Dispose();
        _sofiaSans.Dispose();
    }
}
