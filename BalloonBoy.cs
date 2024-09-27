using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using BalloonWorld.Collisions;

namespace BalloonWorld
{
	public class BalloonBoy
	{
		private Texture2D _player;
		private SoundEffect jump;
		Vector2 position;
		Vector2 velocity;

		BoundingRectangle bounds = new(76, 109, 76 * 2, 109 * 2);

		/// <summary>
		/// Bounding volume of the sprite
		/// </summary>
		public BoundingRectangle Bounds => bounds;

		private const float Gravity = 150f;

		private const float Thrust = -200f;

		/// <summary>
		/// The maximum velocity the player can fall at.
		/// </summary>
		private const float MaxFallSpeed = 200f;

		/// <summary>
		/// The maximum velocity the player can rise at.
		/// </summary>
		private const float MaxRiseSpeed = -300f;

		/// <summary>
		/// Gets the current keyboard state
		/// </summary>
		private KeyboardState currentKeyboardState;
		/// <summary>
		/// Gets the previous keyboard state
		/// </summary>
		private KeyboardState pastKeyboardState;

		private int bbAnimationFrame = 0;

		private int heightOfScreen;

		private int bottomOfScreen;

		/// <summary>
		/// Checks to end the game or not
		/// </summary>
		public bool quitGame = false;

		/// <summary>
		/// The player's current score
		/// </summary>
		public int Score = 0;

		/// <summary>
		/// Constructs a new Balloon Boy
		/// </summary>
		public BalloonBoy(int height, int bottom)
		{
			heightOfScreen = height;
			bottomOfScreen = bottom;
			position = new Vector2(50, 50);
			velocity = new Vector2(0, 0);
			bounds = new BoundingRectangle(position.X, position.Y, 76 * 2, 109 * 2);
		}

		/// <summary>
		/// Loads the content for the sprite
		/// </summary>
		/// <param name="content">The ContentManager to load with</param>
		public void LoadContent(ContentManager content)
		{
			_player = content.Load<Texture2D>("bb");
			jump = content.Load<SoundEffect>("jump");
		}

		/// <summary>
		/// Updates the sprite
		/// </summary>
		/// <param name="gameTime">The GameTime object</param>
		public void Update(GameTime gameTime)
		{
			pastKeyboardState = currentKeyboardState;
			currentKeyboardState = Keyboard.GetState();

			float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

			velocity.Y += Gravity * t;


			if (currentKeyboardState.IsKeyDown(Keys.Space) && pastKeyboardState.IsKeyUp(Keys.Space))
			{
				velocity.Y = Thrust;
				bbAnimationFrame++;
				if (bbAnimationFrame > 2) 
				{
					bbAnimationFrame = 0;
				}
				jump.Play();
			}

			if (velocity.Y > MaxFallSpeed) velocity.Y = MaxFallSpeed;
			if (velocity.Y < MaxRiseSpeed) velocity.Y = MaxRiseSpeed;

			if (currentKeyboardState.IsKeyDown(Keys.Escape))
			{
				quitGame = true;
			}


			position += velocity * t;

			if (position.Y < 0) position.Y = 0;
			if (position.Y > (heightOfScreen - _player.Height) - 100) position.Y = (heightOfScreen - _player.Height) - 100;

			bounds.X = position.X;  // Update bounds position X
			bounds.Y = position.Y;  // Update bounds position Y

		}

		/// <summary>
		/// Draws the sprite on-screen
		/// </summary>
		/// <param name="gameTime">The GameTime object</param>
		/// <param name="spriteBatch">The SpriteBatch to draw with</param>
		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			var baseSource = new Rectangle(76 * bbAnimationFrame, 0, 76, 109);
			spriteBatch.Draw(_player, position, baseSource, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
		}


	}
}
