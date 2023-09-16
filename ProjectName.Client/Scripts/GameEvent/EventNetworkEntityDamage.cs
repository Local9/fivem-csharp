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

                EntityInformation entityInfo = GetEntityInformation(victim, attacker);

                Logger.Debug($"---------------------------------------------.");
                Logger.Debug($"Event '{eventName}' was triggered.");
                Logger.Debug($"Victim: {victim?.Handle} {victim?.GetType()}");
                Logger.Debug($"Attacker: {attacker?.Handle} {attacker?.GetType()}");
                Logger.Debug($"IsDamageFatal: {isDamageFatal}");
                Logger.Debug($"WeaponInfoHash: {weaponInfoHash}");
                Logger.Debug($"IsMeleeDamage: {isMeleeDamage}");
                Logger.Debug($"DamageTypeFlag: {damageTypeFlag}");
                Logger.Debug($"---------------------------------------------.");

                if (isDamageFatal)
                {
                    bool isPlayerVictim = entityInfo.PlayerVictim is not null;
                    bool isPlayerAttacker = entityInfo.PlayerAttacker is not null;
                    bool isPedVictim = entityInfo.PedVictim is not null;
                    bool isPedAttacker = entityInfo.PedAttacker is not null;

                    if (isPlayerVictim && entityInfo.PedAttacker is null)
                    {
                        Logger.Debug($"Player died");
                        OnDeath?.Invoke(entityInfo.PedVictim, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }
                    else if (isPlayerVictim && isPlayerAttacker)
                    {
                        Logger.Debug($"Player killed player");
                        OnPlayerKillPlayer?.Invoke(entityInfo.PlayerAttacker, entityInfo.PlayerVictim, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }
                    else if (isPlayerVictim && isPedAttacker)
                    {
                        Logger.Debug($"Ped killed player");
                        OnPedKillPlayer?.Invoke(entityInfo.PedAttacker, entityInfo.PlayerVictim, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }
                    else if (isPedVictim && isPlayerAttacker)
                    {
                        Logger.Debug($"Player killed ped");
                        OnPlayerKillPed?.Invoke(entityInfo.PlayerAttacker, entityInfo.PedVictim, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }
                    else if (isPedVictim && isPedAttacker)
                    {
                        Logger.Debug($"Ped killed ped");
                        OnPedKillPed?.Invoke(entityInfo.PedAttacker, entityInfo.PedVictim, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }
                    else
                    {
                        Logger.Debug($"Entity killed entity");
                        OnEntityKillEntity?.Invoke(attacker, victim, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }
                }
                else
                {
                    if (entityInfo.VehicleVictim is not null)
                    {
                        Logger.Debug($"Vehicle damaged");
                        OnVehicleDamage?.Invoke(attacker, entityInfo.VehicleVictim, isMeleeDamage, weaponInfoHash, damageTypeFlag);
                    }

                    if (entityInfo.PlayerVictim is not null)
                    {
                        Logger.Debug($"Player damaged");
                        OnPlayerDamage?.Invoke(attacker, entityInfo.PlayerVictim, isMeleeDamage, weaponInfoHash, damageTypeFlag);
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

        private EntityInformation GetEntityInformation(Entity victim, Entity attacker)
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

            return new EntityInformation(pedVictim, pedAttacker, vehicleVictim, vehicleAttacker, playerVictim, playerAttacker);
        }
    }

    class EntityInformation
    {
        public Ped PedVictim = null;
        public Ped PedAttacker = null;
        public Vehicle VehicleVictim = null;
        public Vehicle VehicleAttacker = null;
        public Player PlayerVictim = null;
        public Player PlayerAttacker = null;

        public EntityInformation(Ped pedVictim, Ped pedAttacker, Vehicle vehicleVictim, Vehicle vehicleAttacker, Player playerVictim, Player playerAttacker)
        {
            this.PedVictim = pedVictim;
            this.PedAttacker = pedAttacker;
            this.VehicleVictim = vehicleVictim;
            this.VehicleAttacker = vehicleAttacker;
            this.PlayerVictim = playerVictim;
            this.PlayerAttacker = playerAttacker;
        }
    }
}
