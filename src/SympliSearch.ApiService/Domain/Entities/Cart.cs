using System.ComponentModel.DataAnnotations;
using SympliSearch.ApiService.Domain.Common;

namespace SympliSearch.ApiService.Domain.Entities;

public enum CartStatus
{
    Pending = 0,
    Paid = 1
}

public class Cart : EntityBase
{
    [MaxLength(Constant.KeyLength)] public Guid? BuyerId { get; set; }

    public CartStatus Status { get; set; }

    public decimal SubTotal { get; set; }
    public decimal TotalPromotionDiscountAmount { get; set; }
    public decimal TotalCheckoutAmount { get; set; }

    public decimal DeliveryFee { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public Guid? PromotionId { get; set; }

    public Guid? DeliveryAddressId { get; set; }

    public DateTimeOffset? CheckoutAt { get; set; }
}
