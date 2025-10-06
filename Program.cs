namespace MysteryDungeon;

internal class Program
{
	private static void Main(string[] args)
	{
		DungeonGrid grid = new(new SFML.System.Vector2u(30, 20));
		grid.Generate(new SFML.System.Vector2u(3, 2));
		grid.Render();

		Console.ReadLine();
	}
}
