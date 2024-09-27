using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BalloonWorld.StateManagement;

namespace BalloonWorld
{
	public class ScreenFactory : IScreenFactory
	{
		public GameScreen CreateScreen(Type screenType)
		{
			// All of our screens have empty constructors so we can just use Activator
			return Activator.CreateInstance(screenType) as GameScreen;
		}
	}
}
