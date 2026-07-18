using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Scrapyard.Energy;
using Scrapyard.Keywords;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.ScrapyardCode.Powers;

[RegisterPower]
public class ScrapyardDynamicCompilePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/ScrapyardDynamicCompilePower.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/ScrapyardDynamicCompilePowerBig.png");

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
        {
            return;
        }

        if (ScrapyardKeywordState.WasInitiativeTriggered(cardPlay.Card))
        {
            Flash();
            await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
        }

        if (ScrapyardKeywordState.WasFollowUpTriggered(cardPlay.Card))
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, Amount >= 2 ? 7m : 5m, ValueProp.Move, cardPlay);
        }

        if (ScrapyardKeywordState.WasDecisiveTriggered(cardPlay.Card))
        {
            Flash();
            ScrapyardEnergySystem.GainAdditive(Owner.Player, 1);
        }
    }
}
