using Godot;
using System;

namespace Scalability
{
	public class PlayerMelee : Area2D, IHits
	{
		[Export]
		public float Cooldown { get; set; } = 0.65f;

		public const int BaseSize = 24;

		public bool HitsWithType( HitType type )
		{
			return type == HitType.Melee;
		}

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
