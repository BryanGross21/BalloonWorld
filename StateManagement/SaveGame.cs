using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BalloonWorld.StateManagement
{
	[Serializable]
	public class SaveGame
	{
		public princess player { get; set; }

		public List<Item> items { get; set; }

		public List<Door> doors { get; set; }

	}
}
