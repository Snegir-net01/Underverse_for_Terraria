using ExampleMod.Content.Items;
using ExampleMod.Content.Tiles.Furniture;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Projectiles.Minions
{
	// This file contains all the code necessary for a minion
	// - ModItem - the weapon which you use to summon the minion with
	// - ModBuff - the icon you can click on to despawn the minion
	// - ModProjectile - the minion itself

	// It is not recommended to put all these classes in the same file. For demonstrations sake they are all compacted together so you get a better overview.
	// To get a better understanding of how everything works together, and how to code minion AI, read the guide: https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
	// This is NOT an in-depth guide to advanced minion AI

	// Этот файл содержит весь код, необходимый для создания миньона
	// - ModItem - оружие, с помощью которого вы вызываете миньона
	// - ModBuff - значок, по которому вы можете нажать, чтобы уничтожить миньона
	// - ModProjectile - сам миньон

	// Не рекомендуется помещать все эти классы в один и тот же файл. Для наглядности все они собраны вместе, чтобы вы могли лучше видеть.
	// Чтобы лучше понять, как все это работает и как программировать ИИ для миньонов, прочтите руководство: https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
	// Это НЕ подробное руководство по продвинутому ИИ для миньонов

	public class ExampleSimpleMinionBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoSave[Type] = true; // This buff won't save when you exit the world

			Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
		}

		public override void Update(Player player, ref int buffIndex) {
			// If the minions exist reset the buff time, otherwise remove the buff from the player
			if (player.ownedProjectileCounts[ModContent.ProjectileType<ExampleSimpleMinion>()] > 0) {
				player.buffTime[buffIndex] = 18000;
			}
			else {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	public class ExampleSimpleMinionItem : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.GamepadWholeScreenUseRange[Type] = true; // This lets the player target anywhere on the whole screen while using a controller
			ItemID.Sets.LockOnIgnoresCollision[Type] = true;

			ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f; // The default value is 1, but other values are supported. See the docs for more guidance.
		}

		public override void SetDefaults() {
			Item.damage = 30;
			Item.knockBack = 3f;
			Item.mana = 10; // mana cost
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing; // how the player's arm moves when using the item
			Item.value = Item.sellPrice(gold: 30);
			Item.rare = ItemRarityID.Cyan;
			Item.UseSound = SoundID.Item44; // What sound should play when using the item

			// These below are needed for a minion weapon
			Item.noMelee = true; // this item doesn't do any melee damage
			Item.DamageType = DamageClass.Summon; // Makes the damage register as summon. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type
			Item.buffType = ModContent.BuffType<ExampleSimpleMinionBuff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ModContent.ProjectileType<ExampleSimpleMinion>(); // This item creates the minion projectile
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position, limited by the gameplay range
			position = Main.MouseWorld;
			player.LimitPointToPlayerReachableArea(ref position);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

			return true; // The minion projectile will be spawned by the game since we return true.
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<ExampleItem>())
				.AddTile(ModContent.TileType<ExampleWorkbench>())
				.Register();
		}
	}

	// This minion shows a few mandatory things that make it behave properly.
	// Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
	// If the player targets a certain NPC with right-click, it will fly through tiles to it
	// If it isn't attacking, it will float near the player with minimal movement
	public class ExampleSimpleMinion : ModProjectile
	{
		public override void SetStaticDefaults() {
			// Устанавливает количество кадров у этого миниона на его спрайте
			Main.projFrames[Type] = 4;

			// Необходимо для возможности указывать цель правой кнопкой мыши
			ProjectileID.Sets.MinionTargettingFeature[Type] = true;

			Main.projPet[Type] = true;           // Обозначает, что это питомец или минион
			ProjectileID.Sets.MinionSacrificable[Type] = true; // Нужно, чтобы минион правильно призывался и заменялся при призыве других минионов
			ProjectileID.Sets.CultistIsResistantTo[Type] = true; // Делает Культиста устойчивым к этому снаряду (как и ко всем homing-снарядам)
		}

		public sealed override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 28;
			Projectile.tileCollide = false; // Минион свободно проходит сквозь блоки

			// Эти параметры нужны для оружия, призывающего минионов
			Projectile.friendly = true;           // Управляет нанесением контактного урона врагам
			Projectile.minion = true;             // Объявляет снаряд минионом (много различных эффектов)
			Projectile.DamageType = DamageClass.Summon; // Тип урона (обязательно для нанесения урона)
			Projectile.minionSlots = 1f;          // Сколько слотов минионов занимает этот минион
			Projectile.penetrate = -1;            // Нужно, чтобы минион не исчезал при столкновении с врагами или блоками
		}

		// Здесь можно решить, будет ли минион ломать траву, горшки и т.д.
		public override bool? CanCutTiles() {
			return false;
		}

		// Обязательно, если минион наносит контактный урон
		public override bool MinionContactDamage() {
			return true;
		}

		// AI миниона разделён на несколько методов, чтобы код не был слишком большим.
		// Этот метод просто передаёт значения между частями AI.
		public override void AI() {
			Player owner = Main.player[Projectile.owner];

			if (!CheckActive(owner))
				return;

			GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
			Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
			Visuals();
		}

		// "Проверка активности" — следит, чтобы минион жил, пока жив игрок
		private bool CheckActive(Player owner) {
			if (owner.dead || !owner.active) {
				owner.ClearBuff(ModContent.BuffType<ExampleSimpleMinionBuff>());
				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<ExampleSimpleMinionBuff>())) {
				Projectile.timeLeft = 2;
			}
			return true;
		}

		private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition) {
			Vector2 idlePosition = owner.Center;
			idlePosition.Y -= 48f; // Поднимаемся на 48 пикселей (3 тайла) над центром игрока

			// Если минион не должен бесцельно летать, когда бездельничает, размещаем его в шеренге других минионов
			float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
			idlePosition.X += minionPositionOffsetX; // Встаём позади игрока

			// Код ниже адаптирован из Spazmamini (ID 388, aiStyle 66)

			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			// Телепортируемся к игроку, если слишком далеко
			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f) {
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}

			// Исправление наложения друг на друга (для летающих минионов)
			float overlapVelocity = 0.04f;
			foreach (var other in Main.ActiveProjectiles) {
				if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner &&
					Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width) {
					if (Projectile.position.X < other.position.X)
						Projectile.velocity.X -= overlapVelocity;
					else
						Projectile.velocity.X += overlapVelocity;

					if (Projectile.position.Y < other.position.Y)
						Projectile.velocity.Y -= overlapVelocity;
					else
						Projectile.velocity.Y += overlapVelocity;
				}
			}
		}

		private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter) {
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;

			// Приоритет цели, выбранной игроком правой кнопкой
			if (owner.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);

				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget) {
				foreach (var npc in Main.ActiveNPCs) {
					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
						bool closeThroughWall = between < 100f;

						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall)) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}

			// friendly = true — наносит контактный урон врагам
			// friendly = false — не ломает манекены и т.д. в режиме ожидания
			Projectile.friendly = foundTarget;
		}

		private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition) {
			float speed = 8f;
			float inertia = 20f;

			if (foundTarget) {
				// Есть цель — атакуем
				if (distanceFromTarget > 40f) {
					Vector2 direction = targetCenter - Projectile.Center;
					direction.Normalize();
					direction *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
				}
			}
			else {
				// Нет цели — возвращаемся к игроку
				if (distanceToIdlePosition > 600f) {
					speed = 12f;
					inertia = 60f;
				}
				else {
					speed = 4f;
					inertia = 80f;
				}

				if (distanceToIdlePosition > 20f) {
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
				}
				else if (Projectile.velocity == Vector2.Zero) {
					// Если совсем не двигается — лёгкий толчок
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
			}
		}

		private void Visuals() {
			// Лёгкий наклон в сторону движения
			Projectile.rotation = Projectile.velocity.X * 0.05f;

			// Простая анимация по кадрам сверху вниз
			int frameSpeed = 5;
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= frameSpeed) {
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame >= Main.projFrames[Type]) {
					Projectile.frame = 0;
				}
			}

			// Свечение
			Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
		}
	}
}
