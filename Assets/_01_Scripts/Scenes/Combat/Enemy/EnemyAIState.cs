public class EnemyAIState
{
    public System.Random Rng { get; }
    public EnemyMoveSO CurrentMove { get; private set; }

    public EnemyAIState(int seed) => Rng = new System.Random(seed);

    public void SetMove(EnemyMoveSO move) => CurrentMove = move;
}
