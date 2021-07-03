using Godot;
using System;

namespace Scalability
{
	public class Enemy : Character
	{
		private int _maxHealth = 5;
		[Export]
		public int MaxHealth
		{
			get { return _maxHealth; }
			set
			{
				_maxHealth = value;
				Health = MaxHealth;
			}
		}

		public int Health { get; set; }

		[Export]
		public HitType VulnerableTo { get; set; }

		public void Hurt( HitType type, int amt )
		{
			if ( VulnerableTo != HitType.ANY && type != VulnerableTo )
				return;

			GD.Print( GetType() + " hurt with " + type + " for " + amt );

			Health -= amt;
			var healthBar = GetNode<ProgressBar>( "HealthBar" );
			healthBar.Value = Health;
			
			if ( Health <= 0 )
				QueueFree();
			else if ( Health < MaxHealth )
				healthBar.Visible = true;
			else
				healthBar.Visible = false;
		}

		public override void _Ready()
		{
			var healthBar = GetNode<ProgressBar>( "HealthBar" );
			healthBar.Value = healthBar.MaxValue = Health = MaxHealth;
		}
	}
}
