using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

//Reuses the code from the collision exercise, just worked for my purposes
namespace BalloonWorld.Collisions
{
	/// <summary>
	/// A bounding rectangle for collision
	/// </summary>
	public struct BoundingRectangle
	{
		public float X;

		public float Y;

		public float Width;

		public float Height;

		public float Left => X;

		public float Right => X + Width;

		public float Top => Y;

		public float Bottom => Y + Height;

		public BoundingRectangle(float x, float y, float width, float height) 
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public BoundingRectangle(Microsoft.Xna.Framework.Vector2 position, float height, float width)
		{
			X = position.X;
			Y = position.Y;
			Width = width;
			Height = height;
		}

		public bool collidesWith(BoundingRectangle other) 
		{
			return CollisionHelper.Collides(this, other);
		}

	}
}
