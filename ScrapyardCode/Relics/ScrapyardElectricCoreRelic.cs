using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Scrapyard.ScrapyardCode.ElectricPotential;
using Scrapyard.ScrapyardCode.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.Relics;

public abstract class ScrapyardElectricCoreRelic : ModRelicTemplate
{
    private const string TriggerCountKey = "TriggerCount";

    protected abstract int TriggerCount { get; }

    protected virtual decimal ElectricPotentialAmount => -3m;

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ScrapyardElectricPotentialPower>(ElectricPotentialAmount),
        new DynamicVar(TriggerCountKey, TriggerCount)
    ];

    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature)
            || Owner.PlayerCombatState is null
            || Owner.PlayerCombatState.TurnNumber > 1)
        {
            return;
        }

        for (var i = 0; i < TriggerCount; i++)
        {
            var target = Owner.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
            if (target is null)
            {
                return;
            }

            Flash();
            await ScrapyardElectricPotentialHelper.ApplyDelta(
                choiceContext,
                target,
                ElectricPotentialAmount,
                Owner.Creature,
                null);
        }
    }
}
