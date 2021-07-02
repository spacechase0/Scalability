using Godot;
using System;

namespace Scalability
{
	public class Character : KinematicBody2D
	{
		public Vector2 Velocity { get; set; }
		public float MaxSpeed { get; set; } = 200;
		public float Acceleration { get; set; } = 40;
		public float Deacceleration { get; set; } = 20;

		public virtual Vector2 GetWalkVector()
		{
			return Vector2.Zero;
		}

		public override void _PhysicsProcess( float delta )
		{
			Vector2 vel = Velocity;

			Vector2 movement = GetWalkVector();
			/*
			if ( movement.Length() != 0 )
				movement = movement.Normalized();
			//*/

			Vector2 target = movement * MaxSpeed;

			float accel = 0;
			if ( movement.Dot( vel ) > 0 )
				accel = Acceleration;
			else
				accel = Deacceleration;

			vel = vel.LinearInterpolate( target, accel * delta );

			Velocity = MoveAndSlide( vel );
		}
	}
}
