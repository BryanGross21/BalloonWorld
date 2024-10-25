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
using System.Collections.Generic;


namespace BalloonWorld.Rooms
{
	public class pqMainMenu : GameScreen
	{
		private Texture2D Background;

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

		private SoundEffect selected;
		private Song backgroundMusic;

		private float scale = .75f;
		private double scaleTime;
		private bool scaleDown = false;

		SpriteFont font;
		public override void Activate()
		{
			base.Activate();

			if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");

			


			Background = _content.Load<Texture2D>("pq/pq_screen");
			selected = _content.Load<SoundEffect>("pq/Selection");
			backgroundMusic = _content.Load<Song>("pq/Princess_Quest");
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

			scaleTime += gameTime.TotalGameTime.TotalSeconds;
			if (scaleTime > 1) 
			{
				if (scaleDown == false)
				{
					scale += .01f;
					if (Math.Abs(scale - 1) < .001) 
					{
						scaleDown = true;
					}
				}
				else 
				{
					scale -= .01f;
					if ((Math.Abs(scale - .75) < .001))
					{
						scaleDown = false;
					}
				}
				scaleTime -= 1;
			}



		}

		public override void HandleInput(GameTime gameTime, InputState input)
		{

			pastKeyboardState = currentKeyboardState;
			currentKeyboardState = Keyboard.GetState();

			// Apply keyboard movement
			if(currentKeyboardState.IsKeyDown(Keys.Enter))
			{
				SaveGame saveData = new()
				{
					player = new princess(new SerializePosition ( new Vector2(250, (ScreenManager.GraphicsDevice.Viewport.Height + 500) / 2)), ScreenManager.GraphicsDevice.Viewport.Height, ScreenManager.GraphicsDevice.Viewport.Width),
					items = new List<Item> { new Item(itemType.key, new SerializePosition(new Vector2(500, 225))), new Item(itemType.lantern, new SerializePosition(new Vector2(250, 250))) },
					doors = new List<Door> { new Door(new SerializePosition(new Vector2(1050, 200))) }
				};

				selected.Play();

				Thread.Sleep(1500);

				foreach (var screen in ScreenManager.GetScreens())
					screen.ExitScreen();

				ScreenManager.AddScreen(new pqTestMap(saveData), PlayerIndex.One);
			}
			if (currentKeyboardState.IsKeyDown(Keys.L)) 
			{
				SaveLoadGamePQ sl = new();
				SaveGame saveData = sl.LoadGame();

				selected.Play();

				foreach (var screen in ScreenManager.GetScreens())
					screen.ExitScreen();

				ScreenManager.AddScreen(new pqTestMap(saveData), PlayerIndex.One);
			}
			if (currentKeyboardState.IsKeyDown(Keys.Escape))
			{
				selected.Play();

				Thread.Sleep(1500);

				foreach (var screen in ScreenManager.GetScreens())
					screen.ExitScreen();

				ScreenManager.AddScreen(new GameSelection(), PlayerIndex.One);
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
			var destinationRectangle = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
			spriteBatch.Draw(Background, destinationRectangle, null, Color.White);

			Vector2 destination = new Vector2((graphics.Viewport.Width - 500) / 2, (graphics.Viewport.Height + 200) / 4);
			spriteBatch.DrawString(font, "Rebirth", destination, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);

			destination = new Vector2((graphics.Viewport.Width - 1800) / 2, (graphics.Viewport.Height + 500) / 2);
			spriteBatch.DrawString(font, "Press Enter to Start, L to Load Game, Escape to Exit Game", destination, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);


			spriteBatch.End();
		}

	}
}
