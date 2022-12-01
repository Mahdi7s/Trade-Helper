using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeHelper.Shared.Models;
using System.Data.SQLite;

namespace TradeHelper.Server.Database
{
    public class CryptoDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var conn = new SQLiteConnection(@"DataSource=crypto_database.db; Version=3; Mode=ReadWriteCreate;");
            optionsBuilder.UseSqlite(@"DataSource=crypto_database.db; Mode=ReadWriteCreate;");
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
            
        //}

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetBuy> AssetBuys { get; set; }
        public DbSet<AssetSell> AssetSells { get; set; }
    }
}
