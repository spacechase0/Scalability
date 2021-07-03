using Godot;
using System;
using System.Collections.Generic;

namespace Scalability
{
	public class Region : Node2D
	{
		public const float CameraTweenDuration = 0.25f;

		private Dictionary<string, RegionRoom> roomsByName = new Dictionary<string, RegionRoom>();
		private Dictionary<Vector2, RegionRoom> roomsByPos = new Dictionary<Vector2, RegionRoom>();

		public RegionRoom GetRoom( string name )
		{
			if ( roomsByName.ContainsKey( name ) )
				return roomsByName[ name ];
			return null;
		}

		public RegionRoom GetRoom( Vector2 pos )
		{
			if ( roomsByPos.ContainsKey( pos ) )
				return roomsByPos[ pos ];
			return null;
		}

		public override void _Ready()
		{
			for ( int i = 0; i < GetChildCount(); ++i )
			{
				var child = GetChild( i );
				if ( !( child is RegionRoom room ) )
					continue;
				GD.Print( $"Adding room {room.Name}" );
				roomsByName.Add( room.Name, room );

				room.X = ( int )( room.Position.x / Constants.RoomSize );
				room.Y = ( int )( room.Position.y / Constants.RoomSize );

				Area2D MakeExitArea( Vector2 dir )
				{
					var area = new Area2D();
					area.Monitorable = false;
					area.Monitoring = true;
					area.CollisionMask = Constants.PhysicsMask_Player;

					var collShape = new CollisionShape2D();
					var rectShape = new RectangleShape2D();
					collShape.Shape = rectShape;
					if ( dir == Vector2.Left )
					{
						area.Position = new Vector2( 0, 0 ) * Constants.RoomSize;
						rectShape.Extents = new Vector2( 50, room.Height * Constants.RoomSize / 2 );
						area.Position += new Vector2( -rectShape.Extents.x, rectShape.Extents.y );
					}
					else if ( dir == Vector2.Right )
					{
						area.Position = new Vector2( room.Width, 0 ) * Constants.RoomSize;
						rectShape.Extents = new Vector2( 50, room.Height * Constants.RoomSize / 2 );
						area.Position += new Vector2( rectShape.Extents.x, rectShape.Extents.y );
					}
					else if ( dir == Vector2.Up )
					{
						area.Position = new Vector2( 0, 0 ) * Constants.RoomSize;
						rectShape.Extents = new Vector2( room.Width * Constants.RoomSize / 2, 50 );
						area.Position += new Vector2( rectShape.Extents.x, -rectShape.Extents.y );
					}
					else if ( dir == Vector2.Down )
					{
						area.Position = new Vector2( 0, room.Height ) * Constants.RoomSize;
						rectShape.Extents = new Vector2( room.Width * Constants.RoomSize / 2, 50 );
						area.Position += new Vector2( rectShape.Extents.x, rectShape.Extents.y );
					}
					area.AddChild( collShape );

					area.Connect( "body_entered", this, nameof( PlayerEnteredSide ), new Godot.Collections.Array() { area, room, dir }, ( int ) ConnectFlags.Deferred );
					area.Connect( "body_exited", this, nameof( PlayerExitedSide ), new Godot.Collections.Array() { area, room, dir }, ( int ) ConnectFlags.Deferred );
					room.AddChild( area );
					return area;
				}

				room.AddExit( MakeExitArea( Vector2.Left ) );
				room.AddExit( MakeExitArea( Vector2.Right ) );
				room.AddExit( MakeExitArea( Vector2.Up ) );
				room.AddExit( MakeExitArea( Vector2.Down ) );

				for ( int ix = room.X; ix < room.X + room.Width; ++ix )
				{
					for ( int iy = room.Y; iy < room.Y + room.Height; ++iy )
					{
						GD.Print( $"\tAdding room {room.Name} @ ({ix}, {iy})" );
						roomsByPos.Add( new Vector2( ix, iy ), room );
					}
				}

				room.Active = false;
			}
		}

		private bool transiting = false;
		public void PlayerEnteredSide( Node body, Area2D sender, RegionRoom room, Vector2 side )
		{
			if ( transiting )
				return;

			if ( !( body is Player player ) )
				return;

			if ( sender.GetParent() != room || player.GetParent() != room )
				return;

			var newRoom = GetRoom( new Vector2( Mathf.FloorToInt( player.GlobalPosition.x / Constants.RoomSize ),
												Mathf.FloorToInt( player.GlobalPosition.y / Constants.RoomSize ) ) + side );
			if ( newRoom == null || newRoom == room ) // Not sure why the latter happens
				return;

			GD.Print( "Moving to room " + newRoom.Name + " from room " + room.Name );

			transiting = true;

			var oldGlobal = player.GlobalPosition;
			room.RemoveChild( player );
			room.Active = false;
			room.Visible = true;
			newRoom.AddChild( player );
			newRoom.Visible = true;
			player.GlobalPosition = oldGlobal;

			var camTween = GetNode< Tween >( "CamTween" );
			camTween.InterpolateProperty( player, "global_position", player.GlobalPosition, oldGlobal + side * player.Radius * 2, CameraTweenDuration );
			player.SyncCameraToRoom( camTween, CameraTweenDuration );
			camTween.Connect( "tween_all_completed", this, nameof( CameraTweenFinished ), new Godot.Collections.Array() { room, newRoom }, ( int ) ConnectFlags.Oneshot );
			camTween.Start();
		}

		public void PlayerExitedSide( Node body, Area2D sender, RegionRoom room, Vector2 side )
		{
		}

		public void CameraTweenFinished( RegionRoom oldRoom, RegionRoom newRoom )
		{
			transiting = false;

			oldRoom.Visible = false;
			newRoom.Active = true;

			// TODO: Notify children of oldRoom that player left room
		}
	}
}
