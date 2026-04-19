using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Underverse_Test.Contents.Buffs.MinionBuff;
using Underverse_Test.Contents.Projectiles.Minions;

namespace Underverse_Test.Contents.Items.Weapons
{
    public class GasterBlasterSummonWeapon: ModItem
    {
        public override string Texture => "Underverse_Test/Contents/Assets/Gaster_blaster_summon";

        public override void SetStaticDefaults()
        {
            
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true; // Это позволяет игроку выбирать цель в любой точке экрана при использовании контроллера
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;

            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f; // Значение по умолчанию — 1, но поддерживаются и другие значения. Подробнее см. в документации.
        }
        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.knockBack = 0f;
            Item.mana = 100;
            Item.width = 50;
            Item.height = 70;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item44;

            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<GasterMinionBuff>();
            Item.shoot = ModContent.ProjectileType<Gaster_blaster_summon>();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Здесь вы можете изменить место появления миньона.
            // Большинство стандартных миньонов появляются в точке, на которую указывает курсор, в пределах игрового пространства.
            position = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref position);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Это необходимо для того, чтобы бафф, который поддерживает жизнь вашего миньона и позволяет корректно его дезактивировать, работал
            player.AddBuff(Item.buffType, 2);

            return true; // Снаряд с миньоном будет создан игрой, поскольку мы возвращаем true.
        }
        public override void AddRecipes()
        {
            int[] allTombstones = new int[]
            {
                ItemID.GraveMarker,
                ItemID.CrossGraveMarker,
                ItemID.Headstone,
                ItemID.Gravestone,
                ItemID.Obelisk,
            };

            foreach (int tombstoneID in allTombstones)
            {
                Recipe.Create(Type)
                    .AddIngredient(tombstoneID, 1)
                    .Register();
            }
        }

        // Этот миньон демонстрирует несколько обязательных действий, которые обеспечивают его корректное поведение. 
        // Схема его атаки проста: если враг находится в радиусе 43 клеток, он подлетит к нему и нанесет контактный урон.
        // Если игрок щелкнет правой кнопкой мыши по определенному NPC, миньон пролетит сквозь клетки и направится к нему.
        // Если миньон не атакует, он будет парить рядом с игроком, почти не двигаясь.
    }
}
