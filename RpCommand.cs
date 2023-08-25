using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RegionalPVP
{
    public class RpCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "regionalpvp";

        public string Help => "RegionalPVP Main Command";

        public string Syntax => "/regionalpvp <create(c)/list(l)/here(h)/delete(d)> [Args...]";

        public List<string> Aliases => new List<string>() { "regp" };

        public List<string> Permissions => new List<string>() { "regionalpvp" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, RP.Localize(RP.COMMAND_USAGE));
                return;
            }

            string subCommand = command[0].ToLower();
            switch (subCommand)
            {
                case "c":
                case "create":
                    ExecuteCreate(caller, command.Skip(1).ToArray());
                    break;
                case "l":
                case "list":
                    ExecuteList(caller, command.Skip(1).ToArray());
                    break;
                case "h":
                case "here":
                    ExecuteHere(caller, command.Skip(1).ToArray());
                    break;
                case "d":
                case "delete":
                    ExecuteDelete(caller, command.Skip(1).ToArray());
                    break;
                default:
                    UnturnedChat.Say(caller, RP.Localize(RP.UNKNOWN_SUBCMD, subCommand), Color.red);
                    break;
            }
        }

        private void ExecuteCreate(IRocketPlayer caller, string[] command)
        {
            string name;
            float radius;
            Vector3 centerPos;

            if (caller is UnturnedPlayer player)
            {
                if (command.Length < 1)
                {
                    UnturnedChat.Say(caller, RP.Localize(RP.CREATE_USAGE_PLAYER), Color.red);
                    return;
                }

                name = command[0];
                radius = float.Parse(command[1]);
                centerPos = player.Position;
                if (command.Length >= 4)
                {
                    centerPos = new Vector3
                    {
                        x = float.Parse(command[2]),
                        y = float.Parse(command[3]),
                        z = float.Parse(command[4])
                    };
                }
            }
            else
            {
                if (command.Length < 4)
                {
                    UnturnedChat.Say(caller, RP.Localize(RP.CREATE_USAGE_CONSOLE), Color.red);
                    return;
                }

                name = command[0];
                radius = float.Parse(command[1]);
                centerPos = new Vector3
                {
                    x = float.Parse(command[2]),
                    y = float.Parse(command[3]),
                    z = float.Parse(command[4])
                };
            }

            CircleRegion region = new CircleRegion(centerPos, radius);
            RP.RegionPolicies.Add(new RegionPolicy(name, region));
            RP.SayPolicy(caller, RP.RegionPolicies.Count - 1);
        }

        private void ExecuteList(IRocketPlayer caller, string[] command)
        {
            for (int index = 0; index < RP.RegionPolicies.Count; index++)
            {
                RP.SayPolicy(caller, index);
            }
        }

        private void ExecuteHere(IRocketPlayer caller, string[] command)
        {
            if (!(caller is UnturnedPlayer))
            {
                RP.MultiLineSay(caller, RP.Localize(RP.PLAYER_ONLY), Color.red);
                return;
            }

            UnturnedPlayer player = (UnturnedPlayer)caller;
            RP.SayPolicy(caller, RP.GetActivePolicyIndex(player.Position));
        }

        private void ExecuteDelete(IRocketPlayer caller, string[] command)
        {
            int deleteIndex;

            if (caller is UnturnedPlayer player)
            {
                if (command.Length > 0)
                {
                    string name = command[0];
                    deleteIndex = RP.RegionPolicies.FindIndex(x => x.Name == name);
                }
                else
                {
                    deleteIndex = RP.GetActivePolicyIndex(player.Position);
                }
            }
            else
            {
                if (command.Length == 0)
                {
                    UnturnedChat.Say(caller, RP.Localize(RP.DELETE_USAGE_CONSOLE), Color.red);
                    return;
                }
                string name = command[0];
                deleteIndex = RP.RegionPolicies.FindIndex(x => x.Name == name);
            }

            if (deleteIndex == -1)
            {
                UnturnedChat.Say(caller, RP.Localize(RP.REGION_NOT_AVAILBALE), Color.red);
                return;
            }

            RP.RegionPolicies.RemoveAt(deleteIndex);
            UnturnedChat.Say(caller, RP.Localize(RP.REGION_DELETED, deleteIndex));
        }
    }
}
