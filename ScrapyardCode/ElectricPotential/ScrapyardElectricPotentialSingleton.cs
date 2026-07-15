using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Scrapyard.ScrapyardCode.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;

namespace Scrapyard.ScrapyardCode.ElectricPotential;

/// <summary>
/// 电势全局单例：受伤降电势 + 回合开始比较差值伤害。
/// 放在单例而非 Power 上，确保每个事件只触发一次。
/// </summary>
[RegisterSingleton]
public class ScrapyardElectricPotentialSingleton : HookedSingletonModel
{
    public ScrapyardElectricPotentialSingleton() : base(HookType.Combat)
    {
    }

    // ── 受伤降电势 ──

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (!target.IsAlive) return;

        var power = target.GetPower<ScrapyardElectricPotentialPower>();
        int currentAmount = power?.Amount ?? 0;
        int newAmount = currentAmount - 1;

        if (power != null)
        {
            await PowerCmd.Remove(power);
        }

        await PowerCmd.Apply<ScrapyardElectricPotentialPower>(
            choiceContext,
            target,
            newAmount,
            dealer,
            cardSource);
    }

    // ── 敌人回合开始：电势比较伤害 ──

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != CombatSide.Enemy) return;

        var player = combatState.GetCreaturesOnSide(CombatSide.Player).FirstOrDefault(c => c.IsAlive);
        if (player == null) return;

        int playerAmount = player.GetPower<ScrapyardElectricPotentialPower>()?.Amount ?? 0;

        var enemies = combatState.GetCreaturesOnSide(CombatSide.Enemy).Where(e => e.IsAlive).ToList();

        foreach (var enemy in enemies)
        {
            int enemyAmount = enemy.GetPower<ScrapyardElectricPotentialPower>()?.Amount ?? 0;

            if (playerAmount > enemyAmount)
            {
                await CreatureCmd.Damage(
                    new ThrowingPlayerChoiceContext(),
                    new[] { enemy },
                    playerAmount - enemyAmount,
                    ValueProp.Unpowered,
                    null,
                    null,
                    null
                );
            }
            else if (enemyAmount > playerAmount)
            {
                await CreatureCmd.Damage(
                    new ThrowingPlayerChoiceContext(),
                    new[] { player },
                    enemyAmount - playerAmount,
                    ValueProp.Unpowered,
                    enemy,
                    null,
                    null
                );
            }
        }
    }
}
