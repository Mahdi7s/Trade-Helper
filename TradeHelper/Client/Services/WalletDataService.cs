using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using TradeHelper.Shared.Models;
using System.Text.Json;

namespace TradeHelper.Client.Services
{
    public class WalletDataService
    {
        [Inject]
        private HttpClient Http { get; set; }

        public WalletDataService(HttpClient client)
        {
            Http = client;
        }

        public async Task<Wallet> GetWallet()
        {
            var val = await Http.GetFromJsonAsync<Wallet>("WalletData/Wallet");
            return val;
        }

        public async Task<float> Deposit(float usdt)
        {
            return await Http.GetFromJsonAsync<float>("WalletData/Deposit?usdt=" + usdt);
        }   
        
        public async Task<float> Withdraw(float usdt)
        {
            return await Http.GetFromJsonAsync<float>("WalletData/Withdraw?usdt=" + usdt);
        }

        public async Task<bool> CreateAsset(Asset asset)
        {
            var resp = await Http.PostAsJsonAsync("WalletData/CreateAsset", asset);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsset(Asset asset)
        {
            var resp = await Http.PostAsJsonAsync("WalletData/SaveAsset", asset);
            return resp.IsSuccessStatusCode;
        }       
        
        public async Task<bool> UpdateAssetBuy(AssetBuy assetBuy)
        {
            var resp = await Http.PostAsJsonAsync("WalletData/SaveAssetBuy", assetBuy);
            return resp.IsSuccessStatusCode;
        }     
        
        public async Task<bool> DelAssetBuy(AssetBuy assetBuy)
        {
            var resp = await Http.PostAsJsonAsync("WalletData/DelAssetBuy", assetBuy);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> CreateBuyAsset(AssetBuy assetBuy)
        {
            var resp = await Http.PostAsJsonAsync("WalletData/CreateBuyAsset", assetBuy);
            return resp.IsSuccessStatusCode;
        }

        // ---------------------------------------------------------------------------------

        public async Task<bool> UpdateAssetSell(AssetSell assetsell)
        {
            var resp = await Http.PostAsJsonAsync("WalletData/SaveAssetSell", assetsell);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DelAssetSell(AssetSell assetsell)
        {
            var resp = await Http.PostAsJsonAsync("WalletData/DelAssetSell", assetsell);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> CreateSellAsset(AssetSell assetsell)
        {
            var resp = await Http.PostAsJsonAsync("WalletData/CreateSellAsset", assetsell);
            return resp.IsSuccessStatusCode;
        }
    }
}
