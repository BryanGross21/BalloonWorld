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
using BalloonWorld.Collisions;


namespace BalloonWorld.Rooms
{
	public class CogPlayableCutscene : GameScreen
	{
		int count = 1;

		bool playOnce = true;

		bool isStatic = true;

		private Texture2D _static;

		private double staticAnimationTimer;

		private short staticAnimationFrame = 0;

		ContentManager _content;

		Texture2D _cheeseLoop;

		bool _isPlaying = false;

		Texture2D DialogueBox;

		BoundingRectangle mouse = new(0,0, 32,32);

		public BoundingRectangle Bounds => mouse;

		BoundingRectangle text;

		SpriteFont font;

		Color colorText = Color.White;

		string[] cheeseText = { "Hello, I am a \nrepresentative with C.O.G.S Inc.\nWe are taking over the real estate\nthat this attrocity has hoarded.", "To you \"Toons\" this must be sad.\nThis pitiful game of\nyours will never be finished.\nHowever, isn't it funny?", "The boy catching balloons\nfor the rest of his life.\nThe celestial taunting him in the \nbackground.", "The Princess trapped in a labyrinth\nNever to be saved \nby the likes of YOU!", "This \"game\" has gone on too long.\nWe cannot have a society \nwhere workers play. We must work in \nthe name of Chairman.", "So I dare you \"Toons\" come find us\nWe have been expecting you,\nso better be ready to fight.", "That battle sim that your resistance\nrangers are making will make zero\nimpact. You are delaying \nthe inevitable!", "We will be waiting." };

		int cheeseTextPos = 0;

		int[] cheeseSound = { 1, 2, 1, 3, 1, 1, 2, 1  };

		SoundEffect talk;

		SoundEffect grunt;

		SoundEffect question;

		Song bgm;

		bool _textAppears = true;

		private MouseState pastMousePosition;
		private MouseState currentMousePosition;



		public override void Activate()
		{
			base.Activate();

			if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");

			_static = _content.Load<Texture2D>("Static");

			_cheeseLoop = _content.Load<Texture2D>("cheese");

			DialogueBox = _content.Load<Texture2D>("textbox");

			talk = _content.Load<SoundEffect>("COG_VO_murmur");

			grunt = _content.Load<SoundEffect>("COG_VO_grunt");

			question = _content.Load<SoundEffect>("COG_VO_question_3");

			bgm = _content.Load<Song>("Bossbot_Factory_Finale");

			font = _content.Load<SpriteFont>("menuFont");


		}



		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

			pastMousePosition = currentMousePosition;
			currentMousePosition = Mouse.GetState();


			text = new BoundingRectangle((ScreenManager.GraphicsDevice.Viewport.Width + 300) / 2, (ScreenManager.GraphicsDevice.Viewport.Height - 600) / 2, 640, 388);

			Vector2 mousePosition = new Vector2(currentMousePosition.X, currentMousePosition.Y);

			if (mouse.collidesWith(text))
			{
				colorText = Color.BlueViolet;
				if (currentMousePosition.LeftButton == ButtonState.Pressed && pastMousePosition.LeftButton == ButtonState.Released) 
				{
					playOnce = true;
					cheeseTextPos++;
					if (cheeseTextPos >= cheeseText.Length) 
					{
						foreach (var screen in ScreenManager.GetScreens())
							screen.ExitScreen();

						ScreenManager.AddScreen(new GameSelection(), PlayerIndex.One);
					}
				}
			}
			else
			{
				colorText = Color.Black;
			}


			mouse.X = mousePosition.X;
			mouse.Y = mousePosition.Y;

		}



		public override void Draw(GameTime gameTime)
		{
			var graphics = ScreenManager.GraphicsDevice;
			var spriteBatch = ScreenManager.SpriteBatch;
			var font = ScreenManager.Font;

			spriteBatch.Begin();


			if (_isPlaying)
			{
				spriteBatch.Draw(_cheeseLoop, Vector2.Zero, Color.White);

				if (_textAppears)
				{
					var backSource = new Rectangle(0, 0, 580, 253);
					var destination = new Vector2((graphics.Viewport.Width + 300) / 2, (graphics.Viewport.Height - 500) / 2);
					spriteBatch.Draw(DialogueBox, destination, backSource, Color.White);
					spriteBatch.DrawString(font, cheeseText[cheeseTextPos], destination + new Vector2(20, 50), colorText, 0f, Vector2.Zero, .35f, SpriteEffects.None, 0);
					if (cheeseSound[cheeseTextPos] == 1 && playOnce == true)
					{
						talk.Play();
						playOnce = false;
					}
					else if (cheeseSound[cheeseTextPos] == 2 && playOnce == true)
					{
						question.Play();
						playOnce = false;
					}
					else if (cheeseSound[cheeseTextPos] == 3 && playOnce == true)
					{
						grunt.Play();
						playOnce = false;
					}
				}

			}


			if (isStatic)
			{
				staticAnimationTimer += gameTime.ElapsedGameTime.TotalSeconds;
				if (staticAnimationTimer > .22)
				{
					staticAnimationFrame++;
					if (staticAnimationFrame > 6)
					{
						staticAnimationFrame = 1;
						isStatic = false;
						_isPlaying = true;
						MediaPlayer.Play(bgm);
						MediaPlayer.IsRepeating = true;
						count++;
					}
					staticAnimationTimer -= .22;
				}
				var backSource = new Rectangle(0, 388 * staticAnimationFrame, 640, 388);
				var destinationRectangle = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
				spriteBatch.Draw(_static, destinationRectangle, backSource, Color.White);
			}
			spriteBatch.End();
		}

	}
}
