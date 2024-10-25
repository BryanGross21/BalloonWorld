using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Text.Json;

namespace BalloonWorld.StateManagement
{
	public class SaveLoadGamePQ
	{
		private static string SaveFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BalloonWorldSave.json");
		public void SaveGame(SaveGame data)
		{
			string jsonData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(SaveFilePath, jsonData);
		}

		public SaveGame LoadGame()
		{
			if (File.Exists(SaveFilePath))
			{
				string jsonData = File.ReadAllText(SaveFilePath);
				var saveGame = JsonSerializer.Deserialize<SaveGame>(jsonData);
				return saveGame;
			}
			return null; // Or a new instance of SaveGame
		}
	}
}
