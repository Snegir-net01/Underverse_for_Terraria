using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Underverse_Test.Contents.Buffs.MinionBuff;
using Underverse_Test.Contents.Projectiles.Minions;

namespace Underverse_Test.Contents.Common
{
    internal class Debugger : ModPlayer
    {
        public bool flagTest = false;
        public static ModKeybind Debug;
        public override void Load()
        {
            Debug = KeybindLoader.RegisterKeybind(Mod, "Debug", "/");
        }

        public override void Unload()
        {
            Debug = null;
        }
        public override void PreUpdate()
        {
            if (Debug.JustPressed)
            {
                Main.NewText("Run Debugg");
                flagTest = true;
            }
        }
    }
}
