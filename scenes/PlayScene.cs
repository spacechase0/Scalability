using Godot;
using System;

namespace Scalability
{
	public class PlayScene : Node
	{
		private const string StartingRegion = "Test";
		private const string StartingRoom = "Intro";
		private static readonly Vector2 StartingPosition = new Vector2( 400, 400 );
		private PackedScene Player_Scene = GD.Load< PackedScene >( "res://Player.tscn" );

		private Player player;

		public override void _Ready()
		{
			var region = ( Region ) GD.Load< PackedScene >( $"res://world/{StartingRegion}Region.tscn" ).Instance();
			region.Name = "ActiveRegion";
			AddChild( region );

			var startRoom = region.GetRoom( StartingRoom );
			player = ( Player ) Player_Scene.Instance();
			player.Name = "Player";
			startRoom.AddChild( player );
			player.Position = StartingPosition;
			startRoom.Active = true;
			player.SyncCameraToRoom();
			player.Damaged += OnPlayerDamaged;
		}

		private void OnPlayerDamaged( object sender, EventArgs args )
		{
			var player = sender as Player;

			var healthBar = GetNode< ProgressBar >( "UI/HealthBar" );
			healthBar.Value = player.Health;
			healthBar.MaxValue = player.MaxHealth;
		}
	}
}
