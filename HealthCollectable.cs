using Godot;
using System;

namespace Scalability
{
	public class HealthCollectable : Collectable
	{
		public override string GetText()
		{
			return "Max Health";
		}

		public override void Apply( Player player )
		{
			player.MaxHealth += 10;
		}
	}
}
