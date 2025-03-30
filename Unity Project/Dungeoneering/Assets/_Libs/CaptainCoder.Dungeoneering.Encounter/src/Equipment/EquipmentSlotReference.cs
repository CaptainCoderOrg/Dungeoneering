namespace CaptainCoder.Dungeoneering.Encounter;
public abstract record class EquipmentSlotReference(HeroEntityData Hero)
{
    public event System.Action OnDataChanged
    {
        add => DataChanged += value;
        remove => DataChanged -= value;
    }
    protected System.Action DataChanged { get; private set; }
    public abstract EquipmentData Data { get; set; }
    public abstract bool CanHold(EquipmentData data, out string result);
    public bool TrySetEquipment(EquipmentData data, out string result)
    {
        if (Data != null)
        {
            result = "Slot was already occupied";
            return false;
        }
        if (!CanHold(data, out result)) { return false; }
        Data = data;
        return true;
    }
    public bool TrySwapEquipment(EquipmentSlotReference other, out string result)
    {
        if (!CanHold(other.Data, out result) || !other.CanHold(Data, out result))
        {
            return false;
        }
        (other.Data, Data) = (Data, other.Data);
        result = "Swapped";
        return true;
    }
}
public sealed record class BackpackSlotReference(HeroEntityData Hero, int SlotIndex) : EquipmentSlotReference(Hero)
{
    public override bool CanHold(EquipmentData data, out string result)
    {
        result = "Success";
        return true;
    }
    public override EquipmentData Data
    {
        get => Hero.Inventory.Count > SlotIndex ? Hero.Inventory[SlotIndex] : null;
        set
        {
            while (Hero.Inventory.Count <= SlotIndex) { Hero.Inventory.Add(null); }
            Hero.Inventory[SlotIndex] = value;
            DataChanged?.Invoke();
        }
    }

}

public sealed record class LeftHandSlotReference(HeroEntityData Hero) : EquipmentSlotReference(Hero)
{
    public override EquipmentData Data
    {
        get => Hero.LeftHand;
        set
        {
            if (Data is HeldEquipmentData previous && previous.IsTwoHanded)
            {
                Hero.RightHandSlot.SetTwoHanded(null);
            }
            HeldEquipmentData data = (HeldEquipmentData)value;
            Hero.LeftHand = data;
            if (data != null && data.IsTwoHanded)
            {
                Hero.RightHandSlot.SetTwoHanded(data);
            }
            DataChanged?.Invoke();
        }
    }

    public override bool CanHold(EquipmentData data, out string result)
    {
        if (data == null)
        {
            result = "Success";
            return true;
        }
        if (data is not HeldEquipmentData heldEquipmentData)
        {
            result = "Not a held item";
            return false;
        }
        if (heldEquipmentData.IsTwoHanded && Hero.RightHand != null)
        {
            result = "Requires two hands";
            return false;
        }
        result = "Success";
        return true;
    }

    internal void SetTwoHanded(HeldEquipmentData data)
    {
        Hero.LeftHand = data;
        DataChanged?.Invoke();
    }
}
public sealed record class RightHandSlotReference(HeroEntityData Hero) : EquipmentSlotReference(Hero)
{
    public override EquipmentData Data
    {
        get => Hero.RightHand;
        set
        {
            if (Data is HeldEquipmentData previous && previous.IsTwoHanded)
            {
                Hero.LeftHandSlot.SetTwoHanded(null);
            }
            HeldEquipmentData data = (HeldEquipmentData)value;
            Hero.RightHand = data;
            if (data != null && data.IsTwoHanded)
            {
                Hero.LeftHandSlot.SetTwoHanded(data);
            }
            DataChanged?.Invoke();
        }
    }

    public override bool CanHold(EquipmentData data, out string result)
    {
        if (data == null)
        {
            result = "Success";
            return true;
        }
        if (data is not HeldEquipmentData heldEquipmentData)
        {
            result = "Not a held item";
            return false;
        }
        if (heldEquipmentData.IsTwoHanded && Hero.LeftHand != null)
        {
            result = "Requires two hands";
            return false;
        }
        result = "Success";
        return true;
    }

    internal void SetTwoHanded(HeldEquipmentData data)
    {
        Hero.RightHand = data;
        DataChanged?.Invoke();
    }
}
public sealed record class WornEquipmentSlotReference(HeroEntityData Hero) : EquipmentSlotReference(Hero)
{
    public override EquipmentData Data
    {
        get => Hero.WornArmor;
        set
        {
            Hero.WornArmor = (WornEquipmentData)value;
            DataChanged?.Invoke();
        }
    }

    public override bool CanHold(EquipmentData data, out string result)
    {
        if (data == null)
        {
            result = "Success";
            return true;
        }
        if (data is not WornEquipmentData)
        {
            result = "Not a worn item";
            return false;
        }
        result = "Success";
        return true;
    }
}
public sealed record class AccessorySlotReference(HeroEntityData Hero) : EquipmentSlotReference(Hero)
{
    public override EquipmentData Data
    {
        get => Hero.Accessory;
        set
        {
            Hero.Accessory = (AccessoryEquipmentData)value;
            DataChanged?.Invoke();
        }
    }

    public override bool CanHold(EquipmentData data, out string result)
    {
        if (data == null)
        {
            result = "Success";
            return true;
        }
        if (data is not AccessoryEquipmentData)
        {
            result = "Not an accessory item";
            return false;
        }
        result = "Success";
        return true;
    }
}