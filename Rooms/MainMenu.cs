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

public enum MenuOptions 
{
	StartGame = 1,
	EndlessMode = 2,
	ExitGame = 3,
}


namespace BalloonWorld.Rooms
{
	public class MainMenu : GameScreen
	{
		private Texture2D menuContent;

		private bool isEndlessUnlocked = false;

		private int highScore = 0;

		StreamReader reader;

		/// <summary>
		/// Gets the current keyboard state
		/// </summary>
		private KeyboardState currentKeyboardState;
		/// <summary>
		/// Gets the previous keyboard state
		/// </summary>
		private KeyboardState pastKeyboardState;

		ContentManager _content;

		MenuOptions currentOption = MenuOptions.StartGame;

		private SoundEffect option;
		private SoundEffect selected;
		private Song backgroundMusic;

		SpriteFont font;

		public override void Activate()
		{
			base.Activate();

			if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");

			string[] info = new string[2];
			string filePath = Environment.CurrentDirectory;
			filePath += "\\gameInfo.txt";
			if (File.Exists(filePath))
			{
				reader = new(filePath);
				info = reader.ReadLine().Split(',');
			}
			else 
			{
				info[0] = "0";
				info[1] = "false";
			}
			highScore = Convert.ToInt32(info[0]);
			if (info[1] == "false")
			{
				isEndlessUnlocked = false;
			}
			else
			{
				isEndlessUnlocked = true;
			}


			menuContent = _content.Load<Texture2D>("misc");
			option = _content.Load<SoundEffect>("hoop");
			selected = _content.Load<SoundEffect>("twinkle09");
			backgroundMusic = _content.Load<Song>("Turtle_Crusher");
			font = _content.Load<SpriteFont>("menuFont");
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play(backgroundMusic);
		}



		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, false);

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
		}

		public override void HandleInput(GameTime gameTime, InputState input)
		{

			pastKeyboardState = currentKeyboardState;
			currentKeyboardState = Keyboard.GetState();

			// Apply keyboard movement
			if (currentKeyboardState.IsKeyDown(Keys.Up) && pastKeyboardState.IsKeyUp(Keys.Up))
			{
				if (isEndlessUnlocked)
				{
					currentOption--;
					if (currentOption == 0)
					{
						currentOption = MenuOptions.ExitGame;
					}
				}
				else
				{
					if (currentOption == MenuOptions.StartGame)
					{
						currentOption = MenuOptions.ExitGame;
					}
					else
					{
						currentOption = MenuOptions.StartGame;
					}
				}
				option.Play();
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down) && pastKeyboardState.IsKeyUp(Keys.Down))
			{
				if (isEndlessUnlocked)
				{
					currentOption++;
					if ((int)currentOption == 4)
					{
						currentOption = (MenuOptions)1;
					}
				}
				else
				{
					if (currentOption == MenuOptions.StartGame)
					{
						currentOption = MenuOptions.ExitGame;
					}
					else
					{
						currentOption = MenuOptions.StartGame;
					}
				}
				option.Play();
			}
			if (currentKeyboardState.IsKeyDown(Keys.E) && currentOption == MenuOptions.StartGame)
			{
				selected.Play();

				Thread.Sleep(1000);

				foreach (var screen in ScreenManager.GetScreens())
					screen.ExitScreen();

				ScreenManager.AddScreen(new GameplayScreen(false, highScore), PlayerIndex.One);
			}
			if (currentKeyboardState.IsKeyDown(Keys.E) && currentOption == MenuOptions.EndlessMode)
			{
				selected.Play();

				Thread.Sleep(1000);

				foreach (var screen in ScreenManager.GetScreens())
					screen.ExitScreen();

				ScreenManager.AddScreen(new GameplayScreen(true, highScore), PlayerIndex.One);
			}
			if (currentKeyboardState.IsKeyDown(Keys.E) && currentOption == MenuOptions.ExitGame)
			{
				selected.Play();

				Thread.Sleep(1000);

				ScreenManager.Game.Exit();
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

			spriteBatch.Begin();
			var backSource = new Rectangle(778, 686, 509, 382);
			var destinationRectangle = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
			spriteBatch.Draw(menuContent, destinationRectangle, backSource, Color.White);

			Vector2 destination = new Vector2((graphics.Viewport.Width - 500) / 2, (graphics.Viewport.Height - 200) / 2);
			spriteBatch.DrawString(font, "High Score: " + highScore, destination, Color.White);

			if (isEndlessUnlocked) {
				destination = new Vector2((graphics.Viewport.Width - 350) / 2, (graphics.Viewport.Height + 100) / 2);
				backSource = new Rectangle(854, 599, 137, 70);
				spriteBatch.Draw(menuContent, destination, backSource, Color.White, 0f, Vector2.Zero, new Vector2(2.5f, 1.5f), SpriteEffects.None, 0);

				destination = new Vector2((graphics.Viewport.Width - 350) / 2, (graphics.Viewport.Height + 400) / 2);
				spriteBatch.Draw(menuContent, destination, backSource, Color.White, 0f, Vector2.Zero, new Vector2(2.5f, 1.5f), SpriteEffects.None, 0);

				destination = new Vector2((graphics.Viewport.Width - 350) / 2, (graphics.Viewport.Height + 700) / 2);
				spriteBatch.Draw(menuContent, destination, backSource, Color.White, 0f, Vector2.Zero, new Vector2(2.5f, 1.5f), SpriteEffects.None, 0);

				Color color = Color.White;

				if (currentOption == MenuOptions.StartGame)
				{
					color = Color.Red;
				}
				else
				{
					color = Color.LightBlue;
				}


				destination = new Vector2((graphics.Viewport.Width - 222.5f) / 2, (graphics.Viewport.Height + 150) / 2);
				spriteBatch.DrawString(font, "Start Game", destination, color, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0);


				if (currentOption == MenuOptions.EndlessMode)
				{
					color = Color.Red;
				}
				else
				{
					color = Color.LightBlue;
				}


				destination = new Vector2((graphics.Viewport.Width - 220) / 2, (graphics.Viewport.Height + 450) / 2);
				spriteBatch.DrawString(font, "Endless", destination, color, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0);

				if (currentOption == MenuOptions.ExitGame)
				{
					color = Color.Red;
				}
				else
				{
					color = Color.LightBlue;
				}


				destination = new Vector2((graphics.Viewport.Width - 220) / 2, (graphics.Viewport.Height + 750) / 2);
				spriteBatch.DrawString(font, "Exit Game", destination, color, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0);

			}
			else 
			{

				destination = new Vector2((graphics.Viewport.Width - 350) / 2, (graphics.Viewport.Height + 100) / 2);
				backSource = new Rectangle(854, 599, 137, 70);
				spriteBatch.Draw(menuContent, destination, backSource, Color.White, 0f, Vector2.Zero, new Vector2(2.5f, 1.5f), SpriteEffects.None, 0);

				destination = new Vector2((graphics.Viewport.Width - 350) / 2, (graphics.Viewport.Height + 400) / 2);
				spriteBatch.Draw(menuContent, destination, backSource, Color.White, 0f, Vector2.Zero, new Vector2(2.5f, 1.5f), SpriteEffects.None, 0);

				Color color = Color.White;

				if (currentOption == MenuOptions.StartGame)
				{
					color = Color.Red;
				}
				else 
				{
					color = Color.LightBlue;
				}


				destination = new Vector2((graphics.Viewport.Width - 222.5f) / 2, (graphics.Viewport.Height + 150) / 2);
				spriteBatch.DrawString(font, "Start Game", destination, color, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0);

				if (currentOption == MenuOptions.ExitGame)
				{
					color = Color.Red;
				}
				else 
				{
					color = Color.LightBlue;
				}


				destination = new Vector2((graphics.Viewport.Width - 220) / 2, (graphics.Viewport.Height + 450) / 2);
				spriteBatch.DrawString(font, "Exit Game", destination, color, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0);

			}
	
			spriteBatch.End();
		}

	}
}
