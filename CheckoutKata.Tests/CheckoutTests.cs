using System;
using System.Collections.Generic;
using Xunit;

public class CheckoutTests
{
    private readonly ICheckout checkout;

    public CheckoutTests()
    {
        // Initialize the checkout system with a set of pricing rules, including items with various offers.
        var pricingRules = new Dictionary<string, (int UnitPrice, int? SpecialPriceQuantity, int? SpecialPrice)>
        {
            { "A", (50, 3, 130) },
            { "B", (30, 2, 45) },
            { "C", (20, null, null) },
            { "D", (15, null, null) },
            { "E", (40, 4, 120) },
            { "F", (10, 5, 40) },
            { "G", (25, null, null) },
            { "H", (60, 2, 100) }
        };

        checkout = new Checkout(pricingRules);
    }

    // Positive Test Cases
    [Trait("Category", "Positive Cases")]
    [Fact]
    public void Scan_SingleItemEWithoutOffer_ReturnsCorrectPrice()
    {
        // Scans a single item "E", which has a unit price of 40 and no discount at this quantity.
        checkout.Scan("E");
        // Expected outcome: Total price should be 40.
        Assert.Equal(40, checkout.GetTotalPrice());
    }

    [Trait("Category", "Positive Cases")]
    [Fact]
    public void Scan_FourItemsEWithOffer_ReturnsDiscountedPrice()
    {
        // Scans four "E" items, which triggers the 4-for-120 offer.
        checkout.Scan("E");
        checkout.Scan("E");
        checkout.Scan("E");
        checkout.Scan("E");
        // Expected outcome: Total price should be 120 as per the special offer.
        Assert.Equal(120, checkout.GetTotalPrice());
    }

    [Trait("Category", "Positive Cases")]
    [Fact]
    public void Scan_FiveItemsFWithOffer_ReturnsDiscountedPrice()
    {
        // Scans five "F" items, which triggers the 5-for-40 offer.
        checkout.Scan("F");
        checkout.Scan("F");
        checkout.Scan("F");
        checkout.Scan("F");
        checkout.Scan("F");
        // Expected outcome: Total price should be 40 as per the special offer.
        Assert.Equal(40, checkout.GetTotalPrice());
    }

    [Trait("Category", "Positive Cases")]
    [Fact]
    public void Scan_MultipleItemsWithVariousOffers_ReturnsCorrectTotal()
    {
        // Scans multiple items: 3 "A" items (3-for-130), 2 "B" items (2-for-45),
        // 4 "E" items (4-for-120), and 2 "H" items (2-for-100).
        checkout.Scan("A");
        checkout.Scan("A");
        checkout.Scan("A"); // 3-for-130 offer applied to A
        checkout.Scan("B");
        checkout.Scan("B"); // 2-for-45 offer applied to B
        checkout.Scan("E");
        checkout.Scan("E");
        checkout.Scan("E");
        checkout.Scan("E"); // 4-for-120 offer applied to E
        checkout.Scan("H");
        checkout.Scan("H"); // 2-for-100 offer applied to H
        // Expected outcome: Total price should be 395 (130 + 45 + 120 + 100).
        Assert.Equal(395, checkout.GetTotalPrice());
    }

    [Trait("Category", "Edge Cases")]
    [Fact]
    public void Scan_ItemsWithoutSpecialPrices_CalculatesTotalNormally()
    {
        // Scans items "C", "D", and "G" which have no special offers.
        checkout.Scan("C");
        checkout.Scan("D");
        checkout.Scan("G");
        // Expected outcome: Total price should be 60 (C=20, D=15, G=25).
        Assert.Equal(60, checkout.GetTotalPrice());
    }
}
