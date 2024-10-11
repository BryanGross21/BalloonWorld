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

namespace BalloonWorld.Rooms
{
	public enum GameOptions	{
		BW = 1,
		PQ = 2,
		Exit = 3
	}
	public class GameSelection : GameScreen
	{
		private Texture2D BalloonIcon;
		private Texture2D PQIcon;
		private Texture2D controls;
		private Texture2D confirmation;

		/// <summary>
		/// Gets the current keyboard state
		/// </summary>
		private KeyboardState currentKeyboardState;
		/// <summary>
		/// Gets the previous keyboard state
		/// </summary>
		private KeyboardState pastKeyboardState;

		ContentManager _content;

		GameOptions currentOption = GameOptions.BW;

		private SoundEffect option;
		private SoundEffect selected;
		private Song backgroundMusic;

		SpriteFont font;

		public override void Activate()
		{
			base.Activate();

			if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");

			PQIcon = _content.Load<Texture2D>("pq/pqicon");
			BalloonIcon = _content.Load<Texture2D>("pq/balloon_thumbnail");
			controls = _content.Load<Texture2D>("pq/Controls_Arrows");
			confirmation = _content.Load<Texture2D>("pq/Confirm_Key");
			option = _content.Load<SoundEffect>("pq/Option");
			selected = _content.Load<SoundEffect>("pq/Selection");
			backgroundMusic = _content.Load<Song>("Swallowed_By_The_Void");
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
				currentOption--;
				if ((int)currentOption < 1) 
				{
					currentOption = GameOptions.Exit; 
				}
				option.Play();
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down) && pastKeyboardState.IsKeyUp(Keys.Down))
			{
				currentOption++;
				if ((int)currentOption > 3) 
				{
					currentOption = GameOptions.BW;
				}
				option.Play();
			}
			if (currentKeyboardState.IsKeyDown(Keys.E) && currentOption == GameOptions.BW)
			{
				selected.Play();

				Thread.Sleep(1500);

				foreach (var screen in ScreenManager.GetScreens())
					screen.ExitScreen();

				ScreenManager.AddScreen(new MainMenu(), PlayerIndex.One);
			}
			if (currentKeyboardState.IsKeyDown(Keys.E) && currentOption == GameOptions.PQ)
			{
				selected.Play();

				Thread.Sleep(1500);

				foreach (var screen in ScreenManager.GetScreens())
					screen.ExitScreen();

				ScreenManager.AddScreen(new pqMainMenu(), PlayerIndex.One);
			}
			if (currentKeyboardState.IsKeyDown(Keys.E) && currentOption == GameOptions.Exit)
			{
				selected.Play();

				Thread.Sleep(1500);

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

			graphics.Clear(Color.Black);

			spriteBatch.Begin();
			Vector2 destination = new Vector2((graphics.Viewport.Width - 700) / 4, (graphics.Viewport.Height - 900) / 4);
			spriteBatch.DrawString(font, "Select your game: ", destination, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

			destination = new Vector2((graphics.Viewport.Width - 700) / 4, (graphics.Viewport.Height - 300) / 4);
			spriteBatch.Draw(BalloonIcon, destination, null, Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0);

			Color color = Color.White;

			if (currentOption == GameOptions.BW)
			{
				color = Color.Red;
			}
			else
			{
				color = Color.White;
			}

			spriteBatch.DrawString(font, "Balloon World", destination + new Vector2(600, 100), color, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

			destination = new Vector2((graphics.Viewport.Width - 700) / 4, (graphics.Viewport.Height + 1250) / 4);
			spriteBatch.Draw(PQIcon, destination, null, Color.White, 0f, Vector2.Zero, .35f, SpriteEffects.None, 0);

			if (currentOption == GameOptions.PQ)
			{
				color = Color.Red;
			}
			else
			{
				color = Color.White;
			}

			spriteBatch.DrawString(font, "Princess Quest", destination + new Vector2(600, 100), color, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

			if (currentOption == GameOptions.Exit)
			{
				color = Color.Red;
			}
			else
			{
				color = Color.White;
			}

			destination = new Vector2((graphics.Viewport.Width - 700) / 4, (graphics.Viewport.Height + 2500) / 4);
			spriteBatch.DrawString(font, "Exit Game", destination, color, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

			spriteBatch.DrawString(font, "Select:", destination + new Vector2(600, 0), Color.White);

			spriteBatch.Draw(controls, destination + new Vector2(850, 0), new Rectangle(0, 96, 96, 96), Color.White, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 1);


			spriteBatch.DrawString(font, "Confirm:", destination + new Vector2(1100, 0), Color.White);

			spriteBatch.Draw(confirmation, destination + new Vector2(1400, 0), null, Color.White, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 1);

			spriteBatch.End();
		}
	}
}
