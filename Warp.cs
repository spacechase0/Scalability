using Godot;
using System;

namespace Scalability
{
	public class Warp : Area2D
	{
		[Export]
		public string Region { get; set; }

		[Export]
		public string Room { get; set; }

		[Export]
		public Vector2 Coordinates { get; set; }

		public override void _Ready()
		{
			Connect( "body_entered", this, nameof( SomebodyEntered ), flags: ( int ) ConnectFlags.Deferred );
		}

		public void SomebodyEntered( Node body )
		{
			if ( body is Player player )
			{
				player.GoToRegion( Region, Room, Coordinates );
			}
		}
	}
}
