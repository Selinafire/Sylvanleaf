using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Scrapyard.Characters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Scrapyard.Cards;

[RegisterCard(typeof(ScrapyardCardPool))]
public sealed class ScrapyardCoupling : ScrapyardCard
{
    protected override bool HasEnergyCostX => true;

    public override bool GainsBlock => true;

    public ScrapyardCoupling() : base(0, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var x = Math.Max(0, ResolveEnergyXValue());
        if (IsPrime(x))
        {
            var damage = IsUpgraded ? x + 1 : x;
            var hits = IsUpgraded ? x + 1 : x;
            var combatState = CombatState;
            ArgumentNullException.ThrowIfNull(combatState);

            await DamageCmd.Attack(damage)
                .WithHitCount(hits)
                .FromCard(this, cardPlay)
                .TargetingRandomOpponents(combatState, true)
                .Execute(choiceContext);
            return;
        }

        await CreatureCmd.GainBlock(
            Owner.Creature,
            IsUpgraded ? 2m * x : x,
            ValueProp.Move,
            cardPlay);
    }

    private static bool IsPrime(int value)
    {
        if (value < 2)
        {
            return false;
        }

        for (var i = 2; i * i <= value; i++)
        {
            if (value % i == 0)
            {
                return false;
            }
        }

        return true;
    }
}
