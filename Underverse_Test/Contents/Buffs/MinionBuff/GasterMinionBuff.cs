using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Underverse_Test.Contents.Projectiles.Minions;

namespace Underverse_Test.Contents.Buffs.MinionBuff
{
    public class GasterMinionBuff: ModBuff
    {
        public override string Texture => "Underverse_Test/Contents/Assets/GasterMinionBuff";
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // Этот бафф не сохранится при выходе из игры
            Main.buffNoTimeDisplay[Type] = true; // Оставшееся время не отображается на этом бaфе
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // Если миньоны есть, сбросьте время действия баффа, в противном случае снимите бафф с игрока
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Gaster_blaster_summon>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
