using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace MysteryDungeon;

internal static class Program
{
	public static RenderWindow Window { get; set; }

	private static void Main()
	{
		Window = new(new VideoMode(800, 600), "Mystery Dungeon", Styles.Titlebar | Styles.Close);

		Window.Closed += (_, _) => Window.Close();

		DungeonGrid grid = new();
		grid.Generate(new Vector2i(60, 45), new Vector2i(4, 3), 8);
		Minimap minimap = new(grid);

		while (Window.IsOpen)
		{
			Window.DispatchEvents();

			Window.Clear();
			Window.Draw(minimap);
			Window.Display();
		}
	}
}
