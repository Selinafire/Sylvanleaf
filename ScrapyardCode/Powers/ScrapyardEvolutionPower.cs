using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Scrapyard.ScrapyardCode.Powers;

[RegisterPower]
public class ScrapyardEvolutionPower : ModPowerTemplate
{
    private bool _isProcessingBonusDraw;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/ScrapyardEvolutionPower.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/ScrapyardEvolutionPowerBig.png");

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool wasDrawnNaturally)
    {
        if (_isProcessingBonusDraw || card.Owner != Owner.Player || card.Type != CardType.Status)
        {
            return;
        }

        _isProcessingBonusDraw = true;
        Flash();
        await CardPileCmd.Draw(choiceContext, (int)Amount, Owner.Player);
        _isProcessingBonusDraw = false;
    }
}
