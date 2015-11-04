

// This file was automatically generated.
// Do not make changes directly to this file - edit the template instead.
// 
// The following connection settings were used to generate this file
// 
//     Configuration file:     "ionixTests\App.config"
//     Connection String Name: "SqlConn"
//     Connection String:      "Data Source=.;Initial Catalog=NORTHWND;User Id=admin;password=**zapped**;"

// ReSharper disable RedundantUsingDirective
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable RedundantNameQualifier

using ionix.Data.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ionixTests.Models
{

    // ************************************************************************
    // POCO classes

	[Table("Alphabeticallistofproducts")]
    public partial class Alphabeticallistofproducts
    {
        [DbSchema(IsNullable = false, IsKey=true, Order = 1)]
		public int ProductID { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 40, Order = 2)]
		public string ProductName { get; set; }

        [DbSchema(IsNullable = true, Order = 3)]
		public int? SupplierID { get; set; }

        [DbSchema(IsNullable = true, Order = 4)]
		public int? CategoryID { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 20, Order = 5)]
		public string QuantityPerUnit { get; set; }

        [DbSchema(IsNullable = true, Order = 6)]
		public decimal? UnitPrice { get; set; }

        [DbSchema(IsNullable = true, Order = 7)]
		public short? UnitsInStock { get; set; }

        [DbSchema(IsNullable = true, Order = 8)]
		public short? UnitsOnOrder { get; set; }

        [DbSchema(IsNullable = true, Order = 9)]
		public short? ReorderLevel { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 10)]
		public bool Discontinued { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 15, Order = 11)]
		public string CategoryName { get; set; }

    }


	[Table("Categories")]
    public partial class Categories
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = ionix.Data.StoreGeneratedPattern.Identity, Order = 1)]
		public int CategoryID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 15, Order = 2)]
		public string CategoryName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 1073741823, Order = 3)]
		public string Description { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 2147483647, Order = 4)]
		public byte[] Picture { get; set; }

    }


	[Table("CategorySalesfor1997")]
    public partial class CategorySalesfor1997
    {
        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 15, Order = 1)]
		public string CategoryName { get; set; }

        [DbSchema(IsNullable = true, Order = 2)]
		public decimal? CategorySales { get; set; }

    }


	[Table("CurrentProductList")]
    public partial class CurrentProductList
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = ionix.Data.StoreGeneratedPattern.Identity, Order = 1)]
		public int ProductID { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 40, Order = 2)]
		public string ProductName { get; set; }

    }


	[Table("CustomerandSuppliersbyCity")]
    public partial class CustomerandSuppliersbyCity
    {
        [DbSchema(IsNullable = true, MaxLength = 15, Order = 1)]
		public string City { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 40, Order = 2)]
		public string CompanyName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 30, Order = 3)]
		public string ContactName { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 9, Order = 4)]
		public string Relationship { get; set; }

    }


	[Table("Customers")]
    public partial class Customers
    {
        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 5, Order = 1)]
		public string CustomerID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 40, Order = 2)]
		public string CompanyName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 30, Order = 3)]
		public string ContactName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 30, Order = 4)]
		public string ContactTitle { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60, Order = 5)]
		public string Address { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 6)]
		public string City { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 7)]
		public string Region { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10, Order = 8)]
		public string PostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 9)]
		public string Country { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 24, Order = 10)]
		public string Phone { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 24, Order = 11)]
		public string Fax { get; set; }

    }


	[Table("Employees")]
    public partial class Employees
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = ionix.Data.StoreGeneratedPattern.Identity, Order = 1)]
		public int EmployeeID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 20, Order = 2)]
		public string LastName { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 10, Order = 3)]
		public string FirstName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 30, Order = 4)]
		public string Title { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 25, Order = 5)]
		public string TitleOfCourtesy { get; set; }

        [DbSchema(IsNullable = true, Order = 6)]
		public DateTime? BirthDate { get; set; }

        [DbSchema(IsNullable = true, Order = 7)]
		public DateTime? HireDate { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60, Order = 8)]
		public string Address { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 9)]
		public string City { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 10)]
		public string Region { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10, Order = 11)]
		public string PostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 12)]
		public string Country { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 24, Order = 13)]
		public string HomePhone { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 4, Order = 14)]
		public string Extension { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 2147483647, Order = 15)]
		public byte[] Photo { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 1073741823, Order = 16)]
		public string Notes { get; set; }

        [DbSchema(IsNullable = true, Order = 17)]
		public int? ReportsTo { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 255, Order = 18)]
		public string PhotoPath { get; set; }

    }


	[Table("Invoices")]
    public partial class Invoices
    {
        [DbSchema(IsNullable = true, MaxLength = 40, Order = 1)]
		public string ShipName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60, Order = 2)]
		public string ShipAddress { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 3)]
		public string ShipCity { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 4)]
		public string ShipRegion { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10, Order = 5)]
		public string ShipPostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 6)]
		public string ShipCountry { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 5, Order = 7)]
		public string CustomerID { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 40, Order = 8)]
		public string CustomerName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60, Order = 9)]
		public string Address { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 10)]
		public string City { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 11)]
		public string Region { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10, Order = 12)]
		public string PostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 13)]
		public string Country { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 31, Order = 14)]
		public string Salesperson { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 15)]
		public int OrderID { get; set; }

        [DbSchema(IsNullable = true, Order = 16)]
		public DateTime? OrderDate { get; set; }

        [DbSchema(IsNullable = true, Order = 17)]
		public DateTime? RequiredDate { get; set; }

        [DbSchema(IsNullable = true, Order = 18)]
		public DateTime? ShippedDate { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 40, Order = 19)]
		public string ShipperName { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 20)]
		public int ProductID { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 40, Order = 21)]
		public string ProductName { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 22)]
		public decimal UnitPrice { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 23)]
		public short Quantity { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 24)]
		public float Discount { get; set; }

        [DbSchema(IsNullable = true, Order = 25)]
		public decimal? ExtendedPrice { get; set; }

        [DbSchema(IsNullable = true, Order = 26)]
		public decimal? Freight { get; set; }

    }


	[Table("OrderDetails")]
    public partial class OrderDetails
    {
        [DbSchema(IsNullable = false, IsKey=true, Order = 1)]
		public int OrderID { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 2)]
		public int ProductID { get; set; }

        [DbSchema(IsNullable = false, DefaultValue = "0m", Order = 3)]
		public decimal UnitPrice { get; set; }

        [DbSchema(IsNullable = false, DefaultValue = "1", Order = 4)]
		public short Quantity { get; set; }

        [DbSchema(IsNullable = false, DefaultValue = "0", Order = 5)]
		public float Discount { get; set; }

    }


	[Table("OrderDetailsExtended")]
    public partial class OrderDetailsExtended
    {
        [DbSchema(IsNullable = false, IsKey=true, Order = 1)]
		public int OrderID { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 2)]
		public int ProductID { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 40, Order = 3)]
		public string ProductName { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 4)]
		public decimal UnitPrice { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 5)]
		public short Quantity { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 6)]
		public float Discount { get; set; }

        [DbSchema(IsNullable = true, Order = 7)]
		public decimal? ExtendedPrice { get; set; }

    }


	[Table("Orders")]
    public partial class Orders
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = ionix.Data.StoreGeneratedPattern.Identity, Order = 1)]
		public int OrderID { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 5, Order = 2)]
		public string CustomerID { get; set; }

        [DbSchema(IsNullable = true, Order = 3)]
		public int? EmployeeID { get; set; }

        [DbSchema(IsNullable = true, Order = 4)]
		public DateTime? OrderDate { get; set; }

        [DbSchema(IsNullable = true, Order = 5)]
		public DateTime? RequiredDate { get; set; }

        [DbSchema(IsNullable = true, Order = 6)]
		public DateTime? ShippedDate { get; set; }

        [DbSchema(IsNullable = true, Order = 7)]
		public int? ShipVia { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "0m", Order = 8)]
		public decimal? Freight { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 40, Order = 9)]
		public string ShipName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60, Order = 10)]
		public string ShipAddress { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 11)]
		public string ShipCity { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 12)]
		public string ShipRegion { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10, Order = 13)]
		public string ShipPostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 14)]
		public string ShipCountry { get; set; }

    }


	[Table("OrdersQry")]
    public partial class OrdersQry
    {
        [DbSchema(IsNullable = false, IsKey=true, Order = 1)]
		public int OrderID { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 5, Order = 2)]
		public string CustomerID { get; set; }

        [DbSchema(IsNullable = true, Order = 3)]
		public int? EmployeeID { get; set; }

        [DbSchema(IsNullable = true, Order = 4)]
		public DateTime? OrderDate { get; set; }

        [DbSchema(IsNullable = true, Order = 5)]
		public DateTime? RequiredDate { get; set; }

        [DbSchema(IsNullable = true, Order = 6)]
		public DateTime? ShippedDate { get; set; }

        [DbSchema(IsNullable = true, Order = 7)]
		public int? ShipVia { get; set; }

        [DbSchema(IsNullable = true, Order = 8)]
		public decimal? Freight { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 40, Order = 9)]
		public string ShipName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60, Order = 10)]
		public string ShipAddress { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 11)]
		public string ShipCity { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 12)]
		public string ShipRegion { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10, Order = 13)]
		public string ShipPostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 14)]
		public string ShipCountry { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 40, Order = 15)]
		public string CompanyName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60, Order = 16)]
		public string Address { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 17)]
		public string City { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 18)]
		public string Region { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10, Order = 19)]
		public string PostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 20)]
		public string Country { get; set; }

    }


	[Table("OrderSubtotals")]
    public partial class OrderSubtotals
    {
        [DbSchema(IsNullable = false, IsKey=true, Order = 1)]
		public int OrderID { get; set; }

        [DbSchema(IsNullable = true, Order = 2)]
		public decimal? Subtotal { get; set; }

    }


	[Table("Products")]
    public partial class Products
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = ionix.Data.StoreGeneratedPattern.Identity, Order = 1)]
		public int ProductID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 40, Order = 2)]
		public string ProductName { get; set; }

        [DbSchema(IsNullable = true, Order = 3)]
		public int? SupplierID { get; set; }

        [DbSchema(IsNullable = true, Order = 4)]
		public int? CategoryID { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 20, Order = 5)]
		public string QuantityPerUnit { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "0m", Order = 6)]
		public decimal? UnitPrice { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "0", Order = 7)]
		public short? UnitsInStock { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "0", Order = 8)]
		public short? UnitsOnOrder { get; set; }

        [DbSchema(IsNullable = true, DefaultValue = "0", Order = 9)]
		public short? ReorderLevel { get; set; }

        [DbSchema(IsNullable = false, DefaultValue = "0", Order = 10)]
		public bool Discontinued { get; set; }

    }


	[Table("ProductsAboveAveragePrice")]
    public partial class ProductsAboveAveragePrice
    {
        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 40, Order = 1)]
		public string ProductName { get; set; }

        [DbSchema(IsNullable = true, Order = 2)]
		public decimal? UnitPrice { get; set; }

    }


	[Table("ProductSalesfor1997")]
    public partial class ProductSalesfor1997
    {
        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 15, Order = 1)]
		public string CategoryName { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 40, Order = 2)]
		public string ProductName { get; set; }

        [DbSchema(IsNullable = true, Order = 3)]
		public decimal? ProductSales { get; set; }

    }


	[Table("ProductsbyCategory")]
    public partial class ProductsbyCategory
    {
        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 15, Order = 1)]
		public string CategoryName { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 40, Order = 2)]
		public string ProductName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 20, Order = 3)]
		public string QuantityPerUnit { get; set; }

        [DbSchema(IsNullable = true, Order = 4)]
		public short? UnitsInStock { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 5)]
		public bool Discontinued { get; set; }

    }


	[Table("Region")]
    public partial class Region
    {
        [DbSchema(IsNullable = false, IsKey=true, Order = 1)]
		public int RegionID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50, Order = 2)]
		public string RegionDescription { get; set; }

    }


	[Table("SalesbyCategory")]
    public partial class SalesbyCategory
    {
        [DbSchema(IsNullable = false, IsKey=true, Order = 1)]
		public int CategoryID { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 15, Order = 2)]
		public string CategoryName { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 40, Order = 3)]
		public string ProductName { get; set; }

        [DbSchema(IsNullable = true, Order = 4)]
		public decimal? ProductSales { get; set; }

    }


	[Table("SalesTotalsbyAmount")]
    public partial class SalesTotalsbyAmount
    {
        [DbSchema(IsNullable = true, Order = 1)]
		public decimal? SaleAmount { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 2)]
		public int OrderID { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 40, Order = 3)]
		public string CompanyName { get; set; }

        [DbSchema(IsNullable = true, Order = 4)]
		public DateTime? ShippedDate { get; set; }

    }


	[Table("Shippers")]
    public partial class Shippers
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = ionix.Data.StoreGeneratedPattern.Identity, Order = 1)]
		public int ShipperID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 40, Order = 2)]
		public string CompanyName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 24, Order = 3)]
		public string Phone { get; set; }

    }


	[Table("SummaryofSalesbyQuarter")]
    public partial class SummaryofSalesbyQuarter
    {
        [DbSchema(IsNullable = true, Order = 1)]
		public DateTime? ShippedDate { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 2)]
		public int OrderID { get; set; }

        [DbSchema(IsNullable = true, Order = 3)]
		public decimal? Subtotal { get; set; }

    }


	[Table("SummaryofSalesbyYear")]
    public partial class SummaryofSalesbyYear
    {
        [DbSchema(IsNullable = true, Order = 1)]
		public DateTime? ShippedDate { get; set; }

        [DbSchema(IsNullable = false, IsKey=true, Order = 2)]
		public int OrderID { get; set; }

        [DbSchema(IsNullable = true, Order = 3)]
		public decimal? Subtotal { get; set; }

    }


	[Table("Suppliers")]
    public partial class Suppliers
    {
        [DbSchema(IsNullable = false, IsKey=true, DatabaseGeneratedOption = ionix.Data.StoreGeneratedPattern.Identity, Order = 1)]
		public int SupplierID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 40, Order = 2)]
		public string CompanyName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 30, Order = 3)]
		public string ContactName { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 30, Order = 4)]
		public string ContactTitle { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 60, Order = 5)]
		public string Address { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 6)]
		public string City { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 7)]
		public string Region { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 10, Order = 8)]
		public string PostalCode { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 15, Order = 9)]
		public string Country { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 24, Order = 10)]
		public string Phone { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 24, Order = 11)]
		public string Fax { get; set; }

        [DbSchema(IsNullable = true, MaxLength = 1073741823, Order = 12)]
		public string HomePage { get; set; }

    }


	[Table("Territories")]
    public partial class Territories
    {
        [DbSchema(IsNullable = false, IsKey=true, MaxLength = 20, Order = 1)]
		public string TerritoryID { get; set; }

        [DbSchema(IsNullable = false, MaxLength = 50, Order = 2)]
		public string TerritoryDescription { get; set; }

        [DbSchema(IsNullable = false, Order = 3)]
		public int RegionID { get; set; }

    }


}

