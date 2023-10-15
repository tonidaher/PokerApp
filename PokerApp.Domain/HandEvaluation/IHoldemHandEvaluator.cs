using PokerApp.Domain.DataModel;
namespace PokerApp.Domain.HandEvaluation
{
    public interface IHoldemHandEvaluator
    {
        HandEvaluationResult EvaluateHand(string cards, string board, int playersCount);
    }
}
