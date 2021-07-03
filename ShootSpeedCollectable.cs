using Godot;
using System;

namespace Scalability
{
	public class ShootSpeedCollectable : Collectable
	{
		public override string GetText()
		{
			return "Shoot Speed +";
		}

		public override void Apply( Player player )
		{
			player.ShootCooldownTime /= 2.5f;
		}
	}
}
