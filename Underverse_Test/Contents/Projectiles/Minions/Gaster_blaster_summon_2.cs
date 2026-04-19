using Underverse_Test.Contents.Items;
using Underverse_Test.Contents.Buffs.MinionBuff;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Underverse_Test.Contents.Common;


namespace Underverse_Test.Contents.Projectiles.Minions
{
    public class Gaster_blaster_summon_2: ModProjectile
    {
        public bool FlagStage = true;
        public bool Fire = false;
        
        public override string Texture => "Underverse_Test/Contents/Assets/GasterBlasterProjectile";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;// кадры

            ProjectileID.Sets.MinionTargettingFeature[Type] = FlagStage;

            Main.projPet[Type] = true; // показывает на то что проджектайл питомец или миньен
            ProjectileID.Sets.MinionSacrificable[Type] = true; //задает логику призыва миньена ( он заменяется при призыва других и тд)
            ProjectileID.Sets.CultistIsResistantTo[Type] = true; // не бьет культиста
        }

        public override void SetDefaults()
        {
            Projectile.damage = 10;
            Projectile.width = 50; // ширина
            Projectile.height = 50; //длни
            Projectile.tileCollide = false; // столконовение с блоками

            Projectile.friendly = true; //контакный урон
            Projectile.minion = true; //это миньен ?
            Projectile.DamageType = DamageClass.Summon; // класс урона
            Projectile.minionSlots = 1f;//сколько слотов занимает
            Projectile.penetrate = -1;//ломается ли от столкновения ? 
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!ChekActive(owner)) return;

            
            Targeting(owner, out bool foundTarget, out float distanceTarget, out Vector2 targetCenter);
            Movement(owner, out float pos, out Vector2 position, foundTarget, targetCenter);
            Atack(foundTarget, targetCenter);
            
        }
        private bool ChekActive(Player owner)
        {
            // базовое поведение при смерти игрока
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<GasterMinionBuff>());
                return false;
            }
            if (owner.HasBuff(ModContent.BuffType<GasterMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            return true;
        }
        private void Targeting(Player owner, out bool foundTarget, out float distanceTarget, out Vector2 targetCenter)
        {
            //---------------------- TARGETING --------------------------------//
            distanceTarget = 600f;
            targetCenter = Projectile.position;
            foundTarget = false;
            //приоретет на пкмную цель
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);
                if (between < 2000f)
                {
                    distanceTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }
            // если пкмной цели нет 
            if (!foundTarget)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center); // расстояние между нпси и миньеном
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between; // проверка на ближайшего 
                        bool inRange = between < distanceTarget; // проверка на радиус атаки 
                        if ((closest && inRange) || !foundTarget)
                        {
                            distanceTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
        }
        private void Movement(Player owner, out float pos, out Vector2 position, bool foundTarget, Vector2 targetCenter) 
        {
            pos = 100f;

            position = owner.Center;
            position.Y -= pos;
            position.X -= pos;
            Vector2 end = position;
            Projectile.position = end;
            if (foundTarget)
            {
                targetCenter.X -= pos;
                targetCenter.Y -= pos;
                end = targetCenter;
            }

            if (!Fire) Projectile.position = end;
        }
        private void Atack(bool foundTarget, Vector2 targetCenter) 
        { 
            //---------------ATACKKK------------------------//
            if (foundTarget)
            {
                
                if (Main.LocalPlayer.GetModPlayer<Debugger>().flagTest && Main.GameUpdateCount % 600 == 0)
                {
                    Main.NewText($"x = {Projectile.Center.X} y = {Projectile.Center.Y}");
                    Main.NewText($"x = {targetCenter.X} y = {targetCenter.Y}");
                }
                
                
                Vector2 rotateTarget = Projectile.Center - targetCenter;
                Projectile.rotation = rotateTarget.ToRotation();
                int frameSpeed = 20;
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= frameSpeed) // каждый 20 тик меняем frame
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame > 3)
                    {
                        Projectile.frame = 3;
                    }

                }
               
                
                if (Projectile.frame == 3 && !Fire)
                {
                    
                    Fire = true;
                    
                    float speed = 50f;
                    float inertia = 4f;
                    Vector2 direction = targetCenter - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;

                    Projectile.ai[0]++;
                    if (Projectile.ai[0] == 60)
                    {

                        Fire = false;
                        Projectile.ai[0] = 0;
                        Projectile.frameCounter = 0;
                        Projectile.frame = 0;
                    }



                }
               
            }

            

            



        }
    }
}
