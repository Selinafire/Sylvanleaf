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
    private static bool _isPotentialDamage;

    public override async Task AfterDamageGiven(
        PlayerChoiceContext choiceContext,
        Creature? dealer,
        DamageResult result,
        ValueProp props,
        Creature target,
        CardModel? cardSource)
    {
        if (_isPotentialDamage || !target.IsAlive) return;

        var power = target.GetPower<ScrapyardElectricPotentialPower>();
        if (power == null) return;

        int newAmount = power.Amount - 1;

        await PowerCmd.Remove(power);

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

        var players = combatState.GetCreaturesOnSide(CombatSide.Player).Where(c => c.IsAlive).ToList();
        if (players.Count == 0) return;

        var enemies = combatState.GetCreaturesOnSide(CombatSide.Enemy).Where(e => e.IsAlive).ToList();

        // 每个玩家与每个敌人比较电势
        foreach (var player in players)
        {
            int playerAmount = player.GetPower<ScrapyardElectricPotentialPower>()?.Amount ?? 0;

            foreach (var enemy in enemies)
            {
                int enemyAmount = enemy.GetPower<ScrapyardElectricPotentialPower>()?.Amount ?? 0;

                if (playerAmount > enemyAmount)
                {
                    _isPotentialDamage = true;
                    await CreatureCmd.Damage(
                        new ThrowingPlayerChoiceContext(),
                        new[] { enemy },
                        playerAmount - enemyAmount,
                        ValueProp.Unpowered,
                        null,
                        null,
                        null
                    );
                    _isPotentialDamage = false;
                }
                else if (enemyAmount > playerAmount)
                {
                    _isPotentialDamage = true;
                    await CreatureCmd.Damage(
                        new ThrowingPlayerChoiceContext(),
                        new[] { player },
                        enemyAmount - playerAmount,
                        ValueProp.Unpowered,
                        null,
                        null,
                        null
                    );
                    _isPotentialDamage = false;
                }
            }
        }

        // 任意玩家拥有互感时，敌人之间也比较电势
        bool anyPlayerHasMutualInduction = players.Any(p => p.GetPower<ScrapyardMutualInductionPower>() is not null);
        if (!anyPlayerHasMutualInduction)
        {
            return;
        }

        for (var i = 0; i < enemies.Count; i++)
        {
            for (var j = i + 1; j < enemies.Count; j++)
            {
                await DamageLowerPotentialEnemy(enemies[i], enemies[j]);
            }
        }
    }

    private static async Task DamageLowerPotentialEnemy(Creature first, Creature second)
    {
        int firstAmount = first.GetPower<ScrapyardElectricPotentialPower>()?.Amount ?? 0;
        int secondAmount = second.GetPower<ScrapyardElectricPotentialPower>()?.Amount ?? 0;

        if (firstAmount == secondAmount)
        {
            return;
        }

        var target = firstAmount < secondAmount ? first : second;
        var damage = Math.Abs(firstAmount - secondAmount);

        _isPotentialDamage = true;
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            new[] { target },
            damage,
            ValueProp.Unpowered,
            null,
            null,
            null
        );
        _isPotentialDamage = false;
    }
}
