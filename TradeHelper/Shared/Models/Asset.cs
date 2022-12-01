using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeHelper.Shared.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        [NotMapped]
        public string Title { get; set; }
        public virtual List<AssetBuy> AssetBuys { get; set; } = new List<AssetBuy>();
        public virtual List<AssetSell> AssetSells { get; set; } = new List<AssetSell>();

        private List<AssetBuy> _assetBuysX;
        [NotMapped]
        public List<AssetBuy> AssetBuysX => _assetBuysX ??= AssetBuys.Where(x => x.Bought).ToList();

        private List<AssetSell> _assetSellX;
        [NotMapped]
        public List<AssetSell> AssetSellsX => _assetSellX ??= AssetSells.Where(x => x.Sold).ToList();
        private float? _buyAvg = null;
        [NotMapped]
        public float BuyAvg => _buyAvg ??= ZeroNanOrInfinity(AssetBuysX.Sum(x => x.Quantity * x.Price) / BuysQty);
        private float? _sellAvg = null;
        [NotMapped]
        public float SellAvg => _sellAvg ??= ZeroNanOrInfinity(AssetSellsX.Sum(x => x.Quantity * x.Price) / SellsQty);
        private float? _buysQty = null;
        [NotMapped]
        public float BuysQty => _buysQty ??= AssetBuysX.Sum(x => x.Quantity);
        private float? _sellQty = null;
        [NotMapped]
        public float SellsQty => _sellQty ??= AssetSellsX.Sum(x => x.Quantity);
        [NotMapped]
        public float AvailableQty => BuysQty - SellsQty;
        private float? _invested = null;
        [NotMapped]
        public float Invested => _invested ??= ZeroNanOrInfinity(AssetBuysX.Sum(x => x.Quantity * x.Price));
        private float? _sold = null;
        [NotMapped]
        public float Sold => _sold ??= ZeroNanOrInfinity(AssetSellsX.Sum(x => x.Quantity * x.Price));
        // Sold Profit
        [NotMapped]
        public float Profit => Sold - AssetSellsX.Sum(x => x.Quantity * BuyAvg);
        // Sold Pnl
        [NotMapped]
        public float X => ZeroNanOrInfinity(SellAvg / BuyAvg); //ZeroNanOrInfinity(Profit == 0.0f ? 0.0f : Profit / AssetSellsX.Sum(x=> x.Quantity * BuyAvg) * 100);
        // Sold & Hold Pnl
        [NotMapped]
        public float CurrentX => ZeroNanOrInfinity((SellAvg > 0 ? (SellAvg + CurrentPrice) / 2 : CurrentPrice) / BuyAvg); //ZeroNanOrInfinity((Profit + (AvailableQty * CurrentPrice)) == 0.0f ? 0.0f : (Profit + (AvailableQty * CurrentPrice)) / Invested * 100);
        [NotMapped]
        public float CurrentProfit => ZeroNanOrInfinity(Invested * CurrentX) - Invested; //ZeroNanOrInfinity(Profit + (AvailableQty * CurrentPrice));
        public bool TradeCompleted => BuysQty > 0 && AvailableQty == 0;
        [NotMapped]
        public float CurrentPrice
        {
            get
            {
                if (_currentPrice == null)
                {
                    _currentPrice = AssetSells.OrderByDescending(x => x.Date).FirstOrDefault()?.Price ??
                        AssetBuys.OrderByDescending(x => x.Date).FirstOrDefault()?.Price;
                }

                return _currentPrice ?? 0.0f;
            }
            set { _currentPrice = value; }
        }
        private float? _currentPrice = null;
        [NotMapped]
        public DateTime LastModified => AssetBuysX.Select(x => x.Date).ToList().Concat(AssetSellsX.Select(x => x.Date)).OrderByDescending(x => x.Ticks).FirstOrDefault();

        public bool SetMarket(MarketStatsRoot marketStats)
        {
            if (AvailableQty > 0)
            {
                var stat = marketStats.data.FirstOrDefault(x => x.symbol?.Equals(Name, StringComparison.OrdinalIgnoreCase) ?? false);
                if (stat != null)
                {
                    _currentPrice = stat?.metrics?.market_data?.price_usd ?? 0.0f;
                    Title = stat?.slug;
                    return _currentPrice > 0;
                }
                return false;
            }
            return true;
        }

        private float ZeroNanOrInfinity(float val)
        {
            if (float.IsNaN(val) || float.IsInfinity(val)) return 0.0f;

            return val;
        }
    }
}
