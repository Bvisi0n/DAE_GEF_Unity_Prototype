namespace Assets.Scripts
{
	public interface IObjective
	{
		public enum ObjectiveType { SpawnPoint, Enemy }
		public ObjectiveType Type { get; }
	}
}
