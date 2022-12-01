using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TradeHelper.Shared.Models
{
    public class AssetSell
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public float Quantity { get; set; }
        public float Price { get; set; }
        public bool Sold { get; set; } = true;
        public DateTime Date { get; set; } = DateTime.Now;
        public string Exchange { get; set; }
        public string Comment { get; set; }
    }
}
