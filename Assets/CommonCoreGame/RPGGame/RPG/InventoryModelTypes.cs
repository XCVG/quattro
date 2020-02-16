﻿using CommonCore.State;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonCore.RpgGame.Rpg
{
    public enum MoneyType
    {
        Gold
    }

    public enum AmmoType
    {
        NoAmmo, Para9, Acp45, Nato556, Nato762, Sa68, Shot12, Slug, Arrow, Bolt //game dependent, redo for A3
    }

    public enum AidType //are there even any other stats?
    {
        None, Health //TODO energy
    }

    public enum RestoreType
    {
        Add, Boost, //boost allows going over max, add does not
        Override //override replaces
    }

    public enum ItemFlag
    {
        WeaponTwoHanded, WeaponAutoReload, WeaponNoAmmoUse, WeaponHasADS, WeaponFullAuto, WeaponNoAlert, WeaponHasCharge, WeaponHasRecock, WeaponChargeHold, WeaponShake, WeaponUseCrosshair, WeaponCrosshairInADS, WeaponNoMovebob, WeaponProportionalMovement, WeaponIgnoreLevelledRate, WeaponUnscaledAnimations, WeaponUseFarShootPoint, WeaponNeverRandomize, MeleeWeaponUsePreciseCasting
    }

    //an actual inventory item that the player has
    [JsonObject(IsReference = true)]
    public class InventoryItemInstance
    {
        //public const int UnstackableQuantity = -1;

        public int Quantity { get; set; }
        public float Condition { get; set; } //it's here but basically unimplemented
        public bool Equipped { get; set; }
        [JsonProperty, JsonConverter(typeof(InstanceItemConverter))]
        public InventoryItemModel ItemModel { get; private set; }
        [JsonProperty]
        public Dictionary<string, object> ExtraData { get; private set; } = new Dictionary<string, object>();

        [JsonConstructor]
        internal InventoryItemInstance(InventoryItemModel model, float condition, int quantity, bool equipped)
        {
            ItemModel = model;
            Condition = condition;
            Equipped = equipped;
            Quantity = quantity;
        }

        public InventoryItemInstance(InventoryItemModel model)
        {
            ItemModel = model;
            Condition = model.MaxCondition;
            Equipped = false;
            Quantity = 1;
        }
    }

    public class InstanceItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(InventoryItemModel).IsAssignableFrom(objectType);
            //return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            JObject jsonObject = JObject.Load(reader);
            string modelName = jsonObject["$ItemModel"].Value<string>();
            var model = InventoryModel.GetModel(modelName);
            return model;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var item = value as InventoryItemModel;
            writer.WriteStartObject();
            writer.WritePropertyName("$ItemModel");
            writer.WriteValue(item.Name);
            writer.WriteEndObject();
        }
    }

    // class for invariant inventory defs
    public class InventoryItemDef
    {
        public string NiceName;
        public string Image;
        public string Description;

        public InventoryItemDef(string niceName, string image, string description)
        {
            NiceName = niceName;
            Image = image;
            Description = description;
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}\t{2}]", NiceName, Image, Description);
        }
    }

    //base class for invariant inventory items
    public abstract class InventoryItemModel
    {
        public readonly string Name;
        public readonly float Weight;
        public readonly float Value;
        public readonly float MaxCondition;
        public readonly bool Hidden;
        public readonly bool Essential;
        public readonly string WorldModel;
        public readonly string[] Flags;

        [JsonProperty]
        public Dictionary<string, object> ExtraData { get; private set; } = new Dictionary<string, object>();

        public bool Stackable { get; protected set; }

        public InventoryItemModel(string name, float weight, float value, float maxCondition, bool hidden, bool essential, string[] flags, string worldModel)
        {
            Name = name;
            Weight = weight;
            Value = value;
            MaxCondition = maxCondition;
            Hidden = hidden;
            Essential = essential;
            Stackable = false;
            WorldModel = worldModel;
            Flags = (flags == null || flags.Length == 0) ? new string[0] : flags;
        }

        public virtual string GetStatsString()
        {
            return Essential ? "Essential" : string.Empty;
        }

        public bool CheckFlag(ItemFlag flag)
        {
            return CheckFlag(flag.ToString());
        }

        public bool CheckFlag(string flag)
        {
            return Array.Exists(Flags, x => flag.Equals(x, StringComparison.OrdinalIgnoreCase));
        }


    }

    public class MiscItemModel : InventoryItemModel
    {
        public MiscItemModel(string name, float weight, float value, float maxCondition, bool unique, bool essential, string[] flags, string worldModel)
            : base(name, weight, value, maxCondition, unique, essential, flags, worldModel)
        {
        }
    }

    public abstract class WeaponItemModel : InventoryItemModel
    {
        public readonly float Damage;
        public readonly float DamagePierce;
        public readonly float DamageSpread;        
        public readonly float DamagePierceSpread;
        public readonly DamageType DType;
        public readonly WeaponSkillType SkillType;
        public readonly string ViewModel;
        public readonly string HitPuff;
        public readonly float LowerTime;
        public readonly float RaiseTime;

        public WeaponItemModel(string name, float weight, float value, float maxCondition, bool unique, bool essential, string[] flags,
            float damage, float damagePierce, float damageSpread, float damagePierceSpread,
            DamageType dType, WeaponSkillType skillType, string viewModel, string worldModel, string hitPuff, float lowerTime, float raiseTime)
            : base(name, weight, value, maxCondition, unique, essential, flags, worldModel)
        {
            Damage = damage;
            DamagePierce = damagePierce;
            DamageSpread = damageSpread;
            DamagePierceSpread = damagePierceSpread;
            DType = dType;
            ViewModel = viewModel;
            HitPuff = hitPuff;
            SkillType = skillType;
            LowerTime = lowerTime;
            RaiseTime = raiseTime;
        }
    }

    public class MeleeWeaponItemModel : WeaponItemModel
    {
        public readonly float Reach;
        public readonly float Rate;
        public readonly float EnergyCost;

        public MeleeWeaponItemModel(string name, float weight, float value, float maxCondition, bool unique, bool essential, string[] flags,
            float damage, float damagePierce, float damageSpread, float damagePierceSpread,
            float reach, float rate, float energyCost, DamageType dType, WeaponSkillType skillType,
            string viewModel, string worldModel, string hitPuff, float lowerTime, float raiseTime) 
            : base(name, weight, value, maxCondition, unique, essential, flags, damage, damagePierce, damageSpread, damagePierceSpread, dType, skillType, viewModel, worldModel, hitPuff, lowerTime, raiseTime)
        {
            Reach = reach;
            Rate = rate;
            EnergyCost = energyCost;
        }
    }

    public class RangedWeaponItemModel : WeaponItemModel
    {
        public readonly float ProjectileVelocity;

        public readonly RangeEnvelope Recoil;
        public readonly RangeEnvelope Spread;
        public readonly RangeEnvelope ADSRecoil;
        public readonly RangeEnvelope ADSSpread;
        public readonly PulseEnvelope RecoilImpulse;
        public readonly PulseEnvelope ADSRecoilImpulse;

        public readonly float MovementSpreadFactor;
        public readonly float MovementRecoveryFactor;
        public readonly float CrouchSpreadFactor;
        public readonly float CrouchRecoveryFactor;

        public readonly float FireInterval;
        public readonly int NumProjectiles;
        public readonly int MagazineSize;       
        public readonly float ReloadTime;

        public readonly float ADSZoomFactor;

        public readonly AmmoType AType; 
        public readonly string Projectile;

        //it looks like JSON.net is actually using these constructors and the naming of the parameters matters, which is somewhat terrifying
        public RangedWeaponItemModel(string name, float weight, float value, float maxCondition, bool unique, bool essential, string[] flags,
            float damage, float damagePierce, float damageSpread, float damagePierceSpread, float projectileVelocity,
            RangeEnvelope recoil, RangeEnvelope spread, RangeEnvelope adsRecoil, RangeEnvelope adsSpread,
            PulseEnvelope recoilImpulse, PulseEnvelope adsRecoilImpulse,
            float movementSpreadFactor, float movementRecoveryFactor, float crouchSpreadFactor, float crouchRecoveryFactor,
            float fireInterval, int numProjectiles, int magazineSize, float reloadTime,
            AmmoType aType, DamageType dType, WeaponSkillType skillType, string viewModel, string worldModel, string hitPuff, string projectile, float adsZoomFactor, float lowerTime, float raiseTime)
            : base(name, weight, value, maxCondition, unique, essential, flags, damage, damagePierce, damageSpread, damagePierceSpread, dType, skillType, viewModel, worldModel, hitPuff, lowerTime, raiseTime)
        {
            ProjectileVelocity = projectileVelocity;

            Recoil = recoil;
            Spread = spread;
            ADSRecoil = adsRecoil;
            ADSSpread = adsSpread;
            RecoilImpulse = recoilImpulse;
            ADSRecoilImpulse = adsRecoilImpulse;

            MovementSpreadFactor = movementSpreadFactor;
            MovementRecoveryFactor = movementRecoveryFactor;
            CrouchSpreadFactor = crouchSpreadFactor;
            CrouchRecoveryFactor = crouchRecoveryFactor;

            FireInterval = fireInterval;
            NumProjectiles = numProjectiles;

            MagazineSize = magazineSize;
            ReloadTime = reloadTime;

            ADSZoomFactor = adsZoomFactor;

            AType = aType;
            Projectile = projectile;            
        }

        public bool UseMagazine => MagazineSize > 0;

        public override string GetStatsString()
        {
            StringBuilder str = new StringBuilder(255);
            //TODO finish impl

            return str.ToString() + base.GetStatsString();
        }
    }

    public class ArmorItemModel : InventoryItemModel
    {
        public readonly Dictionary<DamageType, float> DamageResistance;
        public readonly Dictionary<DamageType, float> DamageThreshold;
        public readonly EquipSlot Slot;

        public ArmorItemModel(string name, float weight, float value, float maxCondition, bool unique, bool essential, string[] flags, string worldModel,
            Dictionary<DamageType, float> damageResistance, Dictionary<DamageType, float> damageThreshold, EquipSlot slot)
            : base(name, weight, value, maxCondition, unique, essential, flags, worldModel)
        {
            DamageResistance = new Dictionary<DamageType, float>(damageResistance);
            DamageThreshold = new Dictionary<DamageType, float>(damageThreshold);
            Slot = slot;
        }
    }

    public class AidItemModel : InventoryItemModel
    {
        public readonly AidType AType;
        public readonly RestoreType RType;
        public float Amount;

        public AidItemModel(string name, float weight, float value, float maxCondition, bool unique, bool essential, string[] flags, string worldModel,
            AidType aType, RestoreType rType, float amount)
            : base(name, weight, value, maxCondition, unique, essential, flags, worldModel)
        {
            AType = aType;
            RType = rType;
            Amount = amount;
        }

        public void Apply()
        {
            Apply(this, GameState.Instance.PlayerRpgState);
        }

        public static void Apply(AidItemModel item, CharacterModel player)
        {
            switch (item.AType)
            {
                case AidType.Health:
                    {
                        switch (item.RType)
                        {
                            case RestoreType.Add:
                                player.Health = Math.Min(player.Health + item.Amount, player.DerivedStats.MaxHealth);
                                break;
                            case RestoreType.Boost:
                                player.Health += item.Amount;
                                break;
                            case RestoreType.Override:
                                player.Health = item.Amount;
                                break;
                            default:
                                break;
                        }
                    }
                    break;                
            }
        }
    }

    public class MoneyItemModel : InventoryItemModel
    {
        public readonly MoneyType Type;

        public MoneyItemModel(string name, float weight, float value, float maxCondition, bool unique, bool essential, string[] flags, string worldModel, MoneyType type) :
            base(name, weight, value, maxCondition, unique, essential, flags, worldModel)
        {
            Type = type;
            Stackable = true;
        }
    }

    public class AmmoItemModel : InventoryItemModel
    {
        public readonly AmmoType Type;

        public AmmoItemModel(string name, float weight, float value, float maxCondition, bool unique, bool essential, string[] flags, string worldModel, AmmoType type) :
            base(name, weight, value, maxCondition, unique, essential, flags, worldModel)
        {
            Type = type;
            Stackable = true;
        }
    }
}
