using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;

namespace Client
{
    public class Main : BaseScript
    {
        private int bodyguardHandle = 0;
        private bool isBodyguardActive = false;

        public Main()
        {
            API.RegisterCommand("bg_create", new Action(CreateBodyguard), false);
            API.RegisterCommand("bg_remove", new Action(RemoveBodyguard), false);
        }

        private async void CreateBodyguard()
        {
            if (!isBodyguardActive)
            {
                Ped player = Game.Player.Character;
                API.RequestModel((uint)PedHash.ChemSec01SMM);
                while (!API.HasModelLoaded((uint)PedHash.ChemSec01SMM))
                {
                    Debug.WriteLine("Waiting for model to load");
                    await BaseScript.Delay(100);
                }
                Ped bodyguard = await World.CreatePed(PedHash.ChemSec01SMM, player.Position + (player.ForwardVector * 2));
                if (bodyguard != null && bodyguard.Exists())
                {
                    bodyguard.Task.LookAt(player);
                    API.SetPedAsGroupMember(bodyguard.Handle, API.GetPedGroupIndex(player.Handle));
                    API.SetPedCombatAbility(bodyguard.Handle, 2);
                    API.GiveWeaponToPed(bodyguard.Handle, (uint)WeaponHash.AssaultRifleMk2, 500, false, true);
                    bodyguard.PlayAmbientSpeech("GENERIC_HI");
                    bodyguardHandle = bodyguard.Handle;
                    isBodyguardActive = true;
                }
            } else
            {
                API.SendNuiMessage("A bodyguard is already active!");
            }
        }

        private async void RemoveBodyguard()
        {
            if (isBodyguardActive)
            {
                await BaseScript.Delay(0);
                API.DeletePed(ref bodyguardHandle);
                bodyguardHandle = 0;
                isBodyguardActive = false;
                Debug.WriteLine("The bodyguard has been removed!");
            } else
            {
                Debug.WriteLine("There is no active bodyguard to remove.");
            }
        }
    }
}