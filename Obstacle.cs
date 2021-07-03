using Godot;
using System;

namespace Scalability
{
	public class Obstacle : RigidBody2D
	{
		[Export]
		public HitType BreakType { get; set; } = HitType.Melee;

		public override void _Ready()
		{
			Connect( "body_entered", this, nameof( SomebodyCollided ) );
		}

		public void SomebodyCollided( Node body )
		{
			if ( body is IHits hits && ( hits.HitsWithType( BreakType ) || BreakType == HitType.ANY ) )
				QueueFree();
		}
	}
}
