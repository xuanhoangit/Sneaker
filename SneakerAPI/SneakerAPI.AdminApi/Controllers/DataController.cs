
using Microsoft.AspNetCore.Identity;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Libraries;
using SneakerAPI.Core.Models;
using SneakerAPI.Core.Models.OrderEntities;
using SneakerAPI.Core.Models.ProductEntities;
using SneakerAPI.Core.Models.UserEntities;

namespace SneakerAPI.AdminApi.Controllers.ProductControllers
{   
public static class SeedRoleAdmin
{   

        public static DateTime RandomDateThisMonth()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var range = (now - startOfMonth).TotalSeconds;
            var randomSeconds = Random.Shared.NextDouble() * range;

            return startOfMonth.AddSeconds(randomSeconds);
        }
        public static bool UpdatePrice(IUnitOfWork _uow){
            var pc =_uow.ProductColor.GetAll();
            foreach (var item in pc)
            {
                item.ProductColor__Price=Math.Floor(item.ProductColor__Price / 1000) * 1000;
                _uow.ProductColor.Update(item);
            }
            return true;
        }
        public static bool UpdatePhoneNumber(IUnitOfWork _uow){
            var pc =_uow.CustomerInfo.GetAll();
            foreach (var item in pc)
            {   
                var a=item.CustomerInfo__Phone.Substring(3);
                System.Console.WriteLine(a);
                    item.CustomerInfo__Phone="038" + a;
                
                _uow.CustomerInfo.Update(item);
            }
            return true;
        }
        public static bool UpdateDateProduct(IUnitOfWork _uow){
            var p =_uow.Product.GetAll();
            foreach (var item in p)
            {
                item.Product__CreatedDate=RandomDateThisMonth();
                _uow.Product.Update(item);
            }
            return true;
        }
        public static bool InitOrder(IUnitOfWork _uow){
            var isSeed=_uow.Order.GetAll().Take(1);
            if(isSeed.Any()){
                return false;
            }
            var random = new Random();

            var cartItemResult = _uow.CartItem.GetAll()
            .GroupBy(e => e.CartItem__CreatedByAccountId)
            .Select(g => new 
            {
                AccountId = g.Key,
                CartItemIds = g.Select(x => x.CartItem__Id).ToList(),
            }).ToList();
            foreach (var item in cartItemResult)
            {
            try
            {


                var cartItems = _uow.CartItem.GetCartItem(item.AccountId, item.CartItemIds.ToArray()).Result;
                var randomAddressId=_uow.Address.GetRandomAddressIdByAccountId(item.AccountId);
                if(!randomAddressId.Any())
                continue;
                // Tạo đơn hàng
                var order = new Order
                {
                    Order__CreatedByAccountId = item.AccountId,
                    Order__CreatedDate=RandomDateThisMonth(),
                    Order__AmountDue = cartItems.Sum(c => c.ProductColor.ProductColor__Price * c.CartItem__Quantity),
                    OrderItems = cartItems.Select(c => new OrderItem
                    {
                        OrderItem__ProductColorSizeId = c.CartItem__ProductColorSizeId,
                        OrderItem__Quantity = c.CartItem__Quantity,
                    }).ToList(),
                    Order__PaymentCode = long.Parse($"{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}"),
                    Order__Type = Form_of_purchase.Online,
                    Order__Status = (int)OrderStatus.Completed,
                    Order__PaymentStatus=(int)PaymentStatus.Paid,
                    Order__AddressId=randomAddressId[random.Next(0,randomAddressId.Count())]
                };

                var result = _uow.Order.Add(order);
                if (result)
                {   
                   var userIf=_uow.CustomerInfo.Get(item.AccountId);
                    foreach (var cs in cartItems)
                    {
                        var a=_uow.ProductColorSize.OrderLock(cs.CartItem__ProductColorSizeId,cs.CartItem__Quantity).Result;
                        System.Console.WriteLine(a);
                         userIf.CustomerInfo__TotalSpent+=cs.CartItem__Quantity* cs.ProductColor.ProductColor__Price;
                    }
                    //Cập nhật user totalspend
                    _uow.CustomerInfo.Update(userIf);
                    // Cập nhật trạng thái giỏ hàng
                    _uow.CartItem.RemoveRange(_uow.CartItem.Find(x => item.CartItemIds.Contains(x.CartItem__Id)));
            }
            }catch{

            }}
            return true;
        }
        public static bool InitCart(IUnitOfWork _uow){
            var isSeeed=_uow.CartItem.GetAll(x=>x.CartItem__Id>0).Take(1);
            // System.Console.WriteLine("isSeeed");
            // System.Console.WriteLine(isSeeed!=null);
            if(isSeeed.Any())
            return false;
            var random = new Random();
        var pcs=_uow.ProductColorSize.GetAll().ToList();
        var count=pcs.Count();
        var cartItems = new List<CartItem>();

        for (int i = 0; i < 800; i++)
        {
        var cartItem = new CartItem
            {
                CartItem__Quantity = random.Next(1, 5), // Số lượng từ 1 đến 5
                CartItem__CreatedByAccountId = random.Next(1, 100),
                CartItem__ProductColorSizeId = pcs[random.Next(1, count)].ProductColorSize__Id // ID từ 1 đến 700
            };
            
            cartItems.Add(cartItem);
        }
        var result=_uow.CartItem.AddRange(cartItems);
        if(!result)
        System.Console.WriteLine("Failllllllllllllllllllllllllllllllllllllllllllllllllllllllllll");
        return result;
        }
        public static bool InitProductColor(IUnitOfWork _uow){
            try
            {
                
          
            var isSeed=_uow.ProductColor.GetAll(x=> x.ProductColor__Id>0).Take(1);
            if(isSeed.Any()){
                System.Console.WriteLine("Data already seeded");
                return false;
            }
            var productColors = new List<ProductColor>
{
            // Air Force 1
            new ProductColor { ProductColor__Name = "White", ProductColor__Price = 2050000, ProductColor__ColorId = 1, ProductColor__Description = "Classic all-white Air Force 1", ProductColor__ProductId = 1 },
            new ProductColor { ProductColor__Name = "Black", ProductColor__Price = 2050000, ProductColor__ColorId = 2, ProductColor__Description = "Classic all-black Air Force 1", ProductColor__ProductId = 1 },
            new ProductColor { ProductColor__Name = "Triple Red", ProductColor__Price = 1800000, ProductColor__ColorId = 3, ProductColor__Description = "Bold red statement", ProductColor__ProductId = 1 },
            new ProductColor { ProductColor__Name = "University Blue", ProductColor__Price = 1800000, ProductColor__ColorId = 4, ProductColor__Description = "Popular UNC blue theme", ProductColor__ProductId = 1 },

            // Air Jordan 1
            new ProductColor { ProductColor__Name = "Bred", ProductColor__Price = 3200000, ProductColor__ColorId = 1, ProductColor__Description = "Black and red classic Jordan colorway", ProductColor__ProductId = 2 },
            new ProductColor { ProductColor__Name = "Chicago", ProductColor__Price = 3700000, ProductColor__ColorId = 2, ProductColor__Description = "OG white, red, black combo", ProductColor__ProductId = 2 },
            new ProductColor { ProductColor__Name = "Royal", ProductColor__Price = 32500000, ProductColor__ColorId = 3, ProductColor__Description = "Iconic blue and black combo", ProductColor__ProductId = 2 },
            new ProductColor { ProductColor__Name = "Pine Green", ProductColor__Price = 3250000, ProductColor__ColorId = 4, ProductColor__Description = "Green and black combo", ProductColor__ProductId = 2 },

            // Air Max 90
            new ProductColor { ProductColor__Name = "Infrared", ProductColor__Price = 21000000, ProductColor__ColorId = 1, ProductColor__Description = "Classic bright red hits", ProductColor__ProductId = 3 },
            new ProductColor { ProductColor__Name = "Laser Blue", ProductColor__Price = 21000000, ProductColor__ColorId = 2, ProductColor__Description = "Bright blue vibes", ProductColor__ProductId = 3 },
            new ProductColor { ProductColor__Name = "Volt", ProductColor__Price = 2150000, ProductColor__ColorId = 3, ProductColor__Description = "Neon green style", ProductColor__ProductId = 3 },
            new ProductColor { ProductColor__Name = "Duck Camo", ProductColor__Price = 15000000, ProductColor__ColorId = 4, ProductColor__Description = "Camo design for street wear", ProductColor__ProductId = 3 },

            // Dunk Low
            new ProductColor { ProductColor__Name = "Panda", ProductColor__Price = 9000000, ProductColor__ColorId = 1, ProductColor__Description = "Black and white staple", ProductColor__ProductId = 4 },
            new ProductColor { ProductColor__Name = "Syracuse", ProductColor__Price = 9500000, ProductColor__ColorId = 2, ProductColor__Description = "Orange & white college edition", ProductColor__ProductId = 4 },
            new ProductColor { ProductColor__Name = "Kentucky", ProductColor__Price = 9500000, ProductColor__ColorId = 3, ProductColor__Description = "Blue & white college edition", ProductColor__ProductId = 4 },
            new ProductColor { ProductColor__Name = "Brazil", ProductColor__Price = 9000000, ProductColor__ColorId = 4, ProductColor__Description = "Green & yellow combo", ProductColor__ProductId = 4 },

            // React Infinity Run
            new ProductColor { ProductColor__Name = "White/Crimson", ProductColor__Price = 32000000, ProductColor__ColorId = 1, ProductColor__Description = "Lightweight and energetic", ProductColor__ProductId = 5 },
            new ProductColor { ProductColor__Name = "Black/Volt", ProductColor__Price = 32000000, ProductColor__ColorId = 2, ProductColor__Description = "Dark with neon accent", ProductColor__ProductId = 5 },
            new ProductColor { ProductColor__Name = "Blue Ribbon", ProductColor__Price = 32500000, ProductColor__ColorId = 3, ProductColor__Description = "Sporty vibes", ProductColor__ProductId = 5 },
            new ProductColor { ProductColor__Name = "Multicolor", ProductColor__Price = 37000000, ProductColor__ColorId = 4, ProductColor__Description = "Energetic full-color", ProductColor__ProductId = 5 },

            // Blazer Mid '77
            new ProductColor { ProductColor__Name = "White/Black", ProductColor__Price = 10000000, ProductColor__ColorId = 1, ProductColor__Description = "Retro classic combo", ProductColor__ProductId = 6 },
            new ProductColor { ProductColor__Name = "Vintage Red", ProductColor__Price = 1050000, ProductColor__ColorId = 2, ProductColor__Description = "Old-school red look", ProductColor__ProductId = 6 },
            new ProductColor { ProductColor__Name = "Midnight Navy", ProductColor__Price = 1050000, ProductColor__ColorId = 3, ProductColor__Description = "Stylish navy finish", ProductColor__ProductId = 6 },
            new ProductColor { ProductColor__Name = "Sail", ProductColor__Price = 1000000, ProductColor__ColorId = 4, ProductColor__Description = "Off-white premium look", ProductColor__ProductId = 6 },

            // Ultraboost 23
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 4900000, ProductColor__ColorId = 1, ProductColor__Description = "Premium comfort in black", ProductColor__ProductId = 11 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 4900000, ProductColor__ColorId = 2, ProductColor__Description = "Crisp clean white", ProductColor__ProductId = 11 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 5000000, ProductColor__ColorId = 3, ProductColor__Description = "High-energy run style", ProductColor__ProductId = 11 },
new ProductColor { ProductColor__Name = "Legacy Indigo", ProductColor__Price = 5000000, ProductColor__ColorId = 4, ProductColor__Description = "Cool navy tone", ProductColor__ProductId = 11 },

// Yeezy Boost 350
new ProductColor { ProductColor__Name = "Beluga", ProductColor__Price = 6900000, ProductColor__ColorId = 1, ProductColor__Description = "OG color with orange stripe", ProductColor__ProductId = 12 },
new ProductColor { ProductColor__Name = "Zebra", ProductColor__Price = 7000000, ProductColor__ColorId = 2, ProductColor__Description = "Black and white contrast", ProductColor__ProductId = 12 },
new ProductColor { ProductColor__Name = "Cream White", ProductColor__Price = 7100000, ProductColor__ColorId = 3, ProductColor__Description = "Minimal all-white", ProductColor__ProductId = 12 },
new ProductColor { ProductColor__Name = "Static", ProductColor__Price = 7200000, ProductColor__ColorId = 4, ProductColor__Description = "Subtle grey pattern", ProductColor__ProductId = 12 },

// Samba OG
new ProductColor { ProductColor__Name = "White/Blue", ProductColor__Price = 2300000, ProductColor__ColorId = 1, ProductColor__Description = "Classic soccer-inspired", ProductColor__ProductId = 13 },
new ProductColor { ProductColor__Name = "Black/Gum", ProductColor__Price = 2300000, ProductColor__ColorId = 2, ProductColor__Description = "Timeless streetwear style", ProductColor__ProductId = 13 },
new ProductColor { ProductColor__Name = "Green/White", ProductColor__Price = 2400000, ProductColor__ColorId = 3, ProductColor__Description = "Fresh casual tone", ProductColor__ProductId = 13 },
new ProductColor { ProductColor__Name = "Red/White", ProductColor__Price = 2400000, ProductColor__ColorId = 4, ProductColor__Description = "Bright standout option", ProductColor__ProductId = 13 },

// Gazelle
new ProductColor { ProductColor__Name = "Blue/White", ProductColor__Price = 2200000, ProductColor__ColorId = 1, ProductColor__Description = "Classic retro blue", ProductColor__ProductId = 14 },
new ProductColor { ProductColor__Name = "Green/White", ProductColor__Price = 2200000, ProductColor__ColorId = 2, ProductColor__Description = "Evergreen suede style", ProductColor__ProductId = 14 },
new ProductColor { ProductColor__Name = "Pink/White", ProductColor__Price = 2300000, ProductColor__ColorId = 3, ProductColor__Description = "Playful street option", ProductColor__ProductId = 14 },
new ProductColor { ProductColor__Name = "Black/Gold", ProductColor__Price = 2300000, ProductColor__ColorId = 4, ProductColor__Description = "Chic and versatile", ProductColor__ProductId = 14 },

// NMD R1
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3500000, ProductColor__ColorId = 1, ProductColor__Description = "Stealth city look", ProductColor__ProductId = 15 },
new ProductColor { ProductColor__Name = "Core White", ProductColor__Price = 3500000, ProductColor__ColorId = 2, ProductColor__Description = "Clean urban vibe", ProductColor__ProductId = 15 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 3600000, ProductColor__ColorId = 3, ProductColor__Description = "High-contrast streetwear", ProductColor__ProductId = 15 },
new ProductColor { ProductColor__Name = "Grey Six", ProductColor__Price = 3600000, ProductColor__ColorId = 4, ProductColor__Description = "Neutral everyday tone", ProductColor__ProductId = 15 },
// Superstar
new ProductColor { ProductColor__Name = "White/Black", ProductColor__Price = 2500000, ProductColor__ColorId = 1, ProductColor__Description = "Classic Adidas icon", ProductColor__ProductId = 16 },
new ProductColor { ProductColor__Name = "White/Green", ProductColor__Price = 2500000, ProductColor__ColorId = 2, ProductColor__Description = "Retro tennis vibe", ProductColor__ProductId = 16 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 2600000, ProductColor__ColorId = 3, ProductColor__Description = "Sleek all-black design", ProductColor__ProductId = 16 },
new ProductColor { ProductColor__Name = "White/Gold", ProductColor__Price = 2700000, ProductColor__ColorId = 4, ProductColor__Description = "Elegant classic twist", ProductColor__ProductId = 16 },

// Forum Low
new ProductColor { ProductColor__Name = "White/Blue", ProductColor__Price = 2700000, ProductColor__ColorId = 1, ProductColor__Description = "Basketball heritage", ProductColor__ProductId = 17 },
new ProductColor { ProductColor__Name = "Triple White", ProductColor__Price = 2700000, ProductColor__ColorId = 2, ProductColor__Description = "Clean all-white silhouette", ProductColor__ProductId = 17 },
new ProductColor { ProductColor__Name = "Black/White", ProductColor__Price = 2750000, ProductColor__ColorId = 3, ProductColor__Description = "Street-ready staple", ProductColor__ProductId = 17 },
new ProductColor { ProductColor__Name = "White/Red", ProductColor__Price = 2750000, ProductColor__ColorId = 4, ProductColor__Description = "Retro sporty touch", ProductColor__ProductId = 17 },

// Adizero Adios Pro 3
new ProductColor { ProductColor__Name = "Pulse Mint", ProductColor__Price = 5900000, ProductColor__ColorId = 1, ProductColor__Description = "Elite racing shoe", ProductColor__ProductId = 18 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 5900000, ProductColor__ColorId = 2, ProductColor__Description = "Speed and performance", ProductColor__ProductId = 18 },
new ProductColor { ProductColor__Name = "White/Black", ProductColor__Price = 6000000, ProductColor__ColorId = 3, ProductColor__Description = "Race-day neutral", ProductColor__ProductId = 18 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 6000000, ProductColor__ColorId = 4, ProductColor__Description = "Bold energy runner", ProductColor__ProductId = 18 },

// Predator Accuracy
new ProductColor { ProductColor__Name = "Black/Pink", ProductColor__Price = 4200000, ProductColor__ColorId = 1, ProductColor__Description = "Precision on the pitch", ProductColor__ProductId = 19 },
new ProductColor { ProductColor__Name = "White/Gold", ProductColor__Price = 4300000, ProductColor__ColorId = 2, ProductColor__Description = "Limited edition finesse", ProductColor__ProductId = 19 },
new ProductColor { ProductColor__Name = "Red/Core Black", ProductColor__Price = 4300000, ProductColor__ColorId = 3, ProductColor__Description = "Dominant control look", ProductColor__ProductId = 19 },
new ProductColor { ProductColor__Name = "Blue/White", ProductColor__Price = 4300000, ProductColor__ColorId = 4, ProductColor__Description = "Bright and bold", ProductColor__ProductId = 19 },

// Ozweego
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3100000, ProductColor__ColorId = 1, ProductColor__Description = "Futuristic chunky style", ProductColor__ProductId = 20 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 3100000, ProductColor__ColorId = 2, ProductColor__Description = "Neutral lifestyle tone", ProductColor__ProductId = 20 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3150000, ProductColor__ColorId = 3, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 20 },
new ProductColor { ProductColor__Name = "Solar Yellow", ProductColor__Price = 3200000, ProductColor__ColorId = 4, ProductColor__Description = "Bold Y2K aesthetic", ProductColor__ProductId = 20 },
// ProductColor data from Product__Id 21 to 190
// ProductColor data from Product__Id 21 to 190
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 5252792, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 21 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6565467, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 21 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 7051540, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 21 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 5475887, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 21 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 3729739, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 22 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2637064, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 22 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3696838, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 22 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2582070, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 22 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2073923, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 23 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 2884718, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 23 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 4872159, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 23 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 2535436, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 23 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 2562417, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 24 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2708991, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 24 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 6046058, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 24 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 5914831, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 24 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5374953, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 25 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 6354773, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 25 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 4069055, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 25 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4651435, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 25 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 6507399, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 26 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2754077, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 26 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2191762, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 26 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 4379448, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 26 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 3750691, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 27 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2330321, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 27 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 5978917, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 27 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2208569, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 27 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2952507, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 28 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 4947821, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 28 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3091451, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 28 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 4034575, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 28 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 4816621, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 29 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 4401595, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 29 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 7431622, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 29 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6868785, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 29 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2081833, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 30 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6629007, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 30 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 7409450, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 30 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 6051488, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 30 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 2036175, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 31 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2135389, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 31 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2182283, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 31 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 2185074, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 31 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6724446, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 32 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 7336383, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 32 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 5014238, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 32 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 6928980, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 32 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 5139303, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 33 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 5278908, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 33 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2689774, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 33 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 5238542, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 33 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6343533, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 34 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 3570466, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 34 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3666780, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 34 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6910408, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 34 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 3319891, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 35 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 7352479, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 35 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2730780, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 35 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 5711055, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 35 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 6007557, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 36 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6351282, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 36 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2360091, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 36 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 5629444, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 36 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 4783801, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 37 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 3351076, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 37 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 7071874, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 37 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3892674, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 37 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 7470005, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 38 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 2492928, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 38 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3489816, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 38 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 3835577, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 38 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6856528, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 39 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 4342342, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 39 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 5996439, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 39 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 5918349, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 39 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2942685, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 40 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 3310037, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 40 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2747302, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 40 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2571293, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 40 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 2087494, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 41 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 5632831, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 41 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4241712, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 41 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6892572, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 41 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4875807, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 42 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 6784596, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 42 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 5913228, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 42 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5957896, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 42 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 7212357, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 43 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 2359604, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 43 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5949553, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 43 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 6677720, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 43 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6221283, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 44 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2302838, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 44 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 6475815, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 44 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 4238145, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 44 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 3307221, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 45 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 2770927, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 45 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 5188039, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 45 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 5021701, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 45 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4365724, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 46 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2384916, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 46 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 3183145, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 46 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 2383609, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 46 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 5630230, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 47 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 6824972, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 47 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 7489871, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 47 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 6238649, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 47 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 3281379, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 48 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 4055958, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 48 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 7229463, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 48 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6573305, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 48 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 3000072, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 49 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2814921, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 49 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 5783418, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 49 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6664083, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 49 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2129013, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 50 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6405462, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 50 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2282843, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 50 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 5812189, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 50 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6564424, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 51 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6931626, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 51 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 6814080, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 51 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 5471876, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 51 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6851573, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 52 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 7240120, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 52 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6937538, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 52 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 4958850, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 52 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2414323, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 53 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 4513706, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 53 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 3957932, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 53 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3135851, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 53 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6770140, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 54 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 3765968, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 54 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4896593, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 54 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 4121568, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 54 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 6592347, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 55 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 6265447, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 55 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2637966, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 55 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 6173487, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 55 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 5309142, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 56 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2547112, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 56 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4650685, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 56 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6032006, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 56 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 3968511, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 57 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2847150, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 57 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 3932399, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 57 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2477037, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 57 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2747394, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 58 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 2687986, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 58 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3625354, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 58 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 4171763, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 58 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 3694603, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 59 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 4796133, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 59 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 2145075, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 59 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6127312, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 59 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2154798, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 60 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6814951, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 60 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2117662, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 60 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 3330023, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 60 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 7116238, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 61 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4266307, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 61 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2201698, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 61 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 7409817, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 61 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 7281139, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 62 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 7420213, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 62 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 2161876, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 62 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 7099672, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 62 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 7434054, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 63 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3182730, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 63 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2555114, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 63 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3912904, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 63 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 2945139, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 64 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 7160667, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 64 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3190559, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 64 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2192859, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 64 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 3471339, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 65 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 5061456, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 65 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 3317476, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 65 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2411895, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 65 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 5424484, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 66 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 7367823, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 66 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4062311, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 66 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6929065, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 66 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 3518715, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 67 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 7181179, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 67 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 2842019, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 67 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 5673499, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 67 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4707644, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 68 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 7492450, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 68 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 6030792, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 68 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 5835155, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 68 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 2860343, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 69 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 4286805, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 69 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6819911, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 69 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6671552, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 69 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 5751915, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 70 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 2615759, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 70 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4752197, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 70 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 5778861, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 70 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 4150904, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 71 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4938852, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 71 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 6933512, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 71 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 2077395, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 71 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 5789284, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 72 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4181783, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 72 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3307561, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 72 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5535627, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 72 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2109810, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 73 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 4657927, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 73 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5448485, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 73 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4264442, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 73 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3357318, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 74 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5633852, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 74 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4648230, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 74 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2432118, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 74 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5614444, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 75 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3422870, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 75 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6984563, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 75 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 5659644, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 75 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4117499, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 76 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6893448, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 76 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 6918316, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 76 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6075459, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 76 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6736977, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 77 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4964665, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 77 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6025334, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 77 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 5810396, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 77 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 5099257, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 78 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6474246, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 78 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3488430, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 78 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 4196059, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 78 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 2470983, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 79 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 5824826, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 79 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 4585153, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 79 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4458393, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 79 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 4469131, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 80 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 5368181, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 80 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 3699493, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 80 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 5826908, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 80 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 7417851, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 81 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 3931033, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 81 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4149803, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 81 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6169860, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 81 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4019473, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 82 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2087435, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 82 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 4641920, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 82 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2810075, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 82 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 3142218, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 83 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6430643, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 83 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 7410492, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 83 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 7379674, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 83 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 2414350, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 84 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 3275361, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 84 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 6125715, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 84 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 4669658, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 84 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5505649, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 85 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3955343, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 85 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6406462, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 85 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 5923867, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 85 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 5903007, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 86 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 3524959, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 86 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2666634, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 86 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 7435881, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 86 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 5187124, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 87 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2156806, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 87 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6850689, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 87 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2002021, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 87 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4667328, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 88 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 3888829, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 88 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 6684847, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 88 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 2680153, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 88 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 4235754, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 89 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3992791, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 89 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 4080984, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 89 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4589449, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 89 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 5189419, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 90 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6396993, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 90 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 3368317, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 90 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4727014, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 90 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 3804393, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 91 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 3541440, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 91 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4592180, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 91 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6664039, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 91 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 3351461, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 92 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4432872, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 92 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 2424168, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 92 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5849770, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 92 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2713785, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 93 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3469376, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 93 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5783880, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 93 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4562817, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 93 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6903819, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 94 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2215543, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 94 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4428845, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 94 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6567702, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 94 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2386053, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 95 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4825871, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 95 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2159069, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 95 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2100106, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 95 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3047412, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 96 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 5031348, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 96 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 7041304, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 96 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6182423, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 96 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6137188, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 97 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4735117, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 97 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 2738078, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 97 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3426016, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 97 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 6760413, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 98 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 7391871, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 98 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 5453670, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 98 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 4709035, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 98 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 5599001, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 99 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 7039612, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 99 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 5798834, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 99 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4627517, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 99 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2483465, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 100 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 2217055, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 100 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2532323, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 100 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 5049513, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 100 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6522011, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 101 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 7036005, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 101 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4683249, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 101 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 4386525, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 101 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 3034766, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 102 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 5182617, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 102 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 5743296, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 102 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2564166, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 102 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 3113957, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 103 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 4418395, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 103 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 4671870, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 103 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4520914, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 103 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3181101, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 104 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5242819, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 104 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3737337, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 104 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 5087599, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 104 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 4272214, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 105 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3911925, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 105 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6495666, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 105 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 7233336, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 105 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 7368691, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 106 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2060940, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 106 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4448438, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 106 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 4815371, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 106 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 4575296, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 107 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 6468909, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 107 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 5870782, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 107 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 6303019, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 107 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 5518775, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 108 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 2478644, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 108 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 5952111, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 108 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 7141390, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 108 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6259125, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 109 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 6724372, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 109 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6602053, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 109 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2473161, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 109 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 6955350, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 110 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6576083, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 110 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6136720, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 110 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 6352275, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 110 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 7174168, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 111 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 5159110, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 111 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 5085438, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 111 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 7410183, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 111 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6965487, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 112 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2529122, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 112 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 2995808, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 112 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5993181, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 112 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2962940, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 113 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6746481, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 113 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 4280466, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 113 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3836374, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 113 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 2278370, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 114 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2148139, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 114 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4082148, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 114 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2564842, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 114 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 7344641, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 115 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4354383, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 115 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6698142, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 115 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4153323, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 115 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4008140, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 116 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6394727, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 116 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 6602792, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 116 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 3924209, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 116 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 7215281, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 117 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2693372, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 117 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 3740875, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 117 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3382746, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 117 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2880952, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 118 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6447138, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 118 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 6559376, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 118 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 3431650, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 118 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 5363017, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 119 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2033753, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 119 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 3072825, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 119 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 3351909, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 119 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3332331, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 120 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 3097118, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 120 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2133775, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 120 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 5047799, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 120 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 4213259, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 121 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 5001481, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 121 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4831386, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 121 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3076553, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 121 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4357803, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 122 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4784456, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 122 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 7341066, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 122 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5053645, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 122 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4891521, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 123 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 5725977, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 123 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 4702210, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 123 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 5638441, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 123 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3426780, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 124 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 4761359, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 124 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3300646, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 124 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2030421, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 124 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2626474, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 125 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 5850000, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 125 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 5765409, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 125 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 7171075, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 125 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4890665, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 126 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 5765977, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 126 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 5040184, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 126 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 2688211, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 126 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 3259406, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 127 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 5847057, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 127 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 4672552, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 127 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 6630776, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 127 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 6710012, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 128 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 4257987, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 128 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 7394545, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 128 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 7056448, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 128 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6754308, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 129 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3575755, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 129 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 3199667, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 129 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 7483947, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 129 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3685881, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 130 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 5467518, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 130 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6159621, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 130 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 3284121, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 130 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 5775704, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 131 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4930071, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 131 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4748194, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 131 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 2658530, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 131 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2197925, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 132 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 5578578, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 132 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3941039, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 132 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 3768481, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 132 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 6141768, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 133 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 5766770, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 133 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 4512044, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 133 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3029251, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 133 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 7435157, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 134 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 6722260, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 134 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 6632758, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 134 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6873546, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 134 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 4493136, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 135 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3553831, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 135 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 7082034, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 135 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 5690756, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 135 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 6149207, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 136 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6769992, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 136 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 3265494, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 136 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 5002582, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 136 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6647749, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 137 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4845719, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 137 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 7048163, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 137 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 5283000, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 137 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2034661, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 138 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 5594660, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 138 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 7057050, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 138 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 2947524, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 138 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 3797498, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 139 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3458053, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 139 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 5549852, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 139 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6695205, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 139 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2310811, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 140 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 5693592, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 140 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2568697, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 140 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2188449, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 140 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 4467133, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 141 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2065266, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 141 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 3595561, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 141 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3720542, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 141 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2274894, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 142 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 3632024, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 142 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 4866699, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 142 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5225906, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 142 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4118657, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 143 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 2039870, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 143 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5501899, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 143 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 6877820, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 143 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3482254, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 144 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 6672992, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 144 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 7408568, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 144 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 5996056, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 144 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 3411425, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 145 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3613764, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 145 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2325136, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 145 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2024779, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 145 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 2529385, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 146 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 4993927, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 146 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2406356, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 146 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6717769, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 146 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6262962, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 147 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2371876, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 147 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 4044416, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 147 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 5819750, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 147 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 5559637, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 148 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 2695295, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 148 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3097747, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 148 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 2991064, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 148 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 3295058, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 149 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 5702533, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 149 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 3136377, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 149 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6121976, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 149 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 4536583, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 150 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 4280564, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 150 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6595576, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 150 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 6419957, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 150 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 2598964, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 151 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4008264, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 151 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2657281, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 151 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3723244, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 151 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 3311555, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 152 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 3987772, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 152 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6605126, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 152 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 7265254, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 152 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 5386723, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 153 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3657958, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 153 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 4382260, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 153 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 7496661, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 153 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 4486341, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 154 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 4225022, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 154 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 6225226, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 154 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 4215283, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 154 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 5114177, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 155 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 2348134, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 155 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 7384404, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 155 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4085255, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 155 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3286779, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 156 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 4116746, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 156 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2954762, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 156 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 2782535, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 156 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 4707388, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 157 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2918733, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 157 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 2222179, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 157 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 7341142, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 157 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4230997, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 158 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 7064578, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 158 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 7076898, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 158 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 4662716, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 158 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 5391710, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 159 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2528744, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 159 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 3751451, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 159 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 3405763, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 159 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 5415721, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 160 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 3495595, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 160 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4427124, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 160 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 3119402, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 160 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6187245, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 161 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6556833, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 161 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2149822, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 161 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6268616, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 161 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2873743, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 162 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 6295066, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 162 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3821254, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 162 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 6484523, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 162 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2780565, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 163 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 7154944, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 163 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2491603, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 163 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 6940458, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 163 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6532903, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 164 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2336596, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 164 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3897927, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 164 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 5980231, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 164 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2382003, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 165 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 2061360, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 165 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 4180196, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 165 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4598967, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 165 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 7380241, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 166 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 3933546, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 166 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 5132774, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 166 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 3708076, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 166 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 5117597, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 167 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2655045, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 167 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 4135409, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 167 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 5870503, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 167 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 7148533, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 168 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6955991, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 168 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 6014029, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 168 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 3691546, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 168 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 5338017, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 169 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 4500018, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 169 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 2665987, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 169 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2755912, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 169 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 7050707, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 170 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 5679532, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 170 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 5685191, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 170 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 3014579, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 170 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 3078051, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 171 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2817215, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 171 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 6742672, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 171 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 4099476, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 171 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4541925, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 172 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4561341, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 172 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6529813, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 172 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 6239279, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 172 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4970846, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 173 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 7313758, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 173 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 6821777, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 173 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3592578, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 173 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 4466531, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 174 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2614913, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 174 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 5577352, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 174 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6695053, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 174 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 3126770, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 175 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 7127285, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 175 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 6555152, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 175 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2690195, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 175 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 3331969, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 176 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2652248, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 176 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 3135627, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 176 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 5999607, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 176 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 2782746, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 177 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 6518168, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 177 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 4818487, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 177 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2925114, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 177 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 7315760, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 178 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 4104720, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 178 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 7440802, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 178 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 4164288, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 178 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 6547273, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 179 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 5530322, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 179 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6813128, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 179 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6409507, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 179 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 6763433, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 180 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6141353, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 180 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4227594, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 180 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 5683512, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 180 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 2821270, ProductColor__ColorId = 1, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 181 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4531103, ProductColor__ColorId = 2, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 181 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4083729, ProductColor__ColorId = 3, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 181 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 3211694, ProductColor__ColorId = 4, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 181 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 6692248, ProductColor__ColorId = 5, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 182 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2586703, ProductColor__ColorId = 6, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 182 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 6608298, ProductColor__ColorId = 7, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 182 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 7387408, ProductColor__ColorId = 8, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 182 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 4833692, ProductColor__ColorId = 9, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 183 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 5508344, ProductColor__ColorId = 10, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 183 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 2826939, ProductColor__ColorId = 1, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 183 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 2381849, ProductColor__ColorId = 2, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 183 },
new ProductColor { ProductColor__Name = "Grey Two", ProductColor__Price = 4614627, ProductColor__ColorId = 3, ProductColor__Description = "Comfort for all-day wear", ProductColor__ProductId = 184 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 7249243, ProductColor__ColorId = 4, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 184 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 4317181, ProductColor__ColorId = 5, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 184 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 4684680, ProductColor__ColorId = 6, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 184 },
new ProductColor { ProductColor__Name = "Off White", ProductColor__Price = 4695273, ProductColor__ColorId = 7, ProductColor__Description = "Streetwear essential", ProductColor__ProductId = 185 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 5851925, ProductColor__ColorId = 8, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 185 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 7267679, ProductColor__ColorId = 9, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 185 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 6451247, ProductColor__ColorId = 10, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 185 },
new ProductColor { ProductColor__Name = "Triple Black", ProductColor__Price = 5672239, ProductColor__ColorId = 1, ProductColor__Description = "Retro-inspired silhouette", ProductColor__ProductId = 186 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 4884679, ProductColor__ColorId = 2, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 186 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 2777594, ProductColor__ColorId = 3, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 186 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 3754214, ProductColor__ColorId = 4, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 186 },
new ProductColor { ProductColor__Name = "Mint Green", ProductColor__Price = 7156525, ProductColor__ColorId = 5, ProductColor__Description = "Tech-forward build", ProductColor__ProductId = 187 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 4916169, ProductColor__ColorId = 6, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 187 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 7439887, ProductColor__ColorId = 7, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 187 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2063573, ProductColor__ColorId = 8, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 187 },
new ProductColor { ProductColor__Name = "Volt Yellow", ProductColor__Price = 3324507, ProductColor__ColorId = 9, ProductColor__Description = "Casual cool style", ProductColor__ProductId = 188 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 3320653, ProductColor__ColorId = 10, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 188 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 3948877, ProductColor__ColorId = 1, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 188 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 2276563, ProductColor__ColorId = 2, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 188 },
new ProductColor { ProductColor__Name = "Rose Pink", ProductColor__Price = 7479629, ProductColor__ColorId = 3, ProductColor__Description = "Elite performance sneaker", ProductColor__ProductId = 189 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 7372794, ProductColor__ColorId = 4, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 189 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 6124823, ProductColor__ColorId = 5, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 189 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 4141134, ProductColor__ColorId = 6, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 189 },
new ProductColor { ProductColor__Name = "Core Black", ProductColor__Price = 2689207, ProductColor__ColorId = 7, ProductColor__Description = "Everyday classic style", ProductColor__ProductId = 190 },
new ProductColor { ProductColor__Name = "Cloud White", ProductColor__Price = 7354994, ProductColor__ColorId = 8, ProductColor__Description = "Sporty and sleek design", ProductColor__ProductId = 190 },
new ProductColor { ProductColor__Name = "Solar Red", ProductColor__Price = 2065961, ProductColor__ColorId = 9, ProductColor__Description = "Modern performance look", ProductColor__ProductId = 190 },
new ProductColor { ProductColor__Name = "Blue Burst", ProductColor__Price = 2123723, ProductColor__ColorId = 10, ProductColor__Description = "Bold and iconic choice", ProductColor__ProductId = 190 },
        };
            foreach (var item in productColors)
            {
                item.ProductColor__Price=Math.Floor(item.ProductColor__Price / 1000) * 1000;
            }
            var result=_uow.ProductColor.AddRange(productColors);
            if(result){
            System.Console.WriteLine("Added product color successfully");
            return true;
            }
            System.Console.WriteLine("Add product color failed");
            return false;
              }
            catch (System.Exception e)
            {
                
                System.Console.WriteLine(e);
                return false;
            }
        }
 
    public static void InitProductColorSize(IUnitOfWork uow){
        var pcs=uow.ProductColorSize.GetAll().Take(1);
        if(pcs==null || !pcs.Any()){
            var productColorSizes=new List<ProductColorSize>();
            var productColors=uow.ProductColor.GetAll();
            var sizes=uow.Size.GetAll();
            foreach (var pc in productColors)
            {
                foreach (var size in sizes)
                {
                    productColorSizes.Add(new ProductColorSize{
                        ProductColorSize__ProductColorId=pc.ProductColor__Id,
                        ProductColorSize__SizeId=size.Size__Id,
                        ProductColorSize__Quantity=1000
                    });
                }
            }
            uow.ProductColorSize.AddRange(productColorSizes);
        }
        System.Console.WriteLine("Create product color size success");
    }
    public static void InitSize(IUnitOfWork uow){
        var sizes = uow.Size.GetAll();
        var listSize=new List<Size>();
        if(!sizes.Any()|| sizes==null){
            for(int size=35;size<45;size++)
            {
                listSize.Add(new Size{
                    Size__Value=size.ToString()
                });
            }
        }
      try
      {
          uow.Size.AddRange(listSize);
        System.Console.WriteLine("Crate size succes");
      }
      catch (System.Exception)
      {
        
        System.Console.WriteLine("fale create size");
      }
    }
    public static void InitProductCategory(IUnitOfWork uow){
        var product_id=uow.Product.GetAll().Select(x=>x.Product__Id).ToList();
        var cate_id=uow.Category.GetAll().Select(x=>x.Category__Id).ToList();
        var product_cate=new List<ProductCategory>();
        try
        {
        
        if(uow.ProductCategory.GetAll()==null||!uow.ProductCategory.GetAll().Any()){
        int quanity_product_in_cate = product_id.Count() / cate_id.Count();
        int balance= product_id.Count() % cate_id.Count();
        for (int i = 0; i < cate_id.Count() ; i++)
        {   
            if((i*quanity_product_in_cate)+quanity_product_in_cate>product_id.Count()){
                break;
            }
           for (int j = i*quanity_product_in_cate; j < (i*quanity_product_in_cate)+quanity_product_in_cate; j++)
           {
                product_cate.Add(
                    new ProductCategory{
                        ProductCategory__ProductId=product_id[j],
                        ProductCategory__CategoryId=cate_id[i]
                    }
                );
           }
        }
        uow.ProductCategory.AddRange(product_cate);
        }

            System.Console.WriteLine("created product cate");    
        }
        catch (System.Exception)
        { 
            System.Console.WriteLine("faile to craeted product cate");
        }
    }
    public static void InitCategory(IUnitOfWork uow){
         var categories = new List<Category>
                {
                    new Category { Category__Name = "Sneakers", Category__Description = "Casual and sporty footwear." },
                    new Category { Category__Name = "Running Shoes", Category__Description = "Designed for running and jogging." },
                    new Category { Category__Name = "Basketball Shoes", Category__Description = "High-performance shoes for basketball players." },
                    new Category { Category__Name = "Boots", Category__Description = "Durable footwear for various terrains." },
                    new Category { Category__Name = "Sandals", Category__Description = "Open-toed footwear for warm weather." },
                    new Category { Category__Name = "Formal Shoes", Category__Description = "Elegant footwear for business and events." },
                    new Category { Category__Name = "Tennis Shoes", Category__Description = "Shoes specifically made for tennis players." },
                    new Category { Category__Name = "Skate Shoes", Category__Description = "Designed for skateboarding and grip." },
                    new Category { Category__Name = "Loafers", Category__Description = "Slip-on shoes for casual and semi-formal occasions." },
                    new Category { Category__Name = "Hiking Shoes", Category__Description = "Shoes for outdoor activities and rough terrain." }
                };
        try
        {
            var cate=uow.Category.GetAll();
            if(cate==null||!cate.Any()){
                uow.Category.AddRange(categories);
            }
            System.Console.WriteLine("Create 10 categories");
        }
        catch (System.Exception)
        {
            
             System.Console.WriteLine("Fail to created category");
        }
    }
    public static void InitColor(IUnitOfWork uow){
                var colors = new List<Color>
                {
                    new Color { Color__Name = "Black", Color__Description = "A classic and versatile color suitable for various styles." },
                    new Color { Color__Name = "White", Color__Description = "A clean and neutral color that pairs well with any outfit." },
                    new Color { Color__Name = "Red", Color__Description = "A bold and vibrant color that stands out." },
                    new Color { Color__Name = "Blue", Color__Description = "A calm and cool color, popular in many designs." },
                    new Color { Color__Name = "Green", Color__Description = "A refreshing color often associated with nature." },
                    new Color { Color__Name = "Yellow", Color__Description = "A bright and cheerful color that adds energy." },
                    new Color { Color__Name = "Gray", Color__Description = "A neutral color that complements various palettes." },
                    new Color { Color__Name = "Pink", Color__Description = "A soft and playful color, often used in casual designs." },
                    new Color { Color__Name = "Purple", Color__Description = "A royal and luxurious color that adds depth." },
                    new Color { Color__Name = "Brown", Color__Description = "A warm and earthy color, suitable for classic styles." },
                    new Color { Color__Name = "Orange", Color__Description = "A lively and energetic color that catches attention." },
                    new Color { Color__Name = "Beige", Color__Description = "A subtle and versatile color for understated elegance." },
                    new Color { Color__Name = "Burgundy", Color__Description = "A deep red color that exudes sophistication." },
                    new Color { Color__Name = "Navy", Color__Description = "A dark shade of blue, offering a refined look." },
                    new Color { Color__Name = "Olive", Color__Description = "A muted green color, popular in streetwear." },
                    new Color { Color__Name = "Teal", Color__Description = "A blend of blue and green, providing a unique hue." },
                    new Color { Color__Name = "Maroon", Color__Description = "A rich, dark red color, often used in premium designs." },
                    new Color { Color__Name = "Turquoise", Color__Description = "A bright blue-green color that stands out." },
                    new Color { Color__Name = "Gold", Color__Description = "A metallic color symbolizing luxury and prestige." },
                    new Color { Color__Name = "Silver", Color__Description = "A sleek metallic color, adding a modern touch." }
                };
        try
        {
            var color=uow.Color.GetAll();
            if(color==null || !color.Any()){
                uow.Color.AddRange(colors);
            }
            System.Console.WriteLine("created 20 color s"); 
         }
        catch (System.Exception)
        {        
            System.Console.WriteLine("faild to create Color");
        }
    }
    public static async Task InitBrand(IUnitOfWork uow){

            var brandProducts = new Dictionary<string, List<string>>
            {
                { "Nike", new List<string> { "Air Force 1", "Air Jordan 1", "Air Max 90", "Dunk Low", "React Infinity Run", "Blazer Mid '77", "Pegasus 40", "LeBron 21", "ZoomX Vaporfly 3", "Metcon 9" } },
                { "Adidas", new List<string> { "Ultraboost 23", "Yeezy Boost 350", "Samba OG", "Gazelle", "NMD R1", "Superstar", "Forum Low", "Adizero Adios Pro 3", "Predator Accuracy", "Ozweego" } },
                { "Puma", new List<string> { "RS-X", "Suede Classic", "Cali Star", "Future Rider", "Deviate Nitro 2", "Clyde All-Pro", "Smash v2", "Velophasis", "Cell Endura", "Basket Classic" } },
                { "Reebok", new List<string> { "Club C 85", "Classic Leather", "Nano X3", "Pump Omni Zone II", "Zig Kinetica 2.5", "Floatride Energy 5", "Instapump Fury", "DMX Run 10", "Royal Glide", "Shaq Attaq" } },
                { "Converse", new List<string> { "Chuck Taylor All Star", "One Star", "Run Star Hike", "Jack Purcell", "Chuck 70", "Pro Leather", "Weapon CX", "Star Player", "Fastbreak Pro", "All Star BB Prototype CX" } },
                { "New Balance", new List<string> { "574", "997H", "990v6", "550", "327", "1080v13", "FuelCell SuperComp Elite", "Fresh Foam X More", "Made in USA 998", "2002R" } },
                { "Vans", new List<string> { "Old Skool", "Authentic", "Slip-On", "Sk8-Hi", "Era", "UltraRange EXO", "Chukka Boot", "Half Cab", "Knu Skool", "Style 36" } },
                { "Under Armour", new List<string> { "Curry 11", "HOVR Machina 3", "Project Rock 6", "HOVR Phantom 3", "Flow Velociti Wind 2", "Surge 3", "TriBase Reign 5", "Spawn 5", "Charged Commit TR 3", "UA SlipSpeed" } },
                { "Balenciaga", new List<string> { "Triple S", "Speed Trainer", "Track Sneaker", "Defender", "Runner Sneaker", "X-Pander", "Tyrex Sneaker", "Bulldozer Boot", "3XL Sneaker", "Fossil Sneaker" } },
                { "Gucci", new List<string> { "Ace Sneaker", "Rhyton Sneaker", "Tennis 1977", "Screener Sneaker", "Flashtrek Sneaker", "Basket Sneaker", "Run Sneaker", "Ultrapace", "Hacker Project Sneaker", "GG Supreme Sneaker" } },
                { "Louis Vuitton", new List<string> { "LV Trainer", "Tattoo Sneaker", "LV Runner Tatic", "Frontrow Sneaker", "Time Out Sneaker", "Rivoli Sneaker", "Skate Sneaker", "Run Away Sneaker", "Oberkampf Sneaker", "LV Skate" } },
                { "Fila", new List<string> { "Disruptor II", "Ray Tracer", "Grant Hill 2", "Mindblower", "MB Mesh Sneaker", "Renno Sneaker", "Orbit Zeppa", "Original Fitness", "Teratach 600", "Cage Basketball Shoe" } },
                { "ASICS", new List<string> { "Gel-Kayano 30", "Gel-Nimbus 26", "Gel-Cumulus 25", "Novablast 4", "Metaspeed Sky+", "GT-2000 11", "Gel-Venture 9", "Magic Speed 3", "Hyper Speed 2", "Gel-Sonoma 7" } },
                { "Mizuno", new List<string> { "Wave Rider 27", "Wave Inspire 19", "Rebula Cup", "Morelia Neo III", "Wave Sky 7", "Wave Prophecy X", "Wave Mujin 9", "Wave Shadow 5", "Wave Daichi 7", "Wave Creation 25" } },
                { "Saucony", new List<string> { "Kinvara 14", "Endorphin Speed 3", "Triumph 21", "Ride 16", "Guide 16", "Peregrine 13", "Hurricane 23", "Omni 21", "Freedom 5", "Endorphin Elite" } },
                { "Hoka One One", new List<string> { "Clifton 9", "Bondi 8", "Rincon 3", "Speedgoat 5", "Mach 5", "Tecton X", "Arahi 6", "Torrent 3", "Gaviota 5", "Challenger ATR 7" } },
                { "Salomon", new List<string> { "Speedcross 6", "X Ultra 4", "S/Lab Ultra 3", "Supercross 4", "XA Pro 3D V9", "Genesis Trail", "Thundercross", "Pulsar Trail", "Predict Hike Mid", "Outpulse GTX" } },
                { "Timberland", new List<string> { "6-Inch Premium Boot", "Euro Hiker", "Field Boot", "Radford Boot", "Mt. Maddsen Mid", "Skyla Bay", "Courmayeur Valley", "Garrison Trail", "Linden Woods", "Sprint Trekker" } },
                { "On Running", new List<string> { "Cloudmonster", "Cloud X 3", "Cloudswift 3", "Cloudrunner", "Cloudflow 4", "Cloudboom Echo 3", "Cloudultra 2", "Cloudsurfer", "Cloud 5", "Cloudtrax" } }
            };
            var brands = new List<Brand>
            {
                new() { Brand__Name = "Nike", Brand__Description = "One of the world's leading sports brands, known for Air Jordan, Air Max, Dunk, and more.", Brand__Logo = "nike_logo.png", Brand__IsActive = true},
                new() { Brand__Name = "Adidas", Brand__Description = "A famous footwear brand with iconic models like Ultraboost, Yeezy, and Stan Smith.", Brand__Logo = "adidas_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Puma", Brand__Description = "One of the largest sports brands worldwide, featuring RS-X, Suede Classic, and more.", Brand__Logo = "puma_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Reebok", Brand__Description = "A subsidiary of Adidas, known for training, running, and classic-style shoes.", Brand__Logo = "reebok_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Converse", Brand__Description = "A legendary sneaker brand famous for Chuck Taylor All Star, One Star, and more.", Brand__Logo = "converse_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "New Balance", Brand__Description = "Renowned for high-quality running shoes, including 574, 997, and 990 series.", Brand__Logo = "newbalance_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Vans", Brand__Description = "A top skateboarding brand known for Old Skool, Slip-On, and Sk8-Hi models.", Brand__Logo = "vans_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Under Armour", Brand__Description = "A sportswear giant known for Curry and HOVR basketball and running shoes.", Brand__Logo = "underarmour_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Balenciaga", Brand__Description = "A luxury fashion brand famous for the Triple S and Speed Trainer sneakers.", Brand__Logo = "balenciaga_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Gucci", Brand__Description = "A high-end fashion house with stylish sneakers like Ace and Rhyton.", Brand__Logo = "gucci_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Louis Vuitton", Brand__Description = "A luxury brand producing premium sneakers with an elegant aesthetic.", Brand__Logo = "louisvuitton_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Fila", Brand__Description = "A sportswear brand known for trendy sneakers like Disruptor and Ray Tracer.", Brand__Logo = "fila_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "ASICS", Brand__Description = "A leading running shoe company, famous for the Gel-Kayano and Gel-Nimbus series.", Brand__Logo = "asics_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Mizuno", Brand__Description = "A Japanese brand excelling in sports footwear, especially in soccer and running.", Brand__Logo = "mizuno_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Saucony", Brand__Description = "A premium running shoe brand offering models like Kinvara and Triumph.", Brand__Logo = "saucony_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Hoka One One", Brand__Description = "A running shoe company known for its thick, cushioned sole designs.", Brand__Logo = "hoka_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Salomon", Brand__Description = "An outdoor footwear brand specializing in trail running shoes like Speedcross.", Brand__Logo = "salomon_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "Timberland", Brand__Description = "A famous boot brand known for the classic Timberland 6-inch Boot.", Brand__Logo = "timberland_logo.png", Brand__IsActive = true },
                new() { Brand__Name = "On Running", Brand__Description = "A sports shoe brand featuring CloudTec sole technology for better performance.", Brand__Logo = "onrunning_logo.png", Brand__IsActive = true },
            };

            // Kiểm tra nếu danh sách Brand chưa tồn tại
            foreach (var br in brands){
                var products=new List<Product>();
                foreach (var p in brandProducts[br.Brand__Name])
                {
                    products.Add(
                        new Product {
                            Product__Name =br.Brand__Name+" "+ p,
                            Product__Description = $"A premium {br.Brand__Name} sneaker: {p}.",
                            Product__BrandId = br.Brand__Id,
                            Product__CreatedByAccountId = 11,
                            Product__Status = (int)Status.Unreleased,
                            Product__CreatedDate = RandomDateThisMonth()
                        }
                    );
                }
                
                br.Products=products;
            }
            var brand=await uow.Brand.GetAllAsync();
            if (brand==null || !brand.Any())
            {
                uow.Brand.AddRange(brands);
                Console.WriteLine("Successfully inserted 20 brands!");
            }
            else
            {
                Console.WriteLine("Brands already exist in the database.");
            }
    }
    public static async Task InitializeAccount(IServiceProvider serviceProvider,IUnitOfWork _uow)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityAccount>>();
         var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
         string[] roleNames = { RolesName.Admin, RolesName.Customer, RolesName.Manager,RolesName.Staff};
          foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }
        for (int i = 1; i < 100; i++)
        {   
            if(i==1){
                if (await userManager.FindByEmailAsync("admin@gmail.com") == null)
                {
                    var user = new IdentityAccount
                    {
                        UserName = "admin@gmail.com",
                        Email = "admin@gmail.com",
                        EmailConfirmed = true
                    };

                    var result= await userManager.CreateAsync(user, "Admin@123"); // Mật khẩu mặc định
                    if(result.Succeeded){
                        await userManager.AddToRoleAsync(user,"Admin");
                        _uow.StaffInfo.Add(new StaffInfo{
                            StaffInfo__AccountId=user.Id,
                            StaffInfo__Avatar=HandleString.DefaultImage,
                            StaffInfo__FirstName="Staff",
                            StaffInfo__LastName="Account"+i,
                            StaffInfo__Phone="03824567"+ (i<10?"0"+i:i.ToString())
                            
                        });
                    }
            }
            }
            string email = $"user{i}@gmail.com";
            if(await userManager.FindByEmailAsync(email)!=null){
                break;
            }
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityAccount
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

               var result= await userManager.CreateAsync(user, "User@123"); // Mật khẩu mặc định
                if(result.Succeeded){
                    if(i%10==0){
                        await userManager.AddToRoleAsync(user,"Staff");
                        _uow.StaffInfo.Add(new StaffInfo{
                            StaffInfo__AccountId=user.Id,
                            StaffInfo__Avatar=HandleString.DefaultImage,
                            StaffInfo__FirstName="Staff",
                            StaffInfo__LastName="Account"+i,
                            StaffInfo__Phone="01234567"+ (i<10?"0"+i:i.ToString())
                            
                        });
                    }
                    else{
                        await userManager.AddToRoleAsync(user,"Customer");
                         _uow.CustomerInfo.Add(new CustomerInfo{
                            CustomerInfo__SpendingPoint=0,
                            CustomerInfo__TotalSpent=0,
                            CustomerInfo__AccountId=user.Id,
                            CustomerInfo__FirstName="Customer",
                            CustomerInfo__LastName="Account"+i,
                            CustomerInfo__Phone="01234567"+ (i<10?"0"+i:i.ToString()),
                            CustomerInfo__Avatar=HandleString.DefaultImage
                        });
                        }
                }
            }

        }
    }

}
}