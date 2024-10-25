using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Audio;

namespace BalloonWorld
{
	/// <summary>
	/// Handles audio importing
	/// </summary>
	[ContentImporter(".ogg, .mp3, .wav, .wma", DefaultProcessor = "CustomSoundProcessor", DisplayName = "Custom Sound Importer")]
	public class CustomImporter : ContentImporter<object>
	{

		public override object Import(string filename, ContentImporterContext context)
		{
			string extension = Path.GetExtension(filename).ToLower();

			if (extension == ".ogg" || extension == ".wav" || extension == ".mp3" || extension == ".wma")
			{
				var type = AudioFileType.Wma;
				if (extension == ".ogg")
				{
					type = AudioFileType.Ogg;
				}
				else if (extension == ".wav")
				{
					type = AudioFileType.Wav;
				}
				else if (extension == ".mp3") 
				{
					type = AudioFileType.Mp3;
				}
				return new AudioContent(filename, type);
			}
			else
			{
				throw new InvalidContentException("Unsupported file format: " + extension);
			}
		}

	}
}
