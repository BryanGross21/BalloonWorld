using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using BalloonWorld.StateManagement;
using Microsoft.Xna.Framework.Content;
using SharpDX.Direct3D9;
using System;
using System.IO;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.Reflection.PortableExecutable;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;

using BalloonWorld.Rooms;
using System.Net;

namespace BalloonWorld
{
	public class pqUIClass
	{

		int playerHP;

		int amountOfKeys;

		SpriteFont font;
		Texture2D keys;

		/// <summary>
		/// Loads the content for the sprite
		/// </summary>
		/// <param name="content">The ContentManager to load with</param>
		public void LoadContent(ContentManager content)
		{
			keys = content.Load<Texture2D>("pq/168580");
			font = content.Load<SpriteFont>("menuFont");
		}

		/// <summary>
		/// Updates the sprite
		/// </summary>
		/// <param name="gameTime">The GameTime object</param>
		public void Update(GameTime gameTime, princess player)
		{
			amountOfKeys = player.amountOfKeys;
			playerHP = player.currentHp;
		}

		/// <summary>
		/// Draws the sprite on-screen
		/// </summary>
		/// <param name="gameTime">The GameTime object</param>
		/// <param name="spriteBatch">The SpriteBatch to draw with</param>
		public void Draw(GameTime gameTime, SpriteBatch spriteBatch, int width)
		{
			var bounding = new Rectangle(0, 0, 32, 51);
			spriteBatch.Draw(keys, new Vector2(0, 25), bounding, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0);
			spriteBatch.DrawString(font, "X" + amountOfKeys, new Vector2(100, 100), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
			spriteBatch.DrawString(font,"Health: " + playerHP, new Vector2(width - 800, 25), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
			
		}
	}
}
