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
using BalloonWorld.Rooms;
using System.Windows.Forms;
using System.Threading;

namespace BalloonWorld
{

	public class Door
	{
		private Texture2D door;

		BoundingRectangle bounds;

		public bool isLocked = true;

		public bool canBeUnlockedWithKey = false;

		public bool isOpening = false;

		/// <summary>
		/// Bounding volume of the sprite
		/// </summary>
		public BoundingRectangle Bounds => bounds;

		private Vector2 position;


		private int animationFrame = 1;

		private double animationTimer = 0;

		/*public Door(Vector2 Position, Brazier brazier) 
		{
			position = Position;
			bounds = new(Position.X, Position.Y, 9 * 2, 15 * 2);
		}*/

		public Door(Vector2 Position)
		{
			position = Position;
			bounds = new(Position.X, Position.Y, 9 * 2, 15 * 2);
			canBeUnlockedWithKey = true;
		}

		/// <summary>
		/// Loads the content for the sprite
		/// </summary>
		/// <param name="content">The ContentManager to load with</param>
		public void LoadContent(ContentManager content)
		{
			door = content.Load<Texture2D>("pq/door");
		}


		/// <summary>
		/// Draws the sprite on-screen
		/// </summary>
		/// <param name="gameTime">The GameTime object</param>
		/// <param name="spriteBatch">The SpriteBatch to draw with</param>
		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
			if (isOpening)
			{
				if (animationTimer > 1 && isOpening) 
				{
					animationFrame++;
					if (animationFrame == 7) isOpening = false; isLocked = false;
					animationTimer -= 1;
				}
			}

			spriteBatch.Draw(door, position, new Rectangle(64 * (animationFrame - 1), 0, 64, 64), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
		}

	}
}
