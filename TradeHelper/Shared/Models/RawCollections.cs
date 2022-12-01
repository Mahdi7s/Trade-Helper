using System;
using System.Collections.Generic;
using System.Text;

namespace TradeHelper.Shared.Models
{
    public static class RawCollections
    {
        public static readonly List<string> AssetCategories = new List<string>
        {
            "Store of Value", "Smart Contracts", "Centralized Exchange", "DeFi", "NFTs", "Memes", 
        };

        public static readonly List<string> Exchange = new List<string>
        {
            "KuCoin", "OKex", "Binance", "Huobi", "Trust Wallet", "SafePal", "MetaMask"
        };
    }
}
