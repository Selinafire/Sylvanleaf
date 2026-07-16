using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Scrapyard.ScrapyardCode.Powers;

namespace Scrapyard.ScrapyardCode.ElectricPotential;

public static class ScrapyardElectricPotentialHelper
{
    public static bool IsCharged(Creature creature)
    {
        return creature.GetPower<ScrapyardElectricPotentialPower>()?.Amount is not null and not 0;
    }

    public static int CountChargedUnits(ICombatState? combatState)
    {
        if (combatState is null)
        {
            return 0;
        }

        return combatState.GetCreaturesOnSide(CombatSide.Player)
            .Concat(combatState.GetCreaturesOnSide(CombatSide.Enemy))
            .Count(creature => creature.IsAlive && IsCharged(creature));
    }

    public static Task ApplyDelta(
        PlayerChoiceContext choiceContext,
        Creature target,
        decimal amount,
        Creature? source,
        CardModel? cardSource)
    {
        return PowerCmd.Apply<ScrapyardElectricPotentialPower>(choiceContext, target, amount, source, cardSource);
    }

    public static Task ApplyDelta(
        PlayerChoiceContext choiceContext,
        IEnumerable<Creature> targets,
        decimal amount,
        Creature? source,
        CardModel? cardSource)
    {
        return PowerCmd.Apply<ScrapyardElectricPotentialPower>(choiceContext, targets, amount, source, cardSource);
    }
}
