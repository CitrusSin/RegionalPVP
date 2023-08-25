using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RegionalPVP
{
    // The static tool class
    internal sealed class RP
    {
        private RP() { }

        public static RegionalPVP Inst => (RegionalPVP) R.Plugins.GetPlugin(Assembly.GetExecutingAssembly());
        public static List<RegionPolicy> RegionPolicies => Inst.RegionPolicies;

        public const string COMMAND_USAGE = "command_usage";
        public const string UNKNOWN_SUBCMD = "unknown_subcmd";
        public const string CREATE_USAGE_PLAYER = "create_usage_player";
        public const string CREATE_USAGE_CONSOLE = "create_usage_console";
        public const string DELETE_USAGE_CONSOLE = "delete_usage_console";
        public const string REGION_POLICY_TEMPLATE = "region_policy_template";
        public const string PLAYER_ONLY = "player_only";
        public const string REGION_NOT_AVAILBALE = "region_not_available";
        public const string REGION_DELETED = "region_deleted";
        public const string ENTER_REGION_NOPVP = "enter_region_nopvp";
        public const string ENTER_REGION_PVP = "enter_region_pvp";
        public const string LEAVE_REGION = "leave_region";

        public static string Localize(string key, params object[] args)
        {
            return Inst.Translate(key, args);
        }

        public static RegionPolicy GetActivePolicy(Vector3 position)
        {
            return Inst.GetActivePolicy(position);
        }

        public static int GetActivePolicyIndex(Vector3 position)
        {
            return Inst.GetActivePolicyIndex(position);
        }

        public static bool CanPvP(Vector3 position)
        {
            return Inst.CanPvP(position);
        }

        public static void MultiLineSay(IRocketPlayer caller, string message)
        {
            if (caller is UnturnedPlayer)
            {
                MultiLineSay(caller, message, Color.green);
            }
            else
            {
                MultiLineSay(caller, message, Color.white);
            }
        }

        public static void MultiLineSay(IRocketPlayer caller, string message, Color color)
        {
            if (caller is UnturnedPlayer)
            {
                string[] lines = message.Split('\n');
                for (int i = 0; i < lines.Length; i += 2)
                {
                    string msg = lines[i];
                    if (i + 1 < lines.Length)
                    {
                        msg += "\n" + lines[i + 1];
                    }
                    UnturnedChat.Say(caller, msg, color);
                }
            }
            else
            {
                string[] lines = message.Split('\n');
                foreach (string s in lines)
                {
                    UnturnedChat.Say(caller, s, color);
                }
            }
        }

        public static void SayPolicy(IRocketPlayer target, int index)
        {
            if (index == -1)
            {
                UnturnedChat.Say(target, Localize(REGION_NOT_AVAILBALE), Color.red);
                return;
            }

            RegionPolicy policy = RegionPolicies[index];
            MultiLineSay(target, Localize(
                REGION_POLICY_TEMPLATE, 
                index, 
                policy.Name, 
                policy.Region.Center, 
                policy.Region.Radius, 
                policy.IsPvP ? "Enabled" : "Disabled"
            ));
        }
    }
}
