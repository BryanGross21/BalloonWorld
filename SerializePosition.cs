using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BalloonWorld
{
	public class SerializePosition
	{
		public float X { get; set; }

		public float Y { get; set; }

		public SerializePosition() { }

		public SerializePosition(Vector2 position)

		{

			X = position.X;

			Y = position.Y;

		}



		public Vector2 ToVector2()

		{

			return new Vector2(X, Y);

		}

	}
}
