using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using BalloonWorld.StateManagement;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.Text.Json;
using System.Diagnostics;
using System.IO;

namespace BalloonWorld.Rooms
{
	public enum PQPauseOptions 
	{
		Continue = 1,
		Save = 2,
		Load = 3,
		Quit = 4
	}

	public class PQPauseMenu : GameScreen
	{
		/// <summary>
		/// Gets the current keyboard state
		/// </summary>
		private KeyboardState currentKeyboardState;
		/// <summary>
		/// Gets the previous keyboard state
		/// </summary>
		private KeyboardState pastKeyboardState;

		tilemap controls;

		Texture2D logo;

		ContentManager _content;

		SaveLoadGamePQ sL = new();

		PQPauseOptions currentOption = PQPauseOptions.Continue;

		private SoundEffect option;
		private SoundEffect selected;
		private Song backgroundMusic;

		SpriteFont font;

		private SaveGame saveData;

		public PQPauseMenu(princess player1, List<Item> items1, List<Door> doors1) 
		{
			saveData = new SaveGame()
			{
				player = player1,
				items = items1,
				doors = doors1
			};
		}

		public override void Activate()
		{
			base.Activate();

			if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");

			option = _content.Load<SoundEffect>("pq/Option");
			selected = _content.Load<SoundEffect>("pq/Selection");
			font = _content.Load<SpriteFont>("menuFont");
			logo = _content.Load<Texture2D>("pq/logo");
			controls = new("map.txt");
			controls.LoadContent(_content);
		}



		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, false);

		}

		public override void HandleInput(GameTime gameTime, InputState input)
		{

			pastKeyboardState = currentKeyboardState;
			currentKeyboardState = Keyboard.GetState();

			// Apply keyboard movement
			if (currentKeyboardState.IsKeyDown(Keys.W)  && pastKeyboardState.IsKeyUp(Keys.W))
			{
				currentOption--;

				if (currentOption == 0) 
				{
					currentOption = PQPauseOptions.Quit;
				}

				option.Play();
			}

			if (currentKeyboardState.IsKeyDown(Keys.S) && pastKeyboardState.IsKeyUp(Keys.S)) 
			{
				currentOption++;

				if ((int)currentOption == 5) 
				{
					currentOption = PQPauseOptions.Continue;
				}

				option.Play();
			}

			if (currentKeyboardState.IsKeyDown(Keys.E)) 
			{
				if (currentOption == PQPauseOptions.Continue)
				{
					selected.Play();

					Thread.Sleep(1500);

					ExitScreen();
				}
				else if (currentOption == PQPauseOptions.Save)
				{
					sL.SaveGame(saveData);

					selected.Play();

					Thread.Sleep(1500);

					ExitScreen();
				}
				else if (currentOption == PQPauseOptions.Load) 
				{
					saveData = sL.LoadGame();

					selected.Play();

					foreach (var screen in ScreenManager.GetScreens())
						screen.ExitScreen();

					ScreenManager.AddScreen(new pqTestMap(saveData), PlayerIndex.One);
				}
				else if (currentOption == PQPauseOptions.Quit)
				{
					selected.Play();

					Thread.Sleep(1500);

					foreach (var screen in ScreenManager.GetScreens())
						screen.ExitScreen();

					ScreenManager.AddScreen(new pqMainMenu(), PlayerIndex.One);
				}
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

			Color color = Color.White;

			graphics.Clear(Color.Black);


			spriteBatch.Begin();
			var destinationRectangle = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);

			controls.Draw(gameTime, spriteBatch);

			if (currentOption == PQPauseOptions.Continue)
			{
				color = Color.Red;
			}
			else 
			{
				color = Color.White;
			}

			Vector2 destination = new Vector2((graphics.Viewport.Width - 500) / 2, (graphics.Viewport.Height + 200) / 4);
			spriteBatch.DrawString(font, "Continue", destination, color, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);

			if (currentOption == PQPauseOptions.Save)
			{
				color = Color.Red;
			}
			else
			{
				color = Color.White;
			}

			destination = new Vector2((graphics.Viewport.Width - 500) / 2, (graphics.Viewport.Height + 600) / 4);
			spriteBatch.DrawString(font, "Save", destination, color, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);

			if (currentOption == PQPauseOptions.Load)
			{
				color = Color.Red;
			}
			else
			{
				color = Color.White;
			}

			destination = new Vector2((graphics.Viewport.Width - 500) / 2, (graphics.Viewport.Height + 1000) / 4);
			spriteBatch.DrawString(font, "Load", destination, color, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);

			if (currentOption == PQPauseOptions.Quit)
			{
				color = Color.Red;
			}
			else
			{
				color = Color.White;
			}

			destination = new Vector2((graphics.Viewport.Width - 500) / 2, (graphics.Viewport.Height + 1400) / 4);
			spriteBatch.DrawString(font, "Quit", destination, color, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);

			destination = new Vector2((graphics.Viewport.Width - 1900) / 2, (graphics.Viewport.Height - 500) / 4);
			spriteBatch.DrawString(font, "Movement:", destination, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

			destination = new Vector2((graphics.Viewport.Width - 1900) / 2, (graphics.Viewport.Height + 1400) / 4);
			spriteBatch.DrawString(font, "Interact:", destination, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

			destination = new Vector2((graphics.Viewport.Width - 500) / 2, (graphics.Viewport.Height - 600) / 4);
			spriteBatch.Draw(logo, destination, null, Color.White, 0f, Vector2.Zero, .45f, SpriteEffects.None, 0);

			spriteBatch.End();
		}
	}
}
