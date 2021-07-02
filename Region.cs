using Godot;
using System;
using System.Collections.Generic;

namespace Scalability
{
	public class Region : Node2D
	{
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

		private Vector2 ignoreSideUntilExit = Vector2.Zero;
		public void PlayerEnteredSide( Node body, Area2D sender, RegionRoom room, Vector2 side )
		{
			if ( ignoreSideUntilExit == side )
				return;

			if ( !( body is Player player ) )
				return;

			if ( sender.GetParent() != room || player.GetParent() != room )
				return;

			var newRoom = GetRoom( new Vector2( ( int )( player.GlobalPosition.x / Constants.RoomSize ),
												( int )( player.GlobalPosition.y / Constants.RoomSize ) ) + side );
			if ( newRoom == null )
				return;

			GD.Print( "Moving to room " + newRoom.Name + " from room to the " + side );

			ignoreSideUntilExit = -side;

			var oldGlobal = player.GlobalPosition;
			room.RemoveChild( player );
			room.Active = false;
			newRoom.AddChild( player );
			newRoom.Active = true;
			player.GlobalPosition = oldGlobal + side * player.Radius * 2;

			player.SyncCameraToRoom();

			// TODO: Notify children that player left room
		}

		public void PlayerExitedSide( Node body, Area2D sender, RegionRoom room, Vector2 side )
		{
			if ( side == ignoreSideUntilExit )
				ignoreSideUntilExit = Vector2.Zero;
		}
	}
}
