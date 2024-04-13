using System;
using System.Collections.Generic;
using HarmonyLib;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace AridBrindasCooldown
{
    [HarmonyPatch]
    public class BrindasVehicleCooldown : RocketPlugin<BrindasVehicleCooldownConfiguration>
    {
        private static Dictionary<CSteamID, double> _nextSaleTime = new Dictionary<CSteamID, double>();
        private static BrindasVehicleCooldownConfiguration configurationInstance;

        protected override void Load()
        {
            configurationInstance = Configuration.Instance;
            Harmony harmony = new Harmony("BrindasCooldown");
            harmony.PatchAll();
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerQuests), nameof(PlayerQuests.ReceiveBuyFromVendor))]
        static public bool ReceiveBuyFromVendor(
            in ServerInvocationContext context,
            Guid assetGuid,
            byte index,
            bool asManyAsPossible,
            PlayerQuests __instance)
        {
            //Brinda checkNPC.name: PS_Trader_12
            if (!__instance.checkNPC.name.Equals("PS_Trader_12")) //ignore all other NPCs
                return true; 
        
            var time = Time.realtimeSinceStartupAsDouble;
            var player = UnturnedPlayer.FromPlayer(__instance.player);
            if (!_nextSaleTime.ContainsKey(player.CSteamID) || _nextSaleTime[player.CSteamID]  < time)
            {
                _nextSaleTime[player.CSteamID] = time + configurationInstance.DefaultTimeoutInSeconds;
                return true;//allow purchase
            }
           
            UnturnedChat.Say(player,
                $"You must wait {(int)(_nextSaleTime[player.CSteamID] - time)}" +
                " seconds to buy another vehicle.",
                Color.red);
            
            return false;//stop purchase
        }
    }
}