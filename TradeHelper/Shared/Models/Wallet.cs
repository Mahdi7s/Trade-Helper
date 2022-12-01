using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeHelper.Shared.Models
{
    public class Wallet // for performance its better to have one wallet at all |
                        // Dashboard (show pie charts & amounts) & Wallet (Deposit & Withdrawal USDT)
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Default Wallet";
        public float TotalUSDT { get; set; } // Deposit & Withdrawal
        [NotMapped]
        public float USDTForTrade => TotalUSDT - (Assets?.Sum(x => x.AvailableQty * x.BuyAvg) ?? 0);
        public virtual List<Asset> Assets { get; set; } = new List<Asset>();

        [NotMapped]
        public float AssetsUsdt => Assets.Sum(x => x.AvailableQty * x.CurrentPrice);
    }
}
