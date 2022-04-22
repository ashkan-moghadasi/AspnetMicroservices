using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Coupon> GetDiscount(string productName)
        {
            var connection = GetConnection();
            var coupon=await  connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName=@ProductName",
                new {ProductName = productName});
            if (coupon == null) return new Coupon() {ProductName = productName, Amount = 0, Description = ""};
            return coupon;

        }

        private NpgsqlConnection GetConnection()
        {
            var connection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            return connection;
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            var connection = GetConnection();
            var affected=await connection.ExecuteAsync(
                "INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                new {ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount});
            return affected != 0 ;
            

        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var connection = GetConnection();
            var affected =
                await connection.ExecuteAsync(
                    "Update Coupon SET ProductName=@ProductName , Description=@Description , Amount=@Amount WHERE Id = @Id",
                    new {ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount , Id = coupon.Id });

            return affected != 0;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            var connection = GetConnection();
            var affected =
                await connection.ExecuteAsync(
                    "Delete From Coupon Where ProductName=@ProductName",
                    new { ProductName = productName });

            return affected != 0;
        }
    }
}
