using Godot;
using System;

namespace Scalability
{
	public class TurretEnemy : Enemy
	{
		[Export]
		public int Damage = 6;

		[Export]
		public float ChargeTime = 0.5f;

		[Export]
		public int Radius = 24;

		public float chargeTime = 0;

		private static readonly PackedScene EnemyBullet_Scene = GD.Load< PackedScene >( "res://EnemyBullet.tscn" );

		public override void _Ready()
		{
			base._Ready();

			var healthBar = ( GetNode< ProgressBar >( "HealthBar" ) );
			healthBar.RectPosition = new Vector2( healthBar.RectPosition.x, -Radius - 16 - healthBar.RectSize.y );

			( GetNode<CollisionShape2D>( "CollisionShape2D" ).Shape as CircleShape2D ).Radius = Radius;
			var sphere = GetNode< MeshInstance2D >( "MeshInstance2D" ).Mesh as SphereMesh;
			sphere.Radius = Radius;
			sphere.Height = Radius * 2;

			chargeTime = ChargeTime;
		}

		public override void _Process( float delta)
		{
			var player = GetParent().GetNodeOrNull( "Player" ) as Player;
			if ( player == null )
				return;

			chargeTime -= delta;
			if ( chargeTime <= 0 )
			{
				chargeTime = ChargeTime;

				var bullet = ( Bullet ) EnemyBullet_Scene.Instance();
				bullet.Damage = 6;
				bullet.Rotation = GetAngleTo( player.GlobalPosition );
				GetParent().AddChild( bullet );
				bullet.Position = Position;
			}
		}
	}
}
