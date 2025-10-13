using SFML.Graphics;

namespace MysteryDungeon;

public enum Team
{
    Neutral,
    Friendly,
    Hostile,
}

internal class Monster(string species, Team team) : Entity
{
    public string Species { get; } = species;

    public Team Team { get; } = team;

    public override Color MinimapDraw()
    {
        switch (this.Team)
        {
            case Team.Neutral:
                return Color.Cyan;
            case Team.Friendly:
                return Color.Yellow;
            case Team.Hostile:
                return Color.Red;
            default:
                return Color.Magenta;
        }

    }
}
