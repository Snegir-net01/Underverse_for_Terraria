using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Underverse_Test.Contents.Items.Weapons
{
    public class small_bone : ModItem
    {
        public override string Texture => "Underverse_Test/Contents/Assets/small_bone";

        public override void SetDefaults()
        {
            Item.damage = 1;
            Item.DamageType = DamageClass.Melee;
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 1;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.value = 0;
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.crit = -100;
            Item.mana = 1;
            Item.maxStack = 1;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Obelisk;
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
    }
}
