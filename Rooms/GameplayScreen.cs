using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using BalloonWorld.StateManagement;
using Microsoft.Xna.Framework.Content;
using SharpDX.Direct3D9;
using System;
using System.IO;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.Reflection.PortableExecutable;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;

namespace BalloonWorld.Rooms
{
	public enum Direction
	{
		Down = 0,
		Up = 1
	}

	public enum SunState 
	{
		Sun = 1, 
		Moon = 2,
		Eclipse = 3
	}
	public class GameplayScreen : GameScreen
	{
		private Texture2D gameContent;
		private Texture2D sunContent;
		private Texture2D nightFilter;
		private Texture2D eclipseBackground;
		private Texture2D _static;
		private bool isEndless;
		double Timer;

		ContentManager _content;

		MenuOptions currentOption = MenuOptions.StartGame;

		private Song backgroundMusic;
		private Song glitchedMusic;
		private Song abyss;

		float backgroundSpeed = 100f; // Speed at which the background moves

		/// <summary>
		/// The sun's animation timer
		/// </summary>
		private double sunAnimationTimer;

		/// <summary>
		/// The sun's animation timer
		/// </summary>
		private double staticAnimationTimer;

		private int goOnce = 0; 

		/// <summary>
		/// The sun's animation timer
		/// </summary>
		private double sunBopTimer = 0;

		private Vector2 sunPosition = new();

		/// <summary>
		/// sun's current animation frame
		/// </summary>
		private short sunAnimationFrame = 0;

		/// <summary>
		/// sun's current animation frame
		/// </summary>
		private short staticAnimationFrame = 0;

		/// <summary>
		/// Checks which sun sprite to use
		/// </summary>
		private bool hasTransitioned = false;

		private bool playStaticAnimation = false;
		private int lastScore = 0;

		/// <summary>
		/// Checks to see if the day is transitioning to night
		/// </summary>
		private bool isTransitioning = false;

		SunState currentState = SunState.Sun;

		Direction currentDirection = Direction.Up;

		private BalloonBoy bb;

		StreamWriter sw;

		List<Balloon> gameObjects = new List<Balloon>();

		/// <summary>
		/// Controls the spawn for balloons
		/// </summary>
		double spawnTime;

		int previousHighScore = 0;

		SpriteFont font;

		public GameplayScreen(bool isEndless, int previousHigh) 
		{
			this.isEndless = isEndless;
			previousHighScore = previousHigh;
		}

		public override void Activate()
		{
			base.Activate();

			if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");

			sunContent = _content.Load<Texture2D>("misc");
			gameContent = _content.Load<Texture2D>("circus");
			nightFilter = _content.Load<Texture2D>("nightfilter");
			eclipseBackground = _content.Load<Texture2D>("circus_eclipse");
			_static = _content.Load<Texture2D>("Static");
			backgroundMusic = _content.Load<Song>("Purple_Smasher");
			glitchedMusic = _content.Load<Song>("PurpleSmasherGlitched");
			abyss = _content.Load<Song>("Swallowed_By_The_Void");
			font = _content.Load<SpriteFont>("menuFont");
			bb = new BalloonBoy(ScreenManager.GraphicsDevice.Viewport.Height, ScreenManager.GraphicsDevice.Viewport.Y);
			bb.LoadContent(_content);
			MediaPlayer.Play(backgroundMusic);
			MediaPlayer.IsRepeating = true; 

		}

		public override void HandleInput(GameTime gameTime, InputState input)
		{
			base.HandleInput(gameTime, input);

			bb.Update(gameTime);
		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, false);

				spawnTime += gameTime.TotalGameTime.TotalSeconds;
				if (spawnTime > 5000)
				{
					Balloon balloon = new Balloon(ScreenManager.GraphicsDevice.Viewport.Height, ScreenManager.GraphicsDevice.Viewport.Width);
					balloon.LoadContent(_content);
					gameObjects.Add(balloon);
					spawnTime -= 5000;
				}

				if (bb.quitGame) 
				{
				string filePath = Environment.CurrentDirectory;
				filePath += "\\gameInfo.txt";
				if (bb.Score >= previousHighScore)
				{
					using (StreamWriter sw = new StreamWriter(filePath, false)) // 'false' to overwrite the file
					{
						if (bb.Score != 55 && isEndless == false)
						{
							sw.WriteLine($"{bb.Score},false");
						}
						else 
						{
							sw.WriteLine($"{bb.Score},true");
						}
					}
				}
				foreach (var screen in ScreenManager.GetScreens())
					screen.ExitScreen();

				ScreenManager.AddScreen(new MainMenu(), PlayerIndex.One);
				}

				List<Balloon> balloonsToRemove = new List<Balloon>();

				foreach (Balloon balloons in gameObjects)
				{
					balloons.Update(gameTime);
				}

				foreach (Balloon balloon in gameObjects)
				{
					if (bb.Bounds.collidesWith(balloon.bounds)) // Ensure the collidesWith method is correctly implemented
					{
						bb.Score += 1;
						balloon.collected.Play();
						balloonsToRemove.Add(balloon); // Collect balloons to remove later
					}
				}


				// Remove the balloons after the iteration
				foreach (Balloon balloon in balloonsToRemove)
				{
					gameObjects.Remove(balloon);
				}

				sunBopTimer += gameTime.ElapsedGameTime.TotalSeconds;
				if (sunBopTimer > .5)
				{
					switch (currentDirection)
					{
						case Direction.Up:
							currentDirection = Direction.Down;
							break;
						case Direction.Down:
							currentDirection = Direction.Up;
							break;
					}
					sunBopTimer -= .5;
				}

				if (bb.Score == 45 && bb.Score != lastScore && isEndless == false)
				{
					lastScore = bb.Score;
					isTransitioning = true;
					goOnce++;

					TimeSpan currentSongPos = MediaPlayer.PlayPosition;
					MediaPlayer.Play(glitchedMusic);
					MediaPlayer.IsRepeating = true;
				}
				else if (bb.Score % 10 == 0 && bb.Score != lastScore && currentState != SunState.Eclipse)
				{
					isTransitioning = true;
					lastScore = bb.Score;
				}

				if (bb.Score == 55 && isEndless == false)
				{
					// Remove the balloons after the iteration
					foreach (Balloon balloon in gameObjects)
					{
					balloonsToRemove.Add(balloon); // Collect balloons to remove later
					}

					foreach (Balloon balloon in balloonsToRemove)
					{
						gameObjects.Remove(balloon);
					}


					Timer += gameTime.TotalGameTime.TotalSeconds;
					if (Timer > 20000)
					{
						string filePath = Environment.CurrentDirectory;
						filePath += "\\gameInfo.txt";
						if (bb.Score >= previousHighScore)
						{
							using (StreamWriter sw = new StreamWriter(filePath, false)) // 'false' to overwrite the file
							{
								sw.WriteLine($"{bb.Score},true");
							}
						}
						foreach (var screen in ScreenManager.GetScreens())
							screen.ExitScreen();

						ScreenManager.AddScreen(new MainMenu(), PlayerIndex.One);
					}

			}


			switch (currentDirection)
			{
				case Direction.Up:
					sunPosition += new Vector2(0, -.25f) * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
					break;
				case Direction.Down:
					sunPosition += new Vector2(0, .25f) * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
					break;
			}

			if (!ScreenManager.Game.IsActive)
			{
				// Pause the music or stop sound effects when the game loses focus
				if (MediaPlayer.State == MediaState.Playing)
				{
					MediaPlayer.Pause();
				}
				return;
			}
			else
			{
				// Resume music if the game becomes active again
				if (MediaPlayer.State == MediaState.Paused)
				{
					MediaPlayer.Resume();
				}
			}

			foreach(Balloon balloons in gameObjects)
			{ 
				balloons.Update(gameTime);
			}

		}



		/// <summary>
		/// Draws the sprite using the supplied SpriteBatch
		/// </summary>
		/// <param name="gameTime">The game time</param>
		public override void Draw(GameTime gameTime)
		{
			var graphics = ScreenManager.GraphicsDevice;
			var spriteBatch = ScreenManager.SpriteBatch;
			var font = ScreenManager.Font;

			if (currentState == SunState.Sun)
			{
				graphics.Clear(Color.DeepSkyBlue);
			}
			else if (currentState == SunState.Moon)
			{
				graphics.Clear(Color.Black);
			}
			else
			{
				graphics.Clear(Color.OrangeRed);
			}

			var backSource = new Rectangle();
			var destinationRectangle = new Rectangle(); ;


			//Sun/Moon/Eclipse
			spriteBatch.Begin();
			if (currentState == SunState.Sun)
			{
				int y = 22;
				sunAnimationTimer += gameTime.ElapsedGameTime.TotalSeconds;
				if (sunAnimationTimer > .22)
				{
					sunAnimationFrame++;
					if (sunAnimationFrame > 7) sunAnimationFrame = 0;
					sunAnimationTimer -= .22;
				}
				if (hasTransitioned)
				{
					y = 179;
				}

				backSource = new(162 * sunAnimationFrame, y, 162, 158);

				spriteBatch.Draw(sunContent, new Vector2(graphics.Viewport.X - 50, 0) + sunPosition, backSource, Color.White, 0f, Vector2.Zero, 5f, SpriteEffects.None, 0);
			}
			else if (currentState == SunState.Moon)
			{
				backSource = new(30, 499, 162, 100);
				spriteBatch.Draw(sunContent, new Vector2(graphics.Viewport.X - 50, 0) + sunPosition, backSource, Color.White, 0f, Vector2.Zero, 5f, SpriteEffects.None, 0);
			}
			else 
			{
				sunAnimationTimer += gameTime.ElapsedGameTime.TotalSeconds;
				if (sunAnimationTimer > .22)
				{
					sunAnimationFrame++;
					if (sunAnimationFrame > 7) sunAnimationFrame = 0;
					sunAnimationTimer -= .22;
				}

				backSource = new(162 * sunAnimationFrame, 337, 162, 158);

				spriteBatch.Draw(sunContent, new Vector2(graphics.Viewport.X - 50, 0) + sunPosition, backSource, Color.White, 0f, Vector2.Zero, 5f, SpriteEffects.None, 0);
			}
			spriteBatch.End();

			//Clouds
			spriteBatch.Begin();
			if (currentState == SunState.Sun)
			{
				backSource = new Rectangle(1972, 2545, 1005, 403);
				spriteBatch.Draw(gameContent, Vector2.Zero, backSource, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
			}
			else if (currentState == SunState.Eclipse) 
			{
				backSource = new Rectangle(1972, 3109, 1153, 419);
				spriteBatch.Draw(gameContent, Vector2.Zero, backSource, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
			}
			spriteBatch.End();
			


			//Midground
			spriteBatch.Begin();
			if (currentState != SunState.Eclipse)
			{
				backSource = new Rectangle(534, 0, 511, 336);
				destinationRectangle = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
				spriteBatch.Draw(gameContent, destinationRectangle, backSource, Color.White);
			}
			else 
			{
				backSource = new Rectangle(534, 0, 511, 336);
				destinationRectangle = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
				spriteBatch.Draw(eclipseBackground, destinationRectangle, backSource, Color.White);
			}
			spriteBatch.End();

			spriteBatch.Begin();
				foreach(Balloon balloons in gameObjects) 
				{
					balloons.Draw(gameTime, spriteBatch);
				}
			spriteBatch.End();

			spriteBatch.Begin();
				bb.Draw(gameTime, spriteBatch);
			spriteBatch.End();


			spriteBatch.Begin();
			if (currentState == SunState.Moon)
			{
				spriteBatch.Draw(nightFilter, destinationRectangle, Color.White);
			}
			spriteBatch.End();

			spriteBatch.Begin();
			if (currentState != SunState.Eclipse)
			{
				spriteBatch.DrawString(font, "Balloons Collected: " + bb.Score.ToString(), new Vector2(graphics.Viewport.Width - 900, graphics.Viewport.Y), Color.White);
			}
			else if (currentState == SunState.Eclipse && bb.Score != 55)
			{
				spriteBatch.DrawString(font, "Victims Collected: " + bb.Score.ToString(), new Vector2(graphics.Viewport.Width - 900, graphics.Viewport.Y), Color.White);
			}
			else if (bb.Score == 55 && currentState == SunState.Eclipse) 
			{
				spriteBatch.DrawString(font, "HELP THEM!!!", new Vector2(graphics.Viewport.Width - 900, graphics.Viewport.Y), Color.Indigo);
			}
			spriteBatch.End();

			spriteBatch.Begin();
				if (isTransitioning)
				{
				staticAnimationTimer += gameTime.ElapsedGameTime.TotalSeconds;
				if (staticAnimationTimer > .22)
				{
					staticAnimationFrame++;
					if (staticAnimationFrame > 6) { staticAnimationFrame = 0; 
						isTransitioning = false;
						hasTransitioned = true;
						if (currentState == SunState.Sun && goOnce == 0)
						{
							currentState = SunState.Moon;
							goOnce = 0;
						}
						else if (currentState == SunState.Moon && goOnce == 0)
						{
							currentState = SunState.Sun;
							goOnce = 0;
						}
						else if (goOnce == 1 && isEndless == false) 
						{
							currentState = SunState.Eclipse;
						}
					}
					staticAnimationTimer -= .22;
				}
				backSource = new Rectangle(0, 388 * staticAnimationFrame, 640, 388);
				destinationRectangle = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
				spriteBatch.Draw(_static, destinationRectangle, backSource, Color.White);
				}
				spriteBatch.End();
				
				

		}

	}
}
