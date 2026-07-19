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
    protected abstract decimal ElectricPotentialAmount { get; }

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ScrapyardElectricPotentialPower>(ElectricPotentialAmount)
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

        Flash();
        await ScrapyardElectricPotentialHelper.ApplyDelta(
            choiceContext,
            Owner.Creature,
            ElectricPotentialAmount,
            Owner.Creature,
            null);
    }
}
