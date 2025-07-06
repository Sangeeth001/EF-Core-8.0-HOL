using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main()
    {
        using var context = new AppDbContext();

        // Insert initial data
        var electronics = new Category { Name = "Electronics" };
        var groceries = new Category { Name = "Groceries" };

        await context.Categories.AddRangeAsync(electronics, groceries);

        var product1 = new Product { Name = "Laptop", Price = 75000, Category = electronics };
        var product2 = new Product { Name = "Rice Bag", Price = 1200, Category = groceries };

        await context.Products.AddRangeAsync(product1, product2);
        await context.SaveChangesAsync();

        // Retrieve all products
        var products = await context.Products.ToListAsync();
        foreach (var p in products)
            Console.WriteLine($"{p.Name} - ₹{p.Price}");

        // Find product by ID
        var product = await context.Products.FindAsync(1);
        Console.WriteLine($"Found: {product?.Name}");

        // First product with price > 50000
        var expensive = await context.Products.FirstOrDefaultAsync(p => p.Price > 50000);
        Console.WriteLine($"Expensive: {expensive?.Name}");

        // Update product price
        var updateProduct = await context.Products.FirstOrDefaultAsync(p => p.Name == "Laptop");
        if (updateProduct != null)
        {
            updateProduct.Price = 70000;
            await context.SaveChangesAsync();
            Console.WriteLine("Laptop price updated.");
        }

        // Delete product
        var deleteProduct = await context.Products.FirstOrDefaultAsync(p => p.Name == "Rice Bag");
        if (deleteProduct != null)
        {
            context.Products.Remove(deleteProduct);
            await context.SaveChangesAsync();
            Console.WriteLine("Rice Bag deleted.");
        }

        // Filter and sort products
        var filtered = await context.Products
            .Where(p => p.Price > 1000)
            .OrderByDescending(p => p.Price)
            .ToListAsync();

        Console.WriteLine("Filtered and sorted products:");
        foreach (var p in filtered)
            Console.WriteLine($"{p.Name} - ₹{p.Price}");

        // Project into DTO
        var productDTOs = await context.Products
            .Select(p => new { p.Name, p.Price })
            .ToListAsync();

        Console.WriteLine("Product DTOs:");
        foreach (var dto in productDTOs)
            Console.WriteLine($"{dto.Name} - ₹{dto.Price}");
    }
}
