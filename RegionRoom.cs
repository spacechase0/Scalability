using Godot;
using System;
using System.Collections.Generic;

namespace Scalability
{
	public class RegionRoom : Node2D
	{
		public int X { get; set; } = int.MinValue; // Set automatically

		public int Y { get; set; } = int.MinValue; // Set automatically

		[Export]
		public int Width { get; set; } = 1;

		[Export]
		public int Height { get; set; } = 1;

		private bool _active = false;
		public bool Active
        {
			get { return _active; }
			set
            {
				_active = value;
				if ( !_active )
				{
					PauseMode = PauseModeEnum.Stop;
					Visible = false;
					foreach ( var exit in exits )
						exit.Monitoring = false;
				}
				else
                {
					PauseMode = PauseModeEnum.Inherit;
					Visible = true;
					foreach ( var exit in exits )
						exit.Monitoring = true;
				}
            }
        }

		private List<Area2D> exits = new List<Area2D>();

		public void AddExit( Area2D exit )
        {
			exits.Add( exit );
        }
	}
}