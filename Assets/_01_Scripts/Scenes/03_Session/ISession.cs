public interface ISession
{
    RunState Run { get; }
    LootService Loot { get; }
    RunDeck Deck { get; }
    RunPerks Perks { get; }
    RunTimer RunTimer { get; }
    RunHeroData Hero { get; }
}
