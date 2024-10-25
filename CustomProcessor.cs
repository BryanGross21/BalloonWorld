using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Audio;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace BalloonWorld
{
	[ContentProcessor(DisplayName = "Audio Processor")]
	public class CustomProcessor : ContentProcessor<object, object>
	{
		public override object Process(object input, ContentProcessorContext context)
		{
			if (input is AudioContent audioContent) 
			{
				if (audioContent.FileType == AudioFileType.Mp3)
				{
					var songProcesssor = new SongProcessor();
					return songProcesssor.Process(audioContent, context);
				}
				else 
				{
					var soundProcessor = new SoundEffectProcessor();
					return soundProcessor.Process(audioContent, context);
				}
			}
			return null;
		}
	}
}
