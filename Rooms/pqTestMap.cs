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
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct2D1;
using System.Collections;

namespace BalloonWorld.Rooms
{
	public class pqTestMap : GameScreen
	{
		private Texture2D testBackground;
		double Timer;

		ContentManager _content;

		private Song backgroundMusic;

		Item key;

		Item lantern;

		List<Item> items = new();

		SpriteFont font;

		princess player;

		pqUIClass ui;

		Door keyDoor;

		Door brazierDoor;

		List<Door> doors = new();

		enemy enemy;

		enemy enemy2;

		List<enemy> enemies = new();

		SoundEffect keyCollect;

		SoundEffect doorOpening;

		SoundEffect lanternCollect;

		SoundEffect hit;



		/// <summary>
		/// Gets the current keyboard state
		/// </summary>
		private KeyboardState currentKeyboardState;
		/// <summary>
		/// Gets the previous keyboard state
		/// </summary>
		private KeyboardState pastKeyboardState;

		private Matrix scaleTransform;
		private Matrix cameraTransform;
		private Vector2 screenCenter;

		private float mapWidth = 1920f;
		private float mapHeight = 1080f;

		double deathTimer;

		List<Item> itemsToRemove = new List<Item>();

		TimeSpan currentSpot;

		squareParticleSystem s;

		SaveGame saveData;

		public pqTestMap(SaveGame data) 
		{
			saveData = data;

		}

		public override void Activate()
		{
			base.Activate();

			if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");

			testBackground = _content.Load<Texture2D>("pq/PQTestMapBackground");
			backgroundMusic = _content.Load<Song>("pq/Gracefully_into_the_Abyss");
			keyCollect = _content.Load<SoundEffect>("pq/pickupCoin");
			doorOpening = _content.Load<SoundEffect>("pq/explosion");
			hit = _content.Load<SoundEffect>("pq/hitHurt");
			lanternCollect = _content.Load<SoundEffect>("pq/powerUp");
			font = _content.Load<SpriteFont>("menuFont");
			player = saveData.player;
			player.LoadContent(_content);
			enemy = new enemy((new Vector2((ScreenManager.GraphicsDevice.Viewport.Width + 500) / 2, (ScreenManager.GraphicsDevice.Viewport.Height + 300) / 2)));
			enemy.LoadContent(_content);
			enemies.Add(enemy);
			key = saveData.items[0];
			key.LoadContent(_content);
			items.Add(key);
			lantern = saveData.items[1];
			lantern.LoadContent(_content);
			items.Add(lantern);
			keyDoor = saveData.doors[0];
			keyDoor.LoadContent(_content);
			doors.Add(keyDoor);
			ui = new pqUIClass();
			ui.LoadContent(_content);
			MediaPlayer.Play(backgroundMusic);
			MediaPlayer.IsRepeating = true;

			screenCenter = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2);
		}

		public override void HandleInput(GameTime gameTime, InputState input)
		{
			base.HandleInput(gameTime, input);

			currentKeyboardState = Keyboard.GetState();

			if (currentKeyboardState.IsKeyDown(Keys.Escape)) 
			{
				foreach (Item item in itemsToRemove) 
				{
					items.Add(item);
				}
				currentSpot = MediaPlayer.PlayPosition;
				ScreenManager.AddScreen(new PQPauseMenu(player, items, doors), PlayerIndex.One);
			}

		}

		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, false);

			if (IsActive)
			{
				player.Update(gameTime);

				Vector2 playerPosition = new Vector2(player.position.X, player.position.Y);
				scaleTransform = Matrix.CreateScale(2f);

				Vector2 cameraPosition = playerPosition - screenCenter / 2f;


				cameraPosition.X = MathHelper.Clamp(cameraPosition.X, 0, mapWidth - ScreenManager.GraphicsDevice.Viewport.Width / 2f);
				cameraPosition.Y = MathHelper.Clamp(cameraPosition.Y, 0, mapHeight - ScreenManager.GraphicsDevice.Viewport.Height / 2f);

				cameraTransform = Matrix.CreateTranslation(-cameraPosition.X, -cameraPosition.Y, 0) * scaleTransform;

				enemy.Update(gameTime, playerPosition);

				ui.Update(gameTime, player);


				foreach (Item item in items)
				{
					if (player.Bounds.collidesWith(item.Bounds))
					{
						if (item.item == itemType.key)
						{
							player.amountOfKeys++;
							keyCollect.Play();
						}
						else
						{
							player.maxHP++;
							player.currentHp = player.maxHP;
							lanternCollect.Play();
						}
						item.itemCollected = true;
					}
					if (item.itemCollected)
					{
						itemsToRemove.Add(item);
					}
				}

				foreach (Item item in itemsToRemove)
				{
					items.Remove(item);
				}

				if (player.Bounds.collidesWith(keyDoor.Bounds))
				{
					if (player.amountOfKeys >= 1 && player.currentKeyboardState.IsKeyDown(Keys.E) && player.pastKeyboardState.IsKeyUp(Keys.E) && keyDoor.isLocked)
					{
						player.amountOfKeys--;
						keyDoor.isOpening = true;
					}
					if (player.currentKeyboardState.IsKeyDown(Keys.E) && player.pastKeyboardState.IsKeyUp(Keys.E) && keyDoor.isLocked == false)
					{
						foreach (var screen in ScreenManager.GetScreens())
							screen.ExitScreen();

						ScreenManager.AddScreen(new CogPlayableCutscene(), PlayerIndex.One);
					}
				}

				if (keyDoor.isOpening) 
				{
					keyDoor.isLocked = false;
				}

				if (player.Bounds.collidesWith(enemy.Bounds) && player.state != princess.playerState.dead)
				{
					var enemyPos = new Vector2(enemy.position.X, enemy.position.Y);
					enemy.position = enemy.position + new Vector2(200, 0);
					player.currentHp--;
					hit.Play();
					if (player.currentHp == 0)
					{
						enemy.position += new Vector2(10000, 0);
						player.state = princess.playerState.dead;
					}
				}

				if (player.backToMain)
				{
					Thread.Sleep(1500);

					foreach (var screen in ScreenManager.GetScreens())
					{
						screen.ExitScreen();
					}

					ScreenManager.AddScreen(new pqMainMenu(), PlayerIndex.One);
				}

			}

				if (!ScreenManager.Game.IsActive || !IsActive)
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



		/// <summary>
		/// Draws the sprite using the supplied SpriteBatch
		/// </summary>
		/// <param name="gameTime">The game time</param>
		public override void Draw(GameTime gameTime)
		{
			var graphics = ScreenManager.GraphicsDevice;
			var spriteBatch = ScreenManager.SpriteBatch;
			var font = ScreenManager.Font;

			var backSource = new Rectangle();
			var destinationRectangle = new Rectangle(); ;

			spriteBatch.Begin(transformMatrix: cameraTransform);
			destinationRectangle = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
			spriteBatch.Draw(testBackground, destinationRectangle, null, Color.White);
			enemy.Draw(gameTime, spriteBatch);

			foreach (Item item in items) 
			{
				item.Draw(gameTime, spriteBatch);
			}

			keyDoor.Draw(gameTime, spriteBatch);

			spriteBatch.End();

			spriteBatch.Begin(transformMatrix: cameraTransform, blendState: BlendState.Additive);
			player.Draw(gameTime, spriteBatch);
			spriteBatch.End();

			spriteBatch.Begin();
			ui.Draw(gameTime, spriteBatch, graphics.Viewport.Width);
			spriteBatch.End();

		}



	}
}
