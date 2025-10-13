using SFML.Graphics;

namespace MysteryDungeon;

internal class Item(string kind) : Entity
{
    public string Kind { get; } = kind;

    public override Color MinimapDraw()
    {
        return Color.Blue;
    }
}
