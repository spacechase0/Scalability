using Godot;
using System;

namespace Scalability
{
	public class ShootCostCollectable : Collectable
	{
		public override string GetText()
		{
			return "Shoot Cost -";
		}

		public override void Apply( Player player )
		{
			player.ShootRadiusCost /= 2;
		}
	}
}
