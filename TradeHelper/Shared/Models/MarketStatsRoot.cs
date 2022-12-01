using System;
using System.Collections.Generic;
using System.Text;

namespace TradeHelper.Shared.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Status
    {
        public int elapsed { get; set; }
        public string timestamp { get; set; }
    }

    public class MarketData
    {
        public float? price_usd { get; set; }
    }

    public class Metrics
    {
        public MarketData market_data { get; set; }
    }

    public class Datum
    {
        public string id { get; set; }
        public string slug { get; set; }
        public string symbol { get; set; }
        public Metrics metrics { get; set; }
    }

    public class MarketStatsRoot
    {
        public Status status { get; set; }
        public List<Datum> data { get; set; }
    }
}
