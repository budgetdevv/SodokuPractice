namespace Sodoku.Solvers
{
    public interface ISolver
    {
        public bool TrySolve(Board board);
    }
}