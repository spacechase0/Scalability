using Godot;
using System;

namespace Scalability
{
	public class SpeedCollectable : Collectable
	{
		public override string GetText()
		{
			return "Speed +";
		}

		public override void Apply( Player player )
		{
			player.MaxSpeed += 125;
			player.Acceleration += 10;
			player.Deacceleration += 5;
		}
	}
}
