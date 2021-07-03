using Godot;
using System;

namespace Scalability
{
	public class Collectable : Area2D
	{
		[Export]
		public string Tag { get; set; }

		public virtual string GetText()
		{
			return "Absolutely Nothing";
		}

		public virtual void Apply( Player player )
		{
		}

		public override void _Ready()
		{
			if ( PlayScene.player != null && PlayScene.player.Collected.Contains( Tag ) )
				QueueFree();

			Connect( "body_entered", this, nameof( SomeoneCollected ) );
		}

		public override void _Process( float delta )
		{
			var glow = GetNode( "BG_Glow" );
			foreach ( var child_ in glow.GetChildren() )
			{
				var child = ( MeshInstance2D ) child_;

				child.Scale += new Vector2( delta, delta );
				if ( child.Scale.x >= 2 )
				{
					child.Scale = new Vector2( 0.33f, 0.33f );
				}
			}
		}

		public void SomeoneCollected( Node body )
		{
			if ( body is Player player )
			{
				player.Collect( this );
				QueueFree();
			}
		}
	}
}
