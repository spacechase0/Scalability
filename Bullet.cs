using Godot;
using System;

namespace Scalability
{
	public class Bullet : KinematicBody2D, IHits
	{
		[Export]
		public bool HurtsPlayer { get; set; } = false;

		[Export]
		public int Damage { get; set; } = 3;

		[Export]
		public float Speed { get; set; } = 500;

		public bool HitsWithType( HitType type )
		{
			return type == HitType.Projectile;
		}

		public override void _PhysicsProcess( float delta )
		{
			Vector2 vel = new Vector2( Mathf.Cos( Rotation ), Mathf.Sin( Rotation ) ) * Speed * delta;
			var coll = MoveAndCollide( vel );
			if ( coll != null )
			{
				// Not sure why this didn't work to begin with without this...
				// Probably got the layers wrong or something
				if ( coll.Collider is Obstacle obstacle )
					obstacle.SomebodyCollided( this );

				else if ( !HurtsPlayer && coll.Collider is Enemy enemy )
					enemy.Hurt( HitType.Projectile, Damage );

				else if ( HurtsPlayer && coll.Collider is Player player )
					player.Hurt( this, Damage );

				QueueFree();
			}
		}
	}
}
