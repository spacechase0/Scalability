using Godot;
using System;

namespace Scalability
{
	public class MeleeEnemy : Enemy
	{
		[Export]
		public int Damage;

		[Export]
		public int Radius;

		public override void _Ready()
		{
			base._Ready();

			( GetNode<CollisionShape2D>( "CollisionShape2D" ).Shape as CircleShape2D ).Radius = Radius;
			var sphere = GetNode< MeshInstance2D >( "MeshInstance2D" ).Mesh as SphereMesh;
			sphere.Radius = Radius;
			sphere.Height = Radius * 2;
		}

		public override Vector2 GetWalkVector()
		{
			var player = GetParent().GetNodeOrNull( "Player" ) as Player;
			if ( player == null )
				return Vector2.Zero;

			float angle = GetAngleTo( player.GlobalPosition );
			return new Vector2( Mathf.Cos( angle ), Mathf.Sin( angle ) );
		}

		public override void _PhysicsProcess( float delta )
		{
			base._PhysicsProcess( delta );

			for ( int i = 0; i < GetSlideCount(); ++i )
			{
				var coll = GetSlideCollision( i );
				SomebodyCollided( coll.Collider as Node );
			}
		}

		public void SomebodyCollided( Node body )
		{
			if ( body is Player player )
			{
				player.Hurt( this, Damage );
			}    
		}
	}
}
