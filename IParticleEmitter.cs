using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BalloonWorld
{
	public interface IParticleEmitter
	{
		public Vector2 Position { get; set; }

		public Vector2 velocity { get; set; }
	}
}
