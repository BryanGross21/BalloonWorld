using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BalloonWorld
{
	public class squareParticleSystem : ParticleSystem
	{
			public IParticleEmitter _emitter;

			public squareParticleSystem(Game game, IParticleEmitter emitter) : base(game, 2000)
			{
				_emitter = emitter;
			}

			protected override void InitializeConstants()
			{
				textureFilename = "square";
				minNumParticles = 2;
				maxNumParticles = 3;
				blendState = BlendState.Additive;

				DrawOrder = AdditiveBlendDrawOrder;
			}

			protected override void InitializeParticle(ref Particle p, Vector2 where)
			{
				var Velocity = _emitter.velocity;
				var Acceleration = Vector2.UnitY * 400;
				var scale = .5f;
				var lifetime = RandomHelper.NextFloat(0.1f, 1.0f);
				var color = new Color();
				if (RandomHelper.Next(1, 3) == 1) 
				{
				color = Color.Green;	
				}
				else{
				color = Color.Purple;
				}
				p.Initialize(where, Velocity, Acceleration, color: color, scale: scale, lifetime: lifetime);
				base.InitializeParticle(ref p, where);
			}

			public override void Update(GameTime gameTime)
			{
				base.Update(gameTime);

				AddParticles(_emitter.Position);
			}
		}
}
