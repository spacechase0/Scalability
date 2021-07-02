using Godot;
using System;

namespace Scalability
{
	public class PlayerMelee : Area2D
	{
		[Export]
		public float Cooldown { get; set; } = 0.65f;

		public const int BaseSize = 24;

		public override void _Ready()
		{
			var tween = GetNode<Tween>( "Tween" );
			tween.InterpolateProperty( GetNode<Polygon2D>( "Polygon2D" ), "color:a", 1, 0, Cooldown, Tween.TransitionType.Linear );
			tween.Connect( "tween_all_completed", this, nameof( TweenCompleted ) );
			tween.Start();
		}

		public void TweenCompleted()
		{
			QueueFree();
		}
	}
}
