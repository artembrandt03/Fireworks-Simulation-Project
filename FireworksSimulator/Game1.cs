using DrawingLibrary.Graphics;
using DrawingLibrary.Input;
using Fireworks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShapeLibrary;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;


namespace FireworksSimulator;

public class Game1 : Game
{
    //---Properties-----------------------------------------------------------------------------------------
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _rt2d;
    private IScreen _screen;
    private IShapesRenderer _shapesRenderer;
    private ISpritesRenderer _spritesRenderer;
    private ICustomMouse _mouse;
    private Texture2D _fadeTexture;
    private KeyboardState _prevKeyboard;
    private MouseState _prevMouse;
    private bool _firstFrame = true;

    //manager for all active fireworks
    private FireworkEnvironment _env;

    //virtual screen size (new from assignment)
    private const int VIRTUAL_WIDTH = 800;
    private const int VIRTUAL_HEIGHT = 600;

    private readonly Random _rng = new Random();

    //state for diplsay mode
    private bool _displayRunning = false;
    private double _displayTimer = 0.0; //seconds since display portion started
    private int _displayWave = 0;      //which step of the displaying we are in (hence 'wave')

    //for sounds effects
    private SoundEffect _launcherEffect;
    private SoundEffect _explosionEffect;

    //one launcher instance PER firework
    private readonly Dictionary<IFirework, SoundEffectInstance> _launcherInstances = new();
    //one explosion instance PER firework
    private readonly Dictionary<IFirework, SoundEffectInstance> _explosionInstances = new();

    private bool _soundEnabled = true; //sound on/off state, we start when its on

    //queued mouse launches (X positions in virtual coordinates) for mouse feature
    private readonly List<float> _queuedMouseLaunchXs = new();

    //for text rendering & key logging
    private SpriteFont _uiFont;
    private bool _keyLoggingEnabled = true;   //on by default
    private string _lastKeyLog = "";
    private double _keyLogTimer = 0.0;        //how long message has been visible
    private const double KEYLOG_DURATION = 5.0;  //seconds before fade
    //---------------------------------------------------------------------------------------------------

    //---Constructor-----------------------------------------------------------------------------------
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    //---------------------------------------------------------------------------------------------------

    //---Methods---------------------------------------------------------------------------------------
    protected override void Initialize()
    {
        Window.AllowUserResizing = true;
        base.Initialize();
        _mouse = CustomMouse.Instance;
        _env = new FireworkEnvironment();
    }

    protected override void LoadContent()
    {
        //RenderTarget with PreserveContents (per instructions)
        _rt2d = new RenderTarget2D(
            GraphicsDevice,
            VIRTUAL_WIDTH,
            VIRTUAL_HEIGHT,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            0,
            RenderTargetUsage.PreserveContents
        );

        _screen = new Screen(_rt2d);

        _shapesRenderer = new ShapesRenderer(GraphicsDevice);
        _spritesRenderer = new SpritesRenderer(GraphicsDevice);

        //1x1 white texture for faden (from assignment spec)
        _fadeTexture = new Texture2D(GraphicsDevice, 1, 1);
        _fadeTexture.SetData(new[] { Color.White });

        //loading sounds by the build name in Content.mgcb
        _launcherEffect = Content.Load<SoundEffect>("Sounds/Launcher");
        _explosionEffect = Content.Load<SoundEffect>("Sounds/Firework Explosion sound");

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _uiFont = Content.Load<SpriteFont>("Fonts/UiFont");
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        _mouse.Update();

        KeyboardState kb = Keyboard.GetState();

        MouseState mouse = Mouse.GetState();

        bool leftClicked = mouse.LeftButton == ButtonState.Pressed &&
                           _prevMouse.LeftButton == ButtonState.Released;

        bool rightClicked = mouse.RightButton == ButtonState.Pressed &&
                            _prevMouse.RightButton == ButtonState.Released;

        //map mouse X from window space to virtual space once
        float mouseX = mouse.X;
        float viewportWidth = GraphicsDevice.Viewport.Width;
        float launchXFromMouse = viewportWidth > 0
            ? mouseX / viewportWidth * VIRTUAL_WIDTH
            : VIRTUAL_WIDTH / 2f;

        //RIGHT CLICK: queue a launch position
        if (rightClicked)
        {
            if (_queuedMouseLaunchXs.Count < 10)
            {
                _queuedMouseLaunchXs.Add(launchXFromMouse);

                if (_keyLoggingEnabled)
                {
                    _lastKeyLog = $"Queued {_queuedMouseLaunchXs.Count} fireworks";
                    _keyLogTimer = 0;
                }
            }
            else
            {
                if (_keyLoggingEnabled)
                {
                    _lastKeyLog = "Queue limit reached (10 fireworks)";
                    _keyLogTimer = 0;
                }
            }
        }

        //LEFT CLICK:
        // - if we have queued positions, launch ALL of them
        // - otherwise, launch a single firework at this X
        if (leftClicked)
        {
            if (_queuedMouseLaunchXs.Count > 0)
            {
                int count = _queuedMouseLaunchXs.Count;

                foreach (float queuedX in _queuedMouseLaunchXs)
                {
                    SpawnMouseFirework(queuedX);
                }

                _queuedMouseLaunchXs.Clear();

                if (_keyLoggingEnabled)
                {
                    _lastKeyLog = $"Launched {count} queued fireworks!";
                    _keyLogTimer = 0;
                }
            }
            else
            {
                SpawnMouseFirework(launchXFromMouse);

                if (_keyLoggingEnabled)
                {
                    _lastKeyLog = $"Mouse launch at X={launchXFromMouse:0}";
                    _keyLogTimer = 0;
                }
            }
        }

        //remember mouse state for next frame
        _prevMouse = mouse;

        //Toggle sound on/off with S
        if (kb.IsKeyDown(Keys.S) && _prevKeyboard.IsKeyUp(Keys.S))
        {
            _soundEnabled = !_soundEnabled;

            if (!_soundEnabled)
            {
                //Immediately stop ALL launcher instances
                foreach (var kvp in _launcherInstances)
                {
                    kvp.Value.Stop();
                    kvp.Value.Dispose();
                }
                _launcherInstances.Clear();

                //Immediately stop ALL explosion instances
                foreach (var kvp in _explosionInstances)
                {
                    kvp.Value.Stop();
                    kvp.Value.Dispose();
                }
                _explosionInstances.Clear();
            }
        }

        //Toggle key logging with T
        if (kb.IsKeyDown(Keys.T) && _prevKeyboard.IsKeyUp(Keys.T))
        {
            _keyLoggingEnabled = !_keyLoggingEnabled;
            _lastKeyLog = "";         
            _keyLogTimer = 0;
        }

        //start the display on L
        if (kb.IsKeyDown(Keys.L) && _prevKeyboard.IsKeyUp(Keys.L))
        {
            // reset state for the show
            _displayRunning = true;
            _displayTimer = 0.0;
            _displayWave = 0;

            //to not overlap with current display show,
            //we'll clear any random fireworks currently on screen
            _env.Items.Clear();
        }

        if (kb.IsKeyDown(Keys.R) && _prevKeyboard.IsKeyUp(Keys.R))
        {
            var colour = new Colour(_rng.Next(64, 256), _rng.Next(64, 256), _rng.Next(64, 256));
            var pattern = ExplosionPatternFactory.CreateRectanglePattern(VIRTUAL_WIDTH / 2f, VIRTUAL_HEIGHT / 2f, 220f, 120f, colour);
            var fw = FireworkFactory.Create(VIRTUAL_WIDTH, VIRTUAL_HEIGHT, colour, pattern);
            _env.AddFirework(fw);

        }

        if (kb.IsKeyDown(Keys.C) && _prevKeyboard.IsKeyUp(Keys.C))
        {
            var colour = new Colour(_rng.Next(64, 256), _rng.Next(64, 256), _rng.Next(64, 256));
            var pattern = ExplosionPatternFactory.CreateCirclePattern(VIRTUAL_WIDTH / 2f, VIRTUAL_HEIGHT / 2f, 120f, colour);
            var fw = FireworkFactory.Create(VIRTUAL_WIDTH, VIRTUAL_HEIGHT, colour, pattern);
            _env.AddFirework(fw);

        }

        if (kb.IsKeyDown(Keys.H) && _prevKeyboard.IsKeyUp(Keys.H))
        {
            var colour = new Colour(_rng.Next(64, 256), _rng.Next(64, 256), _rng.Next(64, 256));

            float cx = VIRTUAL_WIDTH / 2f;
            float cy = VIRTUAL_HEIGHT / 2f;
            float radius = 80f;

            var pattern = ExplosionPatternFactory.CreateStarPattern(cx, cy, radius, colour);
            var fw = FireworkFactory.Create(VIRTUAL_WIDTH, VIRTUAL_HEIGHT, colour, pattern);
            _env.AddFirework(fw);

        }

        if (kb.IsKeyDown(Keys.Space) && _prevKeyboard.IsKeyUp(Keys.Space))
        {
            Colour colour = new Colour(_rng.Next(64, 256), _rng.Next(64, 256), _rng.Next(64, 256));
            IExplosionPattern pattern = ExplosionPatternFactory.Create();
            IFirework firework = FireworkFactory.Create(VIRTUAL_WIDTH, VIRTUAL_HEIGHT, colour, pattern);
            _env.AddFirework(firework);

        }

        if (_displayRunning)
        {
            RunFireworksDisplay(gameTime); //will be implemented below
        }

        //Key logging for our actual functional keys
        if (_keyLoggingEnabled)
        {
            if (kb.IsKeyDown(Keys.Space) && _prevKeyboard.IsKeyUp(Keys.Space))
            {
                _lastKeyLog = "SPACE pressed";
                _keyLogTimer = 0;
            }
            if (kb.IsKeyDown(Keys.R) && _prevKeyboard.IsKeyUp(Keys.R))
            {
                _lastKeyLog = "R pressed";
                _keyLogTimer = 0;
            }
            if (kb.IsKeyDown(Keys.C) && _prevKeyboard.IsKeyUp(Keys.C))
            {
                _lastKeyLog = "C pressed";
                _keyLogTimer = 0;
            }
            if (kb.IsKeyDown(Keys.H) && _prevKeyboard.IsKeyUp(Keys.H))
            {
                _lastKeyLog = "H pressed";
                _keyLogTimer = 0;
            }
            if (kb.IsKeyDown(Keys.L) && _prevKeyboard.IsKeyUp(Keys.L))
            {
                _lastKeyLog = "L pressed";
                _keyLogTimer = 0;
            }
            if (kb.IsKeyDown(Keys.S) && _prevKeyboard.IsKeyUp(Keys.S))
            {
                _lastKeyLog = "S pressed";
                _keyLogTimer = 0;
            }
            if (kb.IsKeyDown(Keys.T) && _prevKeyboard.IsKeyUp(Keys.T))
            {
                _lastKeyLog = "T pressed";
                _keyLogTimer = 0;
            }
        }

        _prevKeyboard = kb;
        _env.Update();

        //Update key log fade timer
        if (_lastKeyLog != "")
        {
            _keyLogTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_keyLogTimer >= KEYLOG_DURATION)
                _lastKeyLog = "";
        }

        //implemented below
        UpdateLauncherSounds();
        UpdateExplosionSounds();

        _env.Clear();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _screen.Set();

        //Clearing only on first frame or when everything is gone
        if (_firstFrame || _env.Items.Count == 0)
        {
            GraphicsDevice.Clear(Color.Black);
            _firstFrame = false;
        }

        //Begin drawing fireworks + particles
        _shapesRenderer.Begin();

        List<IFirework> fireworks = _env.Items;
        foreach (IFirework firework in fireworks)
        {
            //Drawing launcher only if still flying
            if (!firework.Exploded && firework.Launcher != null)
            {
                _shapesRenderer.DrawShape(firework.Launcher.Circle, 1.0f);
            }

            //Drawing only active particles
            foreach (IParticle particle in firework.Particles)
            {
                if (!particle.Done)
                {
                    _shapesRenderer.DrawShape(particle.Circle, 1.0f);
                }
            }
        }

        _shapesRenderer.End();

        _spritesRenderer.Begin(true);
        _spritesRenderer.Draw(
            _fadeTexture,
            new Rectangle(0, 0, VIRTUAL_WIDTH, VIRTUAL_HEIGHT),
            Color.Black * 0.1f
        );
        _spritesRenderer.End();

        //--- UI overlay---
        _spriteBatch.Begin();

        string soundStatus = _soundEnabled ? "ON" : "OFF";
        string loggingStatus = _keyLoggingEnabled ? "ON" : "OFF";
        string helpText =
            "Controls:\n" +
            "SPACE - regular firework\n" +
            "R     - rectangle explosion\n" +
            "C     - circle explosion\n" +
            "H     - star explosion\n" +
            "L     - fireworks show\n" +
            "S     - toggle sound (currently: " + soundStatus + ")\n" +
            "T     - toggle key logging (currently: " + loggingStatus + ")\n" +
            "\n" +
            "Mouse:\n" +
            "Left Click  - launch queued fireworks\n" +
            "Right Click - queue firework launch position\n" +
            "ESC         - quit";

        float uiScale = 0.6f;

        //Text position
        Vector2 textPos = new Vector2(20, 20);

        //Draw the text
        _spriteBatch.DrawString(
            _uiFont,
            helpText,
            textPos,
            Color.White,
            0f,
            Vector2.Zero,
            uiScale,
            SpriteEffects.None,
            0f
        );


        //Key log at bottom-left
        if (_keyLoggingEnabled && _lastKeyLog != "")
        {
            float logScale = 0.7f;
            Vector2 logPos = new Vector2(20, VIRTUAL_HEIGHT - 40);

            _spriteBatch.DrawString(
                _uiFont,
                _lastKeyLog,
                logPos,
                Color.White * (float)(1.0 - (_keyLogTimer / KEYLOG_DURATION)),
                0f,
                Vector2.Zero,
                logScale,
                SpriteEffects.None,
                0f
            );
        }

        //Queued mouse fireworks status (top-right)
        if (_queuedMouseLaunchXs.Count > 0)
        {
            string queuedText = $"Getting ready to launch {_queuedMouseLaunchXs.Count} fireworks";
            float queuedScale = 0.6f;

            Vector2 queuedSize = _uiFont.MeasureString(queuedText) * queuedScale;
            Vector2 queuedPos = new Vector2(
                VIRTUAL_WIDTH - queuedSize.X - 20, 
                20                                  
            );

            _spriteBatch.DrawString(
                _uiFont,
                queuedText,
                queuedPos,
                Color.LightGoldenrodYellow,
                0f,
                Vector2.Zero,
                queuedScale,
                SpriteEffects.None,
                0f
            );
        }

        _spriteBatch.End();
        //-------------------------

        _screen.UnSet();
        _screen.Present(_spritesRenderer, true);

        base.Draw(gameTime);
    }

    //--For Display--------------
    private void LaunchFireworkAt(float x, float y, int launcherLifespanFrames, Colour colour, IExplosionPattern pattern)
    {
        //Here we'll use the second constructor via the factory
        //so with explicit x, y, lifespan
        IFirework fw = FireworkFactory.Create(VIRTUAL_WIDTH, VIRTUAL_HEIGHT, x, y, colour, launcherLifespanFrames, pattern);

        _env.AddFirework(fw);

    }

    private void RunFireworksDisplay(GameTime gameTime)
    {
        //Time since last frame in seconds
        _displayTimer += gameTime.ElapsedGameTime.TotalSeconds;

        //Wave 0: my new star fireworks - three star fireworks from the bottom at t = 0
        if (_displayWave == 0 && _displayTimer >= 0.0)
        {
            float yStart = VIRTUAL_HEIGHT; //from bottom of screen

            //left, center, right
            float x1 = VIRTUAL_WIDTH * 0.25f;
            float x2 = VIRTUAL_WIDTH * 0.50f;
            float x3 = VIRTUAL_WIDTH * 0.75f;

            //all use Star pattern, explode after ~60 ish frames
            Colour col1 = new Colour(255, 200, 200);
            Colour col2 = new Colour(200, 255, 200);
            Colour col3 = new Colour(200, 200, 255);

            IExplosionPattern pStar1 = ExplosionPatternFactory.CreateStarPattern(x1, yStart, 80f, col1);
            IExplosionPattern pStar2 = ExplosionPatternFactory.CreateStarPattern(x2, yStart, 80f, col2);
            IExplosionPattern pStar3 = ExplosionPatternFactory.CreateStarPattern(x3, yStart, 80f, col3);

            LaunchFireworkAt(x1, yStart, 60, col1, pStar1);
            LaunchFireworkAt(x2, yStart, 60, col2, pStar2);
            LaunchFireworkAt(x3, yStart, 60, col3, pStar3);

            _displayWave = 1;
        }

        //Wave 1: two circle explosions from prep lab that happen just a bit later
        else if (_displayWave == 1 && _displayTimer >= 2.0)
        {
            float yStart = VIRTUAL_HEIGHT; //start from bottom again

            float xLeft = VIRTUAL_WIDTH * 0.3f;
            float xRight = VIRTUAL_WIDTH * 0.7f;

            Colour colLeft = new Colour(255, 220, 180);
            Colour colRight = new Colour(180, 220, 255);

            IExplosionPattern pCircleLeft = ExplosionPatternFactory.CreateCirclePattern(VIRTUAL_WIDTH / 2f, VIRTUAL_HEIGHT / 2f, 140f, colLeft);
            IExplosionPattern pCircleRight = ExplosionPatternFactory.CreateCirclePattern(VIRTUAL_WIDTH / 2f, VIRTUAL_HEIGHT / 2f, 140f, colRight);

            LaunchFireworkAt(xLeft, yStart, 55, colLeft, pCircleLeft);
            LaunchFireworkAt(xRight, yStart, 70, colRight, pCircleRight);

            _displayWave = 2;
        }

        //Wave 2: big default explosion in the centre
        else if (_displayWave == 2 && _displayTimer >= 4.0)
        {
            float xCenter = VIRTUAL_WIDTH / 2f;
            float yStart = VIRTUAL_HEIGHT;

            Colour col = new Colour(255, 255, 200);
            IExplosionPattern pattern = ExplosionPatternFactory.Create();

            LaunchFireworkAt(xCenter, yStart, 65, col, pattern);

            _displayWave = 3;
        }

        //Wave 3: this is just to stop the display when everything is done
        else if (_displayWave == 3)
        {
            //When all fireworks have exploded and no particles are left, stop!
            if (_env.Items.Count == 0)
            {
                _displayRunning = false;
                _displayTimer = 0.0;
                _displayWave = 0;
            }
        }
    }
    //----------------------------------------------------------------------------------------------

    //--Helpers-------------------------------------------------------------------------------------

    private void UpdateLauncherSounds()
    {
        //clean up instances whose firework was removed
        var deadFireworks = new List<IFirework>();
        foreach (var kvp in _launcherInstances)
        {
            if (!_env.Items.Contains(kvp.Key))
            {
                kvp.Value.Stop();
                kvp.Value.Dispose();
                deadFireworks.Add(kvp.Key);
            }
        }
        foreach (var fw in deadFireworks)
        {
            _launcherInstances.Remove(fw);
        }

        if (!_soundEnabled)
        {
            return;
        }

        //sync instances with current launcher state
        foreach (var fw in _env.Items)
        {
            bool hasLauncher = !fw.Exploded && fw.Launcher != null;

            if (hasLauncher && !_launcherInstances.ContainsKey(fw))
            {
                var inst = _launcherEffect.CreateInstance();
                inst.IsLooped = true;
                inst.Play();
                _launcherInstances[fw] = inst;
            }
            else if (!hasLauncher && _launcherInstances.TryGetValue(fw, out var inst))
            {
                inst.Stop();
                inst.Dispose();
                _launcherInstances.Remove(fw);
            }
        }
    }

    private void UpdateExplosionSounds()
    {
        //First we clean up explosion instances for fireworks that no longer exist
        var deadExplosions = new List<IFirework>();
        foreach (var kvp in _explosionInstances)
        {
            if (!_env.Items.Contains(kvp.Key))
            {
                kvp.Value.Stop();
                kvp.Value.Dispose();
                deadExplosions.Add(kvp.Key);
            }
        }
        foreach (var fw in deadExplosions)
        {
            _explosionInstances.Remove(fw);
        }

        //If sound is disabled don't start any new explosion sounds!
        if (!_soundEnabled)
        {
            return;
        }

        //For each firework, decide if its explosion sound should be playing yet
        foreach (var fw in _env.Items)
        {
            bool explosionActive = fw.Exploded && fw.Particles.Count > 0;

            if (explosionActive && !_explosionInstances.ContainsKey(fw))
            {
                //Start new explosion sound for this firework
                var inst = _explosionEffect.CreateInstance();
                inst.IsLooped = false;
                inst.Play();
                _explosionInstances[fw] = inst;
            }
            else if (!explosionActive && _explosionInstances.TryGetValue(fw, out var inst))
            {
                //Explosion is over visually so we stop the sound early
                inst.Stop();
                inst.Dispose();
                _explosionInstances.Remove(fw);
            }
        }
    }

    private void SpawnMouseFirework(float launchX)
    {
        float launchY = VIRTUAL_HEIGHT;   //always from bottom
        int lifespan = 60;

        var colour = new Colour(
            _rng.Next(64, 256),
            _rng.Next(64, 256),
            _rng.Next(64, 256)
        );

        IExplosionPattern pattern = ExplosionPatternFactory.Create();

        IFirework mouseFirework = FireworkFactory.Create(
            VIRTUAL_WIDTH,
            VIRTUAL_HEIGHT,
            launchX,
            launchY,
            colour,
            lifespan,
            pattern
        );

        _env.AddFirework(mouseFirework);
    }
}
