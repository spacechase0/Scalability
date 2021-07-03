using Godot;
using System;
using System.Collections.Generic;

namespace Scalability
{
	public class Player : Character
	{
		public const float InvulnerableTime = 0.6f;
		public const float InvulnerableFlash = 0.2f;

		private int _maxHealth = 10;
		public int MaxHealth
		{
			get { return _maxHealth; }
			set
			{
				Health += value - _maxHealth;
				_maxHealth = value;
			}
		}
		public int Health { get; set; } = 10;

		public int Defense { get; set; } = 0;

		public float Radius { get; private set; } = 32;
		public float MinRadius { get; set; } = 24;
		public float MaxRadius { get; set; } = 64;
		public float GrowSpeed { get; set; } = 12;
		public float ShootCooldownTime { get; set; } = 0.5f;
		public float ShootRadiusCost { get; set; } = 1f;

		public HashSet<string> Collected { get; } = new HashSet<string>();

		private bool doingMouseInput = false;

		[Signal]
		public delegate void Damaged();

		[Signal]
		public delegate void CollectedCollectable(Collectable collectable);

		private float shootCooldown = 0;
		private float invulnTime = 0;

		private static PackedScene Bullet_Scene = GD.Load< PackedScene >( "res://Bullet.tscn" );
		private static PackedScene Melee_Scene = GD.Load< PackedScene >( "res://PlayerMelee.tscn" );

		private void SyncSizeStats()
		{
			MaxSpeed = 325 - Radius * 4;
			Acceleration = MaxSpeed / 5;
			Deacceleration = Acceleration / 2;
			Defense = ( int ) ( Radius / 20 );
		}

		public void Collect( Collectable collectable )
		{
			if ( collectable.Tag != null && !Collected.Contains( collectable.Tag ) )
				Collected.Add( collectable.Tag );

			collectable.Apply( this );

			EmitSignal( nameof( CollectedCollectable ), collectable );
		}

		public void Hurt( Node source, int amount )
		{
			if ( invulnTime > 0 )
				return;

			int amountReduced = Math.Max( amount - Defense, 1 );
			GD.Print( "Player hurt by " + source + " for " + amount +"->" + amountReduced );

			Health -= amountReduced;
			invulnTime = InvulnerableTime;

			EmitSignal( nameof( Damaged ) );

			if ( Health <= 0 )
			{
				QueueFree();
				GetTree().ChangeScene( "res://scenes/GameOverScene.tscn" );
			}
		}

		public void GoToRegion( string region, string room, Vector2 coords )
        {
			Health = MaxHealth;
			GetParent<RegionRoom>().GetParent<Region>().GetParent<PlayScene>().SetupWarp( region, room, coords );
        }

		public override Vector2 GetWalkVector()
		{
			return new Vector2( -Input.GetActionStrength( "move_left" ) + Input.GetActionStrength( "move_right" ),
								-Input.GetActionStrength( "move_up"   ) + Input.GetActionStrength( "move_down" ) );
		}

		public void SyncCameraToRoom()
		{
			var room = GetParent< RegionRoom >();
			var cam = GetNode< Camera2D >( "Camera2D" );
			cam.LimitLeft = room.X * Constants.RoomSize;
			cam.LimitTop = room.Y * Constants.RoomSize;
			cam.LimitRight = ( room.X + room.Width ) * Constants.RoomSize;
			cam.LimitBottom = ( room.Y + room.Height ) * Constants.RoomSize;
		}

		public void SyncCameraToRoom( Tween tween, float tweenDur )
		{
			var room = GetParent< RegionRoom >();
			var cam = GetNode< Camera2D >( "Camera2D" );
			tween.InterpolateProperty( cam, "limit_left", cam.LimitLeft, room.X * Constants.RoomSize, tweenDur );
			tween.InterpolateProperty( cam, "limit_top", cam.LimitTop, room.Y * Constants.RoomSize, tweenDur );
			tween.InterpolateProperty( cam, "limit_right", cam.LimitRight, ( room.X + room.Width ) * Constants.RoomSize, tweenDur );
			tween.InterpolateProperty( cam, "limit_bottom", cam.LimitBottom, ( room.Y + room.Height ) * Constants.RoomSize, tweenDur );
		}

		public override void _Ready()
		{
			SyncSizeStats();
		}

		public override void _UnhandledInput( InputEvent @event )
		{
			if ( @event is InputEventMouseMotion mm )
			{
				GetTree().SetInputAsHandled();
				doingMouseInput = true;
			}
		}

		public override void _Process( float delta )
		{
			if ( invulnTime > 0 )
			{
				invulnTime -= delta;
				if ( invulnTime <= 0 )
					Visible = true;
				else
					Visible = ( invulnTime % InvulnerableFlash ) > InvulnerableFlash / 2;
			}

			Vector2 joystick = new Vector2( Input.GetJoyAxis( 0, ( int ) JoystickList.Axis2 ), Input.GetJoyAxis( 0, ( int ) JoystickList.Axis3 ) );
			if ( joystick.Length() >= 0.15f )
			{
				doingMouseInput = false;
				Rotation = joystick.Angle();
			}

			if ( doingMouseInput )
				LookAt( GetGlobalMousePosition() );

			float oldRad = Radius;
			if ( Input.IsActionPressed( "shrink" ) )
				Radius -= GrowSpeed * delta * 2;
			if ( Input.IsActionPressed( "grow" ) )
				Radius += GrowSpeed * delta;
			if ( Radius < MinRadius ) Radius = MinRadius;
			if ( Radius > MaxRadius ) Radius = MaxRadius;

			if ( shootCooldown > 0 ) shootCooldown -= delta;
			if ( Input.IsActionPressed( "shoot" ) && shootCooldown <= 0 && Radius >= MinRadius + ShootRadiusCost )
			{
				var bullet = ( Bullet ) Bullet_Scene.Instance();
				bullet.Rotation = Rotation;
				GetParent().AddChild( bullet );
				bullet.Position = Position;
				Radius -= ShootRadiusCost;
				shootCooldown = ShootCooldownTime;
			}

			var melee = GetNodeOrNull< PlayerMelee >( "PlayerMelee" );
			if ( Input.IsActionJustPressed( "melee" ) && melee == null )
			{
				melee = ( PlayerMelee ) Melee_Scene.Instance();
				melee.Connect( "body_entered", this, nameof( SomethingEnteredMelee ), flags: ( int ) ConnectFlags.Oneshot );
				melee.Scale = new Vector2( Radius / PlayerMelee.BaseSize, Radius / PlayerMelee.BaseSize );
				melee.Position = new Vector2( Radius, 0 );
				AddChild( melee );
			}

			if ( Radius != oldRad )
			{
				( GetNode< CollisionShape2D >( "CollisionShape2D" ).Shape as CircleShape2D ).Radius = Radius;
				var mesh = ( GetNode< MeshInstance2D >( "MeshInstance2D" ).Mesh as SphereMesh );
				mesh.Radius = Radius;
				mesh.Height = Radius * 2;

				SyncSizeStats();
			}
		}

		public void SomethingEnteredMelee( Node body )
		{
			GD.Print( "Player melee'd " + body );
			if ( body is Obstacle obstacle )
				obstacle.SomebodyCollided( GetNode( "PlayerMelee" ) );
			else if ( body is Enemy enemy )
				enemy.Hurt( HitType.Melee, ( int ) ( Radius / 12 ) );
		}
	}

}
