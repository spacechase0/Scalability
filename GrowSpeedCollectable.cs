using Godot;
using System;

namespace Scalability
{
	public class GrowSpeedCollectable : Collectable
	{
		public override string GetText()
		{
			return "Growth Speed +";
		}

		public override void Apply( Player player )
		{
			player.GrowSpeed += 8;
		}
	}
}
