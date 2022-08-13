using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _connectionString = _configuration.GetValue<string>("DatabaseSettings:ConnectionString");
        }
        public async Task<Coupon> CreateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            coupon.Id = await connection.ExecuteScalarAsync<int>("INSERT INTO Coupon(productName, description, amount) VALUES(@ProductName,@Description,@Amount) RETURNING id", new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount});
            return coupon;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var result = await connection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = :ProductName", new { ProductName = productName });
            return result > 0;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName = :ProductName", new { ProductName = productName});
            if(coupon == null){
                return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Description"};
            }
            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var result = await connection.ExecuteAsync("UPDATE Coupon SET Description = @Description, Amount = @Amount WHERE ProductName = @ProductName", new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount});
            return result > 0;
        }
    }
}