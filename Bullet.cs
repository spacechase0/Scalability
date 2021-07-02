using Godot;
using System;

namespace Scalability
{
	public class Bullet : KinematicBody2D
	{
		public float Speed { get; set; } = 500;

		public override void _PhysicsProcess( float delta )
		{
			Vector2 vel = new Vector2( Mathf.Cos( Rotation ), Mathf.Sin( Rotation ) ) * Speed * delta;
			var coll = MoveAndCollide( vel );
			if ( coll != null )
			{
				QueueFree();
			}
		}
	}
}
