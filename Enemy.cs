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
using SharpDX.Direct2D1.Effects;
using BalloonWorld.Rooms;

namespace BalloonWorld
{
	public class enemy
	{
		public enum enemyState
		{
			patrol = 1,
			attack = 2,
			attacking = 3
		}

		public enum enemyDirection
		{
			right = 1,
			left = 2,
		}

		private Texture2D _enemy;
		private Texture2D _square;
		public Vector2 position { get; set; }
		public Vector2 squarePosition { get; set; }
		public float speed { get; set; }

		public enemyState state { get; set; } = enemyState.patrol;

		private enemyDirection direction { get; set; } = enemyDirection.right;

		private bool isMoving = false;

		private double patrolTime = 0;

		private double dropTime = 0;

		Color colorToDraw;


		BoundingRectangle bounds;

		/// <summary>
		/// Bounding volume of the sprite
		/// </summary>
		public BoundingRectangle Bounds => bounds;

		private Vector2 playerPosition;


		private int animationFrame = 1;

		private double animationTimer = 0;

		private Random ran = new Random();


		/// <summary>
		/// Constructs a new enemy
		/// </summary>
		public enemy(Vector2 position)
		{
			this.position = position;

			squarePosition = position;

			int num = ran.Next(1, 3);

			if (num == 1)
			{
				colorToDraw = Color.Green;
			}
			else 
			{
				colorToDraw = Color.Purple;
			}

			bounds = new(position.X, position.Y, 43 * 2f, 50 * 2f);
		}

		/// <summary>
		/// Loads the content for the sprite
		/// </summary>
		/// <param name="content">The ContentManager to load with</param>
		public void LoadContent(ContentManager content)
		{
			_enemy = content.Load<Texture2D>("pq/168319");
			_square = content.Load<Texture2D>("square");
		}

		/// <summary>
		/// Updates the sprite
		/// </summary>
		/// <param name="gameTime">The GameTime object</param>
		public void Update(GameTime gameTime, Vector2 playerPosition)
		{

			float t = (float)gameTime.ElapsedGameTime.TotalSeconds;

			this.playerPosition = playerPosition;

			Vector2 moveDir = playerPosition - position;
			moveDir.Normalize();
			Vector2 distanceToPlayer = playerPosition - position;
			float distance = distanceToPlayer.Length();

			if (state == enemyState.patrol)
			{
				patrolTime += gameTime.ElapsedGameTime.TotalSeconds;
				if (patrolTime > 3f)
				{
					switch (direction)
					{
						case enemyDirection.right:
							direction = enemyDirection.left;
							break;
						case enemyDirection.left:
							direction = enemyDirection.right;
							break;
					}
					patrolTime -= 3.0;
				}
				switch (direction)
				{
					case enemyDirection.right:
						position += new Vector2(1, 0) * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
						break;
					case enemyDirection.left:
						position += new Vector2(-1, 0) * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
						break;
				}

				if (distance <= 250)
				{
					state = enemyState.attack;
				}
				else 
				{
					state = enemyState.patrol;
				}

			}
			else if (state == enemyState.attack) 
			{
				position += moveDir * 100 * t;

				if (moveDir.X < 0)
				{
					direction = enemyDirection.left;
				}
				else if (moveDir.X > 0)
				{
					direction = enemyDirection.right;
				}

				if (distance >= 250)
				{
					state = enemyState.patrol;
				}
				else 
				{
					state = enemyState.attack;
				}

			}

			dropTime += gameTime.ElapsedGameTime.TotalSeconds;
			if (dropTime < 3f)
			{
				var speed = 100;
				if (state == enemyState.attack) 
				{
					speed = 200;
				}
				squarePosition += new Vector2(0, 1) * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			else 
			{
				squarePosition = position;
				dropTime -= 1.5;
			}

			bounds.X = position.X;
			bounds.Y = position.Y;

		}


		/// <summary>
		/// Draws the sprite on-screen
		/// </summary>
		/// <param name="gameTime">The GameTime object</param>
		/// <param name="spriteBatch">The SpriteBatch to draw with</param>
		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Draw the square
			spriteBatch.Draw(_square, squarePosition + new Vector2(RandomHelper.Next(-10, 10), RandomHelper.Next(-15, 16)), null, colorToDraw, 0f, Vector2.Zero, .25f, SpriteEffects.None, 0 );
			spriteBatch.Draw(_square, squarePosition + new Vector2(RandomHelper.Next(-10, 10), RandomHelper.Next(-15, 16)), null, colorToDraw, 0f, Vector2.Zero, .25f, SpriteEffects.None, 0);
			spriteBatch.Draw(_square, squarePosition + new Vector2(RandomHelper.Next(-10, 10), RandomHelper.Next(-15, 16)), null, colorToDraw, 0f, Vector2.Zero, .25f, SpriteEffects.None, 0);

			// Handle enemy animation and drawing
			Rectangle enemyBaseSource = new Rectangle();
			SpriteEffects effect = SpriteEffects.None;

			if (state == enemyState.patrol)
			{
				// Update animation frame based on time
				animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
				if (animationTimer > 1)
				{
					animationFrame++;
					if (animationFrame == 9) animationFrame = 1;
					animationTimer -= 1;
				}

				// Set source rectangle and effect based on direction
				if (direction == enemyDirection.left)
				{
					enemyBaseSource = new Rectangle(6 + 64 * (animationFrame - 1), 419, 43, 50);
					effect = SpriteEffects.FlipHorizontally;
				}
				else if (direction == enemyDirection.right)
				{
					enemyBaseSource = new Rectangle(6 + 64 * (animationFrame - 1), 419, 43, 50);
					effect = SpriteEffects.None;
				}
			}
			else if (state == enemyState.attack)
			{
				// Update animation frame based on time
				animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
				if (animationTimer > 1)
				{
					animationFrame++;
					if (animationFrame == 13) animationFrame = 1;
					animationTimer -= 1;
				}

				// Set source rectangle and effect based on direction
				if (direction == enemyDirection.left)
				{
					enemyBaseSource = new Rectangle(6 + 64 * (animationFrame - 1), 546, 43, 65);
					effect = SpriteEffects.FlipHorizontally;
				}
				else if (direction == enemyDirection.right)
				{
					enemyBaseSource = new Rectangle(6 + 64 * (animationFrame - 1), 546, 43, 65);
					effect = SpriteEffects.None;
				}
			}

			// Draw the enemy sprite
			spriteBatch.Draw(_enemy, position, enemyBaseSource, colorToDraw, 0f, Vector2.Zero, 2f, effect, 0);
		}

	}
}