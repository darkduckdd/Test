public class CorpsmanModel : EntityModel
{
    public float Speed;
    public int TransferRate;
    public CorpsmanLvlConfig[] Lvls;
    private ItemController item = null;

    public ItemController Item => item;

    public CorpsmanModel(string id, int lvl, EntityType type, CorpsmanConfig config) : base(id, lvl, type)
    {
        PricePurchase = config.PricePurchase;
        Lvls = config.Lvls;
        TargetPurchaseValue = config.TargetPurchaseProgress;
        Area = config.Area;
        TransferRate = config.TransferRate;

        UpdateModel();
    }

    public override void GetUpdatedValues()
    {
        TargetUpdateProgress = Lvls[LvlNext].TargetUpdateProgress;
        PriceNextLvl = Lvls[LvlNext].Price;
        Speed = Lvls[Lvl].Speed;
    }

    public override int GetLvlLength()
    {
        return Lvls.Length;
    }

    public void SetItem(ItemController item)
    {
        this.item = item;
    }
    public void RemoveItem()
    {
        item = null;
    }
}
