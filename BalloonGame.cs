using BalloonWorld.Rooms;
using BalloonWorld.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Drawing;

namespace BalloonWorld
{
	public class BalloonGame : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private float _gameScale;
		private Vector2 _gameOffset;
		private readonly ScreenManager _screens;

		public BalloonGame()
		{
			_graphics = new GraphicsDeviceManager(this);
			DisplayMode screen = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
			_graphics.IsFullScreen = true;
			_graphics.PreferredBackBufferWidth = screen.Width;
			_graphics.PreferredBackBufferHeight = screen.Height;
			Content.RootDirectory = "Content";
			IsMouseVisible = false;

			var screenFactory = new ScreenFactory();
			Services.AddService(typeof(IScreenFactory), screenFactory);

			_screens = new ScreenManager(this);
			Components.Add(_screens);

			_screens.AddScreen(new MainMenu(), null);
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
		}

		protected override void Update(GameTime gameTime)
		{
			float screenAspectRatio = (float)GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height;
			float gameAspectRatio = (float)509 / 382;

			if (screenAspectRatio < gameAspectRatio)
			{

				_gameScale = (float)GraphicsDevice.Viewport.Height / 509;
				_gameOffset.X = (GraphicsDevice.Viewport.Width - 382 * _gameScale) / 2f;
				_gameOffset.Y = 0;
			}
			else
			{
				// Letterbox vertically
				_gameScale = (float)GraphicsDevice.Viewport.Width / 509;
				_gameOffset.Y = (GraphicsDevice.Viewport.Height - 382 * _gameScale) / 2f;
				_gameOffset.X = 0;
			}

			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

			// TODO: Add your drawing code here

			_spriteBatch.Begin(transformMatrix: Matrix.CreateScale(_gameScale) * Matrix.CreateTranslation(_gameOffset.X, _gameOffset.Y, 0));
				_screens.Draw(gameTime);
			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
