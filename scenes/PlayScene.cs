using Godot;
using System;

namespace Scalability
{
	public class PlayScene : Node
	{
		//*
		private const string StartingRegion = "Test";
		private const string StartingRoom = "IntroExit";
		/*/
		private const string StartingRegion = "Overworld";
		private const string StartingRoom = "Hub";
		//*/
		private static readonly Vector2 StartingPosition = new Vector2( 50, 50 );
		private static readonly PackedScene Player_Scene = GD.Load< PackedScene >( "res://Player.tscn" );

		public static Player player;

		public void SetupWarp( string region, string room, Vector2 coords )
		{
			if ( player.GetParent() != null )
				player.GetParent().RemoveChild( player );
			var active = GetNodeOrNull< Region >( "ActiveRegion" );
			if ( active != null )
			{
				RemoveChild( active );
				active.Free();
			}

			var neWRegion = ( Region ) GD.Load< PackedScene >( $"res://world/{region}Region.tscn" ).Instance();
			neWRegion.Name = "ActiveRegion";
			AddChild( neWRegion );

			var startRoom = neWRegion.GetRoom( room );
			player.Position = coords;
			startRoom.AddChild( player );
			startRoom.Active = true;
			player.SyncCameraToRoom();
		}

		public override void _Ready()
		{
			player = ( Player ) Player_Scene.Instance();
			player.Name = "Player";

			SetupWarp( StartingRegion, StartingRoom, StartingPosition );

			player.Connect( nameof( Player.Damaged ), this, nameof( OnPlayerDamaged ) );
			player.Connect( nameof( Player.CollectedCollectable ), this, nameof( OnCollected ) );
		}

		public void OnPlayerDamaged()
		{
			var healthBar = GetNode< ProgressBar >( "UI/HealthBar" );
			healthBar.Value = player.Health;
			healthBar.MaxValue = player.MaxHealth;
		}

		public void OnCollected( Collectable collectable )
		{
			var label = GetNode< Label >( "UI/CollectedLabel" );
			var tween = GetNode< Tween >( "CollectedTween" );

			label.Text = "You got " + collectable.GetText();
			tween.InterpolateProperty( label, "modulate:a", 1f, 0f, 4f );
			tween.Start();
		}
	}
}
