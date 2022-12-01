using System;
using System.Collections.Generic;
using System.Text;

namespace TradeHelper.Shared.Models
{
    public class Status2
    {
        public int elapsed { get; set; }
        public string timestamp { get; set; }
    }

    public class OhlcvLast1Hour
    {
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public double volume { get; set; }
    }

    public class OhlcvLast24Hour
    {
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public double volume { get; set; }
    }

    public class MarketData2
    {
        public double price_usd { get; set; }
        public double price_btc { get; set; }
        public double price_eth { get; set; }
        public double volume_last_24_hours { get; set; }
        public double real_volume_last_24_hours { get; set; }
        public double volume_last_24_hours_overstatement_multiple { get; set; }
        public double percent_change_usd_last_1_hour { get; set; }
        public double percent_change_btc_last_1_hour { get; set; }
        public double percent_change_eth_last_1_hour { get; set; }
        public double percent_change_usd_last_24_hours { get; set; }
        public double percent_change_btc_last_24_hours { get; set; }
        public double percent_change_eth_last_24_hours { get; set; }
        public OhlcvLast1Hour ohlcv_last_1_hour { get; set; }
        public OhlcvLast24Hour ohlcv_last_24_hour { get; set; }
        public DateTime last_trade_at { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public List<object> contract_addresses { get; set; }
        public string _internal_temp_agora_id { get; set; }
        public MarketData2 market_data { get; set; }
    }

    public class SingleMarketStatRoot
    {
        public Status2 status { get; set; }
        public Data data { get; set; }
    }
}
