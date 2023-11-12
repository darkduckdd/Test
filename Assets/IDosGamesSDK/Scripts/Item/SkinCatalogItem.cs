using PlayFab.ClientModels;

namespace IDosGames
{
	public class SkinCatalogItem : ItemFromCatalog
	{
		public string Collection => _collection;
		public string ImagePath => _imagePath;
		public float Profit => _profit;
		public RarityType Rarity => _rarity;

		private readonly string _collection;
		private readonly string _imagePath;
		private readonly float _profit;
		private readonly RarityType _rarity;

		public SkinCatalogItem(CatalogItem item, string collection, string imagePath, float profit, RarityType rarity) : base(item)
		{
			_collection = collection;
			_imagePath = imagePath;
			_profit = profit;
			_rarity = rarity;
		}
	}
}