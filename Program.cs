using SFML.System;

namespace MysteryDungeon;

internal static class Program
{
	private static void Main()
	{
		while (true)
		{
			DungeonGrid grid = new();
			grid.Generate(new Vector2i(60, 30), new Vector2i(6, 3), 7);
			grid.Render();

			var key = Console.ReadKey();

			if (key.Key == ConsoleKey.Enter)
			{
				break;
			}
			Console.Clear();
		}
	}
}
