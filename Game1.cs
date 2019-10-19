using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Arkanoid
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameContent gameContent;

        private Paddle paddle;
        private Wall wall;
        private GameBorder gameBorder;
        private Ball ball;

        private int screenWidth = 0;
        private int screenHeight = 0;
        private MouseState oldMouseState;
        private KeyboardState oldKeyboardState;
        private bool readyToServeBall = true;
        private int ballsRemaining = 3;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            gameContent = new GameContent(Content);

            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            if (screenWidth >= 502)
                screenWidth = 502;
            if (screenHeight >= 700)
                screenHeight = 700;

            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();

            int paddleX = (screenWidth - gameContent.imgPaddle.Width) / 2;
            int paddleY = (screenHeight - 100);
            paddle = new Paddle(paddleX, paddleY, screenWidth, spriteBatch, gameContent);
            wall = new Wall(1, 50, spriteBatch, gameContent);
            gameBorder = new GameBorder(screenWidth, screenHeight, spriteBatch, gameContent);
            ball = new Ball(screenWidth, screenHeight, spriteBatch, gameContent);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (IsActive == false)
            {
                return;  //our window is not active don't update
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState newKeyboardState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();

            //process mouse move                                
            if (oldMouseState.X != newMouseState.X)
            {
                if (newMouseState.X >= 0 || newMouseState.X < screenWidth)
                {
                    paddle.MoveTo(newMouseState.X);
                }
            }

            //process left-click
            if (newMouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed && oldMouseState.X == newMouseState.X && oldMouseState.Y == newMouseState.Y && readyToServeBall)
            {
                ServeBall();
            }

            //process keyboard events                           
            if (newKeyboardState.IsKeyDown(Keys.Left))
            {
                paddle.MoveLeft();
            }
            if (newKeyboardState.IsKeyDown(Keys.Right))
            {
                paddle.MoveRight();
            }
            if (oldKeyboardState.IsKeyUp(Keys.Space) && newKeyboardState.IsKeyDown(Keys.Space) && readyToServeBall)
            {
                ServeBall();
            }

            oldMouseState = newMouseState; // this saves the old state      
            oldKeyboardState = newKeyboardState;

            base.Update(gameTime);
        }
    
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            paddle.Draw();
            wall.Draw();
            gameBorder.Draw();
            if (ball.Visible)
            {
                bool inPlay = ball.Move(wall, paddle);
                if (inPlay)
                {
                    ball.Draw();
                }
                else
                {
                    ballsRemaining--;
                    readyToServeBall = true;
                }
            }
            spriteBatch.End(); 
            base.Draw(gameTime);
        }

        private void ServeBall()
        {
            if (ballsRemaining < 1)
            {
                ballsRemaining = 3;
                ball.Score = 0;
                wall = new Wall(1, 50, spriteBatch, gameContent);
            }
            readyToServeBall = false;
            float ballX = paddle.X + (paddle.Width) / 2;
            float ballY = paddle.Y - ball.Height;
            ball.Launch(ballX, ballY, -3, -3);
        }
    }
}
