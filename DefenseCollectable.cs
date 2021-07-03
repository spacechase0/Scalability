using Godot;
using System;

namespace Scalability
{
	public class DefenseCollectable : Collectable
	{
		public override string GetText()
		{
			return "Defense +";
		}

		public override void Apply( Player player )
		{
			player.Defense /= 2;
		}
	}
}
