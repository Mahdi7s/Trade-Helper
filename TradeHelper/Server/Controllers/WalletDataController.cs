using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeHelper.Server.Database;
using TradeHelper.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Newtonsoft.Json;

namespace TradeHelper.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalletDataController : ControllerBase
    {
        [HttpGet]
        [Route("Wallet")]
        public async Task<Wallet> Wallet()
        {
            using (var ctx = new CryptoDbContext())
            {
                var wallet = ctx.Wallets.Include(x => x.Assets).ThenInclude(x => x.AssetBuys)
                    .Include(x => x.Assets).ThenInclude(x => x.AssetSells).FirstOrDefault();

                if (wallet == null)
                {
                    wallet = new Wallet { Name = "Default Wallet" };
                    ctx.Add(wallet);
                    await ctx.SaveChangesAsync();
                }

                var marketStats = await GetMarketStats(1000);
                if (marketStats != null)
                {
                    foreach (var asset in wallet.Assets)
                    {
                        if (!asset.SetMarket(marketStats))
                        {
                            var marketAss = await GetMarketStat(asset.Name);
                            var price = (float)(marketAss?.data?.market_data?.price_usd ?? 0.0f);
                            if(price > 0) asset.CurrentPrice = price;
                            asset.Title = marketAss?.data?.name;
                        }
                    }
                }

                return await Task.FromResult(wallet);
            }
        }

        [HttpGet]
        [Route("Deposit")]
        public async Task<float> Deposit([FromQuery] float usdt)
        {
            if (usdt <= 0) return await Task.FromResult(-100);
            using (var ctx = new CryptoDbContext())
            {
                var wallet = ctx.Wallets.FirstOrDefault();
                if (wallet == null)
                {
                    wallet = new Wallet { TotalUSDT = usdt };
                    ctx.Wallets.Add(wallet);
                }
                else
                {
                    wallet.TotalUSDT += usdt;
                }
                await ctx.SaveChangesAsync();

                return await Task.FromResult(wallet.TotalUSDT);
            }
        }

        [HttpGet]
        [Route("Withdraw")]
        public async Task<float> Withdraw([FromQuery] float usdt)
        {
            if (usdt <= 0) return await Task.FromResult(-100);
            using (var ctx = new CryptoDbContext())
            {
                var wallet = ctx.Wallets.FirstOrDefault();
                if (wallet.USDTForTrade >= usdt)
                {
                    wallet.TotalUSDT -= usdt;
                    await ctx.SaveChangesAsync();
                }
                else
                {
                    return await Task.FromResult(-100);
                }

                return await Task.FromResult(wallet.TotalUSDT);
            }
        }

        [HttpPost]
        [Route("CreateAsset")]
        public async Task<IActionResult> CreateAsset(Asset asset)
        {
            using (var ctx = new CryptoDbContext())
            {
                var wallet = ctx.Wallets.Include(x => x.Assets).FirstOrDefault();
                if (!wallet.Assets.Any(x => x.Name.Equals(asset.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    wallet.Assets.Add(asset);
                    await ctx.SaveChangesAsync();

                    return Ok(asset);
                }

                return BadRequest();
            }
        }

        [HttpPost]
        [Route("SaveAsset")]
        public async Task<IActionResult> SaveAsset(Asset asset)
        {
            using (var ctx = new CryptoDbContext())
            {
                ctx.Update(asset);
                await ctx.SaveChangesAsync();

                return Ok(asset);
            }
        }

        [HttpPost]
        [Route("SaveAssetBuy")]
        public async Task<IActionResult> SaveAssetBuy(AssetBuy assetBuy)
        {
            using (var ctx = new CryptoDbContext())
            {
                assetBuy.Date = DateTime.Now;
                ctx.Update(assetBuy);
                await ctx.SaveChangesAsync();

                return Ok(assetBuy);
            }
        }


        [HttpPost]
        [Route("DelAsset")]
        public async Task<IActionResult> DelAsset(Asset asset)
        {
            using (var ctx = new CryptoDbContext())
            {
                ctx.Remove(asset);
                await ctx.SaveChangesAsync();

                return Ok(asset);
            }
        }

        [HttpPost]
        [Route("DelAssetBuy")]
        public async Task<IActionResult> DelAssetBuy(AssetBuy assetbuy)
        {
            using (var ctx = new CryptoDbContext())
            {
                ctx.Remove(assetbuy);
                await ctx.SaveChangesAsync();

                return Ok(assetbuy);
            }
        }

        [HttpPost]
        [Route("CreateBuyAsset")]
        public async Task<IActionResult> CreateBuyAsset(AssetBuy assetBuy)
        {
            using (var ctx = new CryptoDbContext())
            {
                var asset = ctx.Wallets.Include(x => x.Assets).FirstOrDefault().Assets.FirstOrDefault(x => x.Id == assetBuy.AssetId);
                asset.AssetBuys.Add(assetBuy);

                await ctx.SaveChangesAsync();

                return Ok(assetBuy);
            }
        }

        // ------------------------------------------------------------------

        [HttpPost]
        [Route("SaveAssetSell")]
        public async Task<IActionResult> SaveAssetSell(AssetSell assetSell)
        {
            try
            {
                using (var ctx = new CryptoDbContext())
                {
                    assetSell.Date = DateTime.Now;
                    ctx.Update(assetSell);
                    await ctx.SaveChangesAsync();

                    return Ok(assetSell);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("DelAssetSell")]
        public async Task<IActionResult> DelAssetSell(AssetSell assetSell)
        {
            using (var ctx = new CryptoDbContext())
            {
                ctx.Remove(assetSell);
                await ctx.SaveChangesAsync();

                return Ok(assetSell);
            }
        }

        [HttpPost]
        [Route("CreateSellAsset")]
        public async Task<IActionResult> CreateSellAsset(AssetSell assetSell)
        {
            using (var ctx = new CryptoDbContext())
            {
                var asset = ctx.Wallets.Include(x => x.Assets).FirstOrDefault().Assets.FirstOrDefault(x => x.Id == assetSell.AssetId);
                asset.AssetSells.Add(assetSell);

                await ctx.SaveChangesAsync();

                return Ok(assetSell);
            }
        }

        private async Task<MarketStatsRoot> GetMarketStats(int limit = 500)
        {
            var pages = limit / 500;
            MarketStatsRoot ret = null;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-messari-api-key", "aca8f858-92c2-44fd-961d-19ab349ec825");

                for (int i = 1; i <= pages; i++)
                {
                    var count = i == pages ? limit - ((i - 1) * 500) : 500;
                    using (var response = await httpClient.GetAsync($"https://data.messari.io/api/v2/assets?fields=id,slug,symbol,metrics/market_data/price_usd&limit={count}&page={i}"))
                    {
                        if (!response.IsSuccessStatusCode) break;

                        using (var content = response.Content)
                        {
                            var retval = JsonConvert.DeserializeObject<MarketStatsRoot>(await content.ReadAsStringAsync());
                            if (ret == null)
                            {
                                ret = retval;
                            }
                            else
                            {
                                if (retval?.data == null)
                                    break;
                                else
                                    ret.data.AddRange(retval.data);
                            }
                        }
                    }
                }

                return await Task.FromResult(ret);
            }
        }

        private async Task<SingleMarketStatRoot> GetMarketStat(string asset)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-messari-api-key", "aca8f858-92c2-44fd-961d-19ab349ec825");

                using (var response = await httpClient.GetAsync($"https://data.messari.io/api/v1/assets/{asset}/metrics/market-data"))
                {
                    if (!response.IsSuccessStatusCode) return null;

                    using (var content = response.Content)
                    {
                        var json = await content.ReadAsStringAsync();
                        try
                        {
                            var retval = JsonConvert.DeserializeObject<SingleMarketStatRoot>(json);
                            return await Task.FromResult(retval);
                        }
                        catch
                        {
                            return null;
                        }
                    }
                }
            }
        }
    }
}
