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

namespace BalloonWorld
{
	public enum itemType
	{
		key = 1,
		lantern = 2
	}

	public class Item
	{
		private Texture2D key;

		private Texture2D lantern;

		public itemType item;

		BoundingRectangle bounds;

		/// <summary>
		/// Bounding volume of the sprite
		/// </summary>
		public BoundingRectangle Bounds => bounds;

		private Vector2 position;

		public Item(itemType Item, Vector2 Position) 
		{
			position = Position;
			item = Item;
			if (item == itemType.key)
			{
				bounds = new(Position.X, Position.Y, 32, 51);
			}
			else 
			{
				bounds = new(Position.X, Position.Y, 9 * 2, 15 * 2);
			}
		}

		/// <summary>
		/// Loads the content for the sprite
		/// </summary>
		/// <param name="content">The ContentManager to load with</param>
		public void LoadContent(ContentManager content)
		{
			lantern = content.Load<Texture2D>("pq/168291");
			key = content.Load<Texture2D>("pq/168580");
		}


		/// <summary>
		/// Draws the sprite on-screen
		/// </summary>
		/// <param name="gameTime">The GameTime object</param>
		/// <param name="spriteBatch">The SpriteBatch to draw with</param>
		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			if (item == itemType.lantern)
			{
				Rectangle lanternBaseSource = new Rectangle(923, 18, 9, 15);
				spriteBatch.Draw(lantern, position, lanternBaseSource, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
			}
			else
			{
				var bounding = new Rectangle(0, 0, 32, 51);
				spriteBatch.Draw(key, position, bounding, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
			}
		}

	}
}
