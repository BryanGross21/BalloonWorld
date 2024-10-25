using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BalloonWorld
{
	public class tilemap
	{
		/// <summary>
		/// Dimensions of the tiles and the map
		/// </summary>
		int _tileWidth, _tileHeight, _mapWidth, _mapHeight;

		/// <summary>
		/// The tileset itself
		/// </summary>
		Texture2D _tileTexture;

		/// <summary>
		/// The tile info in the titleset
		/// </summary>
		Rectangle[] _tiles;

		/// <summary>
		/// The tile map data
		/// </summary>
		int[] _map;

		/// <summary>
		/// The file of the map file
		/// </summary>
		string file;

		public tilemap(string file) 
		{
			this.file = file;
		}

		public void LoadContent(ContentManager content) 
		{
			string data = File.ReadAllText(Path.Join(content.RootDirectory, file));
			var lines = data.Split('\n');

			//First line
			var tilesetFrames = lines[0].Trim();
			_tileTexture = content.Load<Texture2D>(tilesetFrames);

			//Second Line
			var secondLine = lines[1].Split(',');
			_tileWidth = int.Parse(secondLine[0]);
			_tileHeight = int.Parse(secondLine[1]);

			//Now we can determine tile bounds
			int tilesetColumns = _tileTexture.Width / _tileWidth;
			int tilesetRows = _tileTexture.Height / _tileHeight;
			_tiles = new Rectangle[tilesetColumns * tilesetRows];

			for (int y = 0; y < tilesetColumns; y++) 
			{
				int index = y;
				_tiles[index] = new Rectangle(index * _tileWidth, 0, _tileWidth, _tileHeight);
			}


			//Third Line
			var thirdLine = lines[2].Split(',');
			_mapWidth = int.Parse(thirdLine[0]);
			_mapHeight = int.Parse(thirdLine[1]);

			//Fourth Line (Now we create the map)
			var fourthLine = lines[3].Split(',');
			_map = new int[_mapWidth * _mapHeight];
			for (int y = 0; y < _mapHeight * _mapWidth; y++)
			{
				_map[y] = int.Parse(fourthLine[y]);
			}
		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch) 
		{
			for (int y = 0; y < _mapHeight; y++) 
			{
				for (int x = 0; x < _mapWidth; x++) 
				{
					int index = _map[y * _mapWidth + x] - 1;
					if(index == -1) 
					{
						continue;
					}
					spriteBatch.Draw(_tileTexture, new Vector2(x * _tileWidth, y * _tileHeight) + new Vector2(100, 250), _tiles[index], Color.White);
				}
			}
		}

	}
}
