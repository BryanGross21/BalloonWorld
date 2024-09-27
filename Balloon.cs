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
using BalloonWorld.Rooms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using BalloonWorld.Collisions;

namespace BalloonWorld
{
	public class Balloon
	{
		public Vector2 Position;
		public Texture2D balloon;
		public SoundEffect collected;

		public int screenWidth;
		public int screenHeight;
		private Direction direction;
		private double directionTimer;
		private Random ran = new Random();

		private Color colorBalloon;

		/// <summary>
		/// Bounding volume of the sprite
		/// </summary>
		public BoundingRectangle bounds = new(40, 104, 40, 104);


		public Balloon(int height, int width)
		{
			screenWidth = width;
			screenHeight = height;

			Position = new Vector2(screenWidth - 10, ran.Next(0, screenHeight - 100)); // 50 is offset from the edge

			int random = ran.Next(0, 3);
			if (random == 0)
			{
				colorBalloon = Color.White;
			}
			else if (random == 1)
			{
				colorBalloon = Color.Green;
			}
			else 
			{
				colorBalloon = Color.Blue;
			}

			bounds = new BoundingRectangle(Position.X, Position.Y, 40, 104);
		}


		/// <summary>
		/// Loads the content for the sprite
		/// </summary>
		/// <param name="content">The ContentManager to load with</param>
		public void LoadContent(ContentManager content)
		{
			balloon = content.Load<Texture2D>("balloon");
			collected = content.Load<SoundEffect>("BWHit");
		}

		public void Update(GameTime gameTime)
		{
			directionTimer += gameTime.ElapsedGameTime.TotalSeconds;

			//Switch directions every 1 second
			if (directionTimer > 1.0)
			{
				switch (direction)
				{
					case Direction.Up:
						direction = Direction.Down;
						break;
					case Direction.Down:
						direction = Direction.Up;
						break;
				}
				directionTimer -= 1.0;
			}

			switch (direction)
			{
				case Direction.Up:
					Position += new Vector2(0, -1) * 50 * (float)gameTime.ElapsedGameTime.TotalSeconds;
					break;
				case Direction.Down:
					Position += new Vector2(0, 1) * 50 * (float)gameTime.ElapsedGameTime.TotalSeconds;
					break;
			}


			Position += new Vector2(-1, 0) * 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
			bounds.X = Position.X;
			bounds.Y = Position.Y;
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch) 
		{
				spriteBatch.Draw(balloon, Position, colorBalloon);
		}
	}
}

