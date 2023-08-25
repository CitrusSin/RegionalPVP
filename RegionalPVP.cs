using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

namespace RegionalPVP
{
    public class RegionalPVP : RocketPlugin<RegionalPVPConfig>
    {
        private RegionalPVPConfig config;
        public RegionalPVPConfig Config => config;
        public List<RegionPolicy> RegionPolicies => config.RegionPolicies;

        private Dictionary<CSteamID, Vector3> lastPos = new Dictionary<CSteamID, Vector3>();

        public override TranslationList DefaultTranslations => new TranslationList {
            { RP.COMMAND_USAGE, "/regionalpvp <create(c)/list(l)/here(h)/delete(d)> [Args...]" },
            { RP.UNKNOWN_SUBCMD, "Unknown subcommand {0}." },
            { RP.CREATE_USAGE_PLAYER, "/regionalpvp <create/c> <name> <radius> [Center X] [Center Y] [Center Z]" },
            { RP.CREATE_USAGE_CONSOLE, "regionalpvp <create/c> <name> <radius> <Center X> <Center Y> <Center Z>" },
            { RP.DELETE_USAGE_CONSOLE, "regionalpvp <delete/d> <name>" },
            { RP.REGION_POLICY_TEMPLATE, "Region #{0}: \nName:   {1}\nCenter: {2}\nRadius: {3}\nPVP:   {4}" },
            { RP.PLAYER_ONLY, "This function is only available for players!" },
            { RP.REGION_NOT_AVAILBALE, "No matching region exists!" },
            { RP.REGION_DELETED, "Region #{0} deleted." },
            { RP.ENTER_REGION_NOPVP, "You're entering region \"{0}\" with PvP disabled!" },
            { RP.ENTER_REGION_PVP, "You're entering region \"{0}\" with PvP enabled!" },
            { RP.LEAVE_REGION, "You're leaving region \"{0}\"!" },
        };

        protected override void Load()
        {
            base.Configuration.Load();
            config = base.Configuration.Instance;
            DamageTool.onPlayerAllowedToDamagePlayer += OnPlayerAllowedToDamagePlayer;
            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerUpdatePosition += OnPlayerUpdatePosition;
        }

        protected override void Unload()
        {
            base.Configuration.Save();
        }

        public RegionPolicy GetActivePolicy(Vector3 position)
        {
            return RegionPolicies.Find(rp => rp.Region.IsInRegion(position));
        }

        public int GetActivePolicyIndex(Vector3 position)
        {
            return RegionPolicies.FindIndex(rp => rp.Region.IsInRegion(position));
        }

        public bool CanPvP(Vector3 position)
        {
            RegionPolicy policy = GetActivePolicy(position);
            if (policy != null)
            {
                return policy.IsPvP;
            }
            return Config.DefaultPvPMode;
        }

        private void OnPlayerAllowedToDamagePlayer(Player instigator, Player victim, ref bool allowed)
        {
            allowed = CanPvP(instigator.transform.position) && CanPvP(victim.transform.position);
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            lastPos[player.CSteamID] = player.Position;
            
            RegionPolicy policy = GetActivePolicy(player.Position);
            if (policy != null)
            {
                UnturnedChat.Say(player, Translate(RP.ENTER_REGION_PVP, policy.Name), Color.yellow);
            }
        }

        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            lastPos.Remove(player.CSteamID);
        }

        private void OnPlayerUpdatePosition(UnturnedPlayer player, Vector3 position)
        {
            int before = GetActivePolicyIndex(lastPos[player.CSteamID]);
            int after = GetActivePolicyIndex(position);
            if (before != after)
            {
                if (after == -1)
                {
                    var policy = RegionPolicies[before];
                    UnturnedChat.Say(player, Translate(RP.LEAVE_REGION, policy.Name), Color.yellow);
                }
                else
                {
                    var policy = RegionPolicies[after];
                    if (policy.IsPvP)
                    {
                        UnturnedChat.Say(player, Translate(RP.ENTER_REGION_PVP, policy.Name), Color.yellow);
                    }
                    else
                    {
                        UnturnedChat.Say(player, Translate(RP.ENTER_REGION_NOPVP, policy.Name), Color.yellow);
                    }
                }
            }
            lastPos[player.CSteamID] = position;
        }
    }
}
