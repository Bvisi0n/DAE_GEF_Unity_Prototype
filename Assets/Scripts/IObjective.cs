namespace Assets.Scripts
{
	public enum ObjectiveType
	{
		SpawnPoint,
		Enemy
	}

	public interface IObjective
	{
		ObjectiveType Type { get; }
	}
}
