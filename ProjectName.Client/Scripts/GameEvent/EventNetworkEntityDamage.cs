using ProjectName.Client.Scripts.GameEvent.Events;
using ProjectName.Shared;
using System.Collections.Generic;

// Query: Could this be moved to the shared project so both the server and the client can use them?
// Investigate: Would there be issues? and what are they?
namespace ProjectName.Client.Scripts.GameEvent.Events
{
    internal delegate void PlayerKillPlayerEvent(Player attacker, Player victim, bool isMeleeDamage, uint weaponInfoHash, int damageTypeFlag);
    internal delegate void PlayerKillPedEvent(Player attacker, Ped victim, bool isMeleeDamage, uint weaponInfoHash, int damageTypeFlag);
    internal delegate void PedKillPlayerEvent(Ped attacker, Player victim, bool isMeleeDamage, uint weaponInfoHash, int damageTypeFlag);
    internal delegate void PedKillPedEvent(Ped attacker, Ped victim, bool isMeleeDamage, uint weaponInfoHash, int damageTypeFlag);
    internal delegate void EntityKillEntityEvent(Entity attacker, Entity victim, bool isMeleeDamage, uint weaponInfoHash, int damageTypeFlag);
    internal delegate void DeadEvent(Entity attacker, bool isMeleeDamage, uint weaponHashInfo, int damageTypeFlag);
    internal delegate void EntityDamageEvent(Entity attacker, Entity victim, bool isMeleeDamage, uint weaponInfoHash, int damageTypeFlag);
    internal delegate void VehicleDamageEvent(Entity attacker, Entity victim, bool isMeleeDamage, uint weaponInfoHash, int damageTypeFlag);
    internal delegate void PlayerDamageEvent(Entity attacker, Player victim, bool isMeleeDamage, uint weaponInfoHash, int damageTypeFlag);
}

namespace ProjectName.Client.Scripts.GameEvent
{

    internal class EventNetworkEntityDamage : ScriptBase
    {
        private static readonly object _padlock = new();
        private static EventNetworkEntityDamage _instance;

        public event PlayerKillPlayerEvent OnPlayerKillPlayer;
        public event PlayerKillPedEvent OnPlayerKillPed;
        public event PedKillPlayerEvent OnPedKillPlayer;
        public event PedKillPedEvent OnPedKillPed;
        public event EntityKillEntityEvent OnEntityKillEntity;
        public event DeadEvent OnDeath;
        public event EntityDamageEvent OnEntityDamage;
        public event VehicleDamageEvent OnVehicleDamage;
        public event PlayerDamageEvent OnPlayerDamage;

        private const string EVENT_NAME = "CEventNetworkEntityDamage";

        private EventNetworkEntityDamage()
        {
            AttachEvent("gameEventTriggered", new Action<string, List<dynamic>>(OnGameEventTriggered));
        }

        internal static EventNetworkEntityDamage Instance
        {
            get
            {
                lock (_padlock)
                {
                    return _instance ??= new EventNetworkEntityDamage();
                }
            }
        }

        private void OnGameEventTriggered(string eventName, List<dynamic> args)
        {
            if (eventName != EVENT_NAME)
                return;

            try
            {
                /// <param name="victim">victim</param>
                /// <param name="attacker">attacker</param>
                /// <param name="arg2">Unknown</param>
                /// <param name="arg3">Unknown</param>
                /// <param name="arg4">Unknown</param>
                /// <param name="isDamageFatal">Is damage fatal to entity. or victim died/destroyed.</param>
                /// <param name="weaponInfoHash">Probably related to common.rpf/data/ai => Item type = "CWeaponInfo"</param>
                /// <param name="arg7">Unknown</param>
                /// <param name="arg8">Unknown</param>
                /// <param name="arg9">Unknown, might be int</param>
                /// <param name="arg10">Unknown, might be int</param>
                /// <param name="isMeleeDamage">Is melee damage</param>
                /// <param name="damageTypeFlag">0 for peds, 116 for the body of a vehicle, 93 for a tire, 120 for a side window, 121 for a rear window, 122 for a windscreen, etc</param>
                Entity victim = Entity.FromHandle((int)args[0]);
                Entity attacker = Entity.FromHandle((int)args[1]);
                bool isDamageFatal = Convert.ToBoolean((int)args[5]);
                uint weaponInfoHash = (uint)args[6];
                bool isMeleeDamage = Convert.ToBoolean((int)args[11]);
                int damageTypeFlag = (int)args[12];

                (Ped victimPed,
                    Ped attackerPed,
                    Vehicle victimVehicle,
                    Vehicle attackerVehicle,
                    Player victimPlayer,
                    Player attackerPlayer) = GetEntityInformation(victim, attacker);

                if (isDamageFatal)
                {
                    // Player killed a player
                    if (victimPlayer is not null && attackerPlayer is not null)
                    {
                        OnPlayerKillPlayer.Invoke(attackerPlayer, victimPlayer, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }
                    // Player killed a ped
                    else if (victimPlayer is null && attackerPlayer is not null)
                    {
                        OnPlayerKillPed.Invoke(attackerPlayer, victimPed, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }
                    // Ped killed a player
                    else if (victimPlayer is not null && attackerPlayer is null)
                    {
                        OnPedKillPlayer.Invoke(attackerPed, victimPlayer, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }
                    // Ped killed a ped
                    else if (attackerPlayer is null && victimPlayer is null)
                    {
                        OnPedKillPed.Invoke(attackerPed, victimPed, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }
                    else
                    {
                        OnEntityKillEntity.Invoke(attacker, victim, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }
                }
                else
                {
                    if (victimPlayer is not null)
                    {
                        OnPlayerDamage.Invoke(attacker, victimPlayer, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }
                    else if (victimVehicle is not null)
                    {
                        OnVehicleDamage.Invoke(attacker, victimVehicle, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"---------------------------------------------.");
                Logger.Error($"Failed to handle event '{eventName}'.");
                Logger.Error($"{ex}");
                Logger.Error($"---------------------------------------------.");
            }
        }

        private (Ped victimPed, Ped attackerPed, Vehicle victimVehicle, Vehicle attackerVehicle, Player victimPlayer, Player attackerPlayer) GetEntityInformation(Entity victim, Entity attacker)
        {
            Ped pedVictim = null;
            Ped pedAttacker = null;
            Vehicle vehicleVictim = null;
            Vehicle vehicleAttacker = null;
            Player playerVictim = null;
            Player playerAttacker = null;

            if (victim is Ped)
                pedVictim = victim as Ped;
            if (victim is Vehicle)
                vehicleVictim = victim as Vehicle;
            if (pedVictim is not null && pedVictim.IsPlayer)
                playerVictim = new Player(NetworkGetPlayerIndexFromPed(pedVictim.Handle));
            if (pedVictim is null && vehicleVictim is not null)
                playerVictim = new Player(NetworkGetPlayerIndexFromPed(vehicleVictim.Driver.Handle));

            if (attacker is Ped)
                pedAttacker = attacker as Ped;
            if (attacker is Vehicle)
                vehicleAttacker = attacker as Vehicle;
            if (pedAttacker is not null && pedAttacker.IsPlayer)
                playerAttacker = new Player(NetworkGetPlayerIndexFromPed(pedAttacker.Handle));
            if (pedAttacker is null && vehicleAttacker is not null)
                playerAttacker = new Player(NetworkGetPlayerIndexFromPed(vehicleAttacker.Driver.Handle));

            return (victimPed: pedVictim,
                    attackerPed: pedAttacker,
                    victimVehicle: vehicleVictim,
                    attackerVehicle: vehicleAttacker,
                    victimPlayer: playerVictim,
                    attackerPlayer: playerAttacker);
        }
    }
}
