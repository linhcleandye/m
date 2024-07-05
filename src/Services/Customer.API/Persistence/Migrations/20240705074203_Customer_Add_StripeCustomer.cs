using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Customer_Add_StripeCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "Customers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "Customers");
        }
    }
}
