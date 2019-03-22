using Microsoft.Xna.Framework;
using Monofoxe.Engine.ECS;
using Monofoxe.Engine.SceneSystem;


namespace Monofoxe.Demo.GameLogic.Entities
{
	/// <summary>
	/// Stores global checkpoint information.
	/// </summary>
	public class CheckpointManager : Entity
	{
		public new string Tag => "checkpointManager";	

		public Vector2 CheckpointPosition
		{
			get => _checkpointPosition;
			set
			{
				_checkpointPosition = value;
				NoCheckpointSet = false;
			}
		}

		private Vector2 _checkpointPosition;
		
		public string MapName = ""; 

		/// <summary>
		/// Tells if there was no checkpoint set.
		/// </summary>
		public bool NoCheckpointSet {get; private set;} = true;

		public CheckpointManager(Layer layer) : base(layer) {}


		public void ResetForNewMap(string mapName)
		{
			MapName = mapName;
			_checkpointPosition = Vector2.Zero;
			NoCheckpointSet = true;
		}
		
	}
}
