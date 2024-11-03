using System;
using System.Collections.Generic;

public interface ICheckout
{
    void Scan(string item);        // Adds an item to the cart by SKU
    int GetTotalPrice();           // Calculates and returns the total price with offers applied
}

public class Checkout : ICheckout
{
    // Pricing rules dictionary: stores each SKU and its pricing rules (unit price, special offer quantity, special offer price)
    private readonly Dictionary<string, (int UnitPrice, int? SpecialPriceQuantity, int? SpecialPrice)> _pricingRules;

    // Dictionary to track scanned items and their quantities
    private readonly Dictionary<string, int> _items = new();

    // Constructor that accepts the pricing rules, ensuring rules can be updated for different transactions
    public Checkout(Dictionary<string, (int UnitPrice, int? SpecialPriceQuantity, int? SpecialPrice)> pricingRules)
    {
        _pricingRules = pricingRules;
    }

    // Scan method to add an item to the cart by its SKU
    public void Scan(string item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Item SKU cannot be null.");

        if (item == "")
            throw new ArgumentException("Item SKU cannot be an empty string.", nameof(item));

        // Increase the count for this SKU or add it if it doesn't exist in the dictionary
        if (_items.ContainsKey(item))
            _items[item]++;
        else
            _items[item] = 1;
    }

    // Method to calculate the total price based on scanned items and current pricing rules
    public int GetTotalPrice()
    {
        int total = 0;

        // Iterate over each item in the cart and calculate its total cost, applying special offers if available
        foreach (var (item, count) in _items)
        {
            // Retrieve unit price and special pricing rules for the item
            if (_pricingRules.TryGetValue(item, out var pricing))
            {
                var (unitPrice, specialQuantity, specialPrice) = pricing;

                // If a special offer applies and quantity meets or exceeds the special offer quantity
                if (specialQuantity.HasValue && specialPrice.HasValue && count >= specialQuantity)
                {
                    // Calculate total using special offer: apply special offer groups, then add remaining items at unit price
                    total += (count / specialQuantity.Value) * specialPrice.Value + (count % specialQuantity.Value) * unitPrice;
                }
                else
                {
                    // If no special offer applies, calculate total for item based on unit price
                    total += count * unitPrice;
                }
            }
            else
            {
                // If item is scanned but not found in pricing rules, throw an exception
                throw new KeyNotFoundException($"The item '{item}' does not have a defined price.");
            }
        }

        // Return the final total price for all items in the cart
        return total;
    }
}
