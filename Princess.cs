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
using System.Threading;
using BalloonWorld.Rooms;
using BalloonWorld.StateManagement;

namespace BalloonWorld
{
	public class princess
	{
		public enum playerState 
		{
			lantern = 1,
			sword = 2,
			dead = 3
		}

		private enum playerDirection 
		{
			right = 1,
			left = 2,
			up = 3,
			down = 4
		}

		private Texture2D _player;
		public Vector2 position;
		float speed;

		public playerState state = playerState.lantern;

		private playerDirection direction = playerDirection.down;

		private bool isMoving = false;

		private double rotateTime = 0;

		private float rotateAmount;

		private bool rotateLeft = false;

		BoundingRectangle bounds = new(33, 63, 33 * 2, 63 * 2);

		/// <summary>
		/// Bounding volume of the sprite
		/// </summary>
		public BoundingRectangle Bounds => bounds;

		/// <summary>
		/// Gets the current keyboard state
		/// </summary>
		public KeyboardState currentKeyboardState;

		/// <summary>
		/// Gets the previous keyboard state
		/// </summary>
		public KeyboardState pastKeyboardState;

		private int princessAnimationFrame = 1;

		private double princessAnimationTimer = 0;

		private int heightOfScreen;

		private int widthOfScreen;

		public int maxHP = 3;

		public int currentHp = 3;

		public int amountOfKeys = 0;

		public bool backToMain = false;


		/// <summary>
		/// Constructs a new Princess Boy
		/// </summary>
		public princess(Vector2 position, int height, int width)
		{
			this.position = position;
			heightOfScreen = height;
			widthOfScreen = width;
			bounds = new(position.X, position.Y, 33 * 2, 63 * 2);
		}

		/// <summary>
		/// Loads the content for the sprite
		/// </summary>
		/// <param name="content">The ContentManager to load with</param>
		public void LoadContent(ContentManager content)
		{
			_player = content.Load<Texture2D>("pq/168291");
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

			if (state == playerState.lantern) 
			{
				rotateTime += gameTime.TotalGameTime.TotalSeconds;
				if (rotateTime > 1f)
				{
					if (rotateLeft == false)
					{
						rotateAmount += .0125f;
						if (rotateAmount >= .25f)
						{
							rotateLeft = true;
						}
					}
					else
					{
						rotateAmount -= .0125f;
						if (rotateAmount <= -.25f)
						{
							rotateLeft = false;
						}
					}
					rotateTime -= 1;
				}
			}

			if (state != playerState.dead)
			{
				if (currentKeyboardState.IsKeyDown(Keys.LeftShift) || currentKeyboardState.IsKeyDown(Keys.RightShift)) speed = 200f;
				else speed = 100f;

				if (currentKeyboardState.IsKeyDown(Keys.A))
				{
					position -= Vector2.UnitX * speed * t;
					isMoving = true;
					direction = playerDirection.left;
				}
				if (currentKeyboardState.IsKeyDown(Keys.D))
				{
					position += Vector2.UnitX * speed * t;
					isMoving = true;
					direction = playerDirection.right;
				}
				if (currentKeyboardState.IsKeyDown(Keys.W))
				{
					position -= Vector2.UnitY * speed * t;
					isMoving = true;
					direction = playerDirection.up;
				}
				if (currentKeyboardState.IsKeyDown(Keys.S))
				{
					position += Vector2.UnitY * speed * t;
					isMoving = true;
					direction = playerDirection.down;
				}

				if (!currentKeyboardState.IsKeyDown(Keys.W) &&
				!currentKeyboardState.IsKeyDown(Keys.A) &&
				!currentKeyboardState.IsKeyDown(Keys.S) &&
				!currentKeyboardState.IsKeyDown(Keys.D))
				{
					isMoving = false;
				}

				if (position.Y < 0) position.Y = 0;
				if (position.Y > heightOfScreen - 123) position.Y = heightOfScreen - 123;
				if (position.X < 0) position.X = 0;
				if (position.X > (widthOfScreen - 66)) position.X = widthOfScreen - 66;

				bounds.X = position.X;
				bounds.Y = position.Y;
			}
		}

		/// <summary>
		/// Draws the sprite on-screen
		/// </summary>
		/// <param name="gameTime">The GameTime object</param>
		/// <param name="spriteBatch">The SpriteBatch to draw with</param>
		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{

			Rectangle playerBaseSource = new Rectangle();
			Rectangle lanternBaseSource = new Rectangle(923, 18, 9, 15);
			Vector2 lPosition = Vector2.Zero;
			SpriteEffects effect = SpriteEffects.None;
			if (isMoving == false)
			{
				if (state == playerState.lantern)
				{
					if (direction == playerDirection.left)
					{
						playerBaseSource = new Rectangle(280, 0, 33, 63);
						lPosition = position - new Vector2(-6f, -55);
						effect = SpriteEffects.None;
					}
					if (direction == playerDirection.right)
					{
						playerBaseSource = new Rectangle(280, 0, 33, 63);
						lPosition = position - new Vector2(-60, -55);
						effect = SpriteEffects.FlipHorizontally;
					}
					if (direction == playerDirection.up)
					{
						playerBaseSource = new Rectangle(280, 148, 33, 63);
						effect = SpriteEffects.None;
					}
					if (direction == playerDirection.down)
					{
						playerBaseSource = new Rectangle(280, 222, 33, 63);
						lPosition = position - new Vector2(-20f, -55);
						effect = SpriteEffects.None;
					}
				}
			}
			else
			{
				if (state == playerState.lantern)
				{
					princessAnimationTimer += gameTime.ElapsedGameTime.TotalSeconds;
					double timeToAnimate = speed == 200f ? timeToAnimate = .1 : .2;
					if (princessAnimationTimer > timeToAnimate)
					{
						princessAnimationFrame++;
						if (princessAnimationFrame == 8) princessAnimationFrame = 1;
						princessAnimationTimer -= timeToAnimate;
					}
					if (direction == playerDirection.left)
					{
						playerBaseSource = new Rectangle(536 + 64 * (princessAnimationFrame - 1), 346, 33, 63);
						lPosition = position - new Vector2(-6f, -55);
						effect = SpriteEffects.None;
					}
					if (direction == playerDirection.right)
					{
						playerBaseSource = new Rectangle(536 + 64 * (princessAnimationFrame - 1), 346, 33, 63);
						lPosition = position - new Vector2(-60, -55);
						effect = SpriteEffects.FlipHorizontally;
					}
					if (direction == playerDirection.up)
					{
						playerBaseSource = new Rectangle(536 + 64 * (princessAnimationFrame - 1), 494, 33, 63);
						effect = SpriteEffects.None;
					}
					if (direction == playerDirection.down)
					{
						playerBaseSource = new Rectangle(536 + 64 * (princessAnimationFrame - 1), 569, 33, 63);
						lPosition = position - new Vector2(-20f, -55);
						effect = SpriteEffects.None;
					}
				}
			}
			if (state == playerState.lantern && direction != playerDirection.up)
			{
				if (direction != playerDirection.down)
				{
					spriteBatch.Draw(_player, lPosition, lanternBaseSource, Color.White, rotateAmount, new Vector2(9 / 2f, 15 / 2f), 1.75f, effect, 0);
					spriteBatch.Draw(_player, position, playerBaseSource, Color.White, 0f, Vector2.Zero, 2f, effect, 0);
				}
				else
				{
					spriteBatch.Draw(_player, position, playerBaseSource, Color.White, 0f, Vector2.Zero, 2f, effect, 0);
					spriteBatch.Draw(_player, lPosition, lanternBaseSource, Color.White, rotateAmount, new Vector2(9 / 2f, 15 / 2f), 1.75f, effect, 0);
				}
			}
			else 
			{
				spriteBatch.Draw(_player, position, playerBaseSource, Color.White, 0f, Vector2.Zero, 2f, effect, 0);
			}
			if (state == playerState.dead) 
			{
				princessAnimationTimer += gameTime.ElapsedGameTime.TotalSeconds;
				if (princessAnimationTimer > .25 && princessAnimationFrame != 14)
				{
					princessAnimationFrame++;
					if (princessAnimationFrame == 14) 
					{
						backToMain = true;
					}
					princessAnimationTimer -= .25;
				}
				playerBaseSource = new Rectangle(11 + (96 * (princessAnimationFrame - 1)), 1079, 73, 73);
				spriteBatch.Draw(_player, position, playerBaseSource, Color.White, 0f, Vector2.Zero, 2f, effect, 0);
			}
		}
	}
}
