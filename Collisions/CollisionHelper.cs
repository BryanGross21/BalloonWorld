using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BalloonWorld.Collisions
{
	public static class CollisionHelper
	{

		/// <summary>
		/// Detects collision between two bounding rectangles
		/// </summary>
		/// <param name="a">the first rectangle</param>
		/// <param name="b">the second rectangle</param>
		/// <returns>True if there is a collision, false if there isn't one</returns>
		public static bool Collides(BoundingRectangle a, BoundingRectangle b) 
		{
			return !(a.Right < b.Left || a.Left > b.Right || a.Top > b.Bottom || a.Bottom < b.Top);
		}
	}
}
