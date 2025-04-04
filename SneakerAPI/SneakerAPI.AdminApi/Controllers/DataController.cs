using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.AdminApi.Controllers.ProductControllers
{   
public static class SeedRoleAdmin
{   
    public static void InitProductColorSize(IUnitOfWork uow){
        var pcs=uow.ProductColorSize.GetAll();
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
                            Product__Name = p,
                            Product__Description = $"A premium {br.Brand__Name} sneaker: {p}.",
                            Product__BrandId = br.Brand__Id,
                            Product__CreatedByAccountId = 16,
                            Product__Status = (int)Status.Unreleased,
                            Product__CreatedDate = DateTime.UtcNow
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
    public static async Task InitializeAccount(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityAccount>>();
         var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
         string[] roleNames = { "Admin", "Customer", "Manager" ,"Staff"};
          foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }
        for (int i = 1; i <= 100; i++)
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
                    if(i%10==0)
                        await userManager.AddToRoleAsync(user,"Staff");
                    else
                        await userManager.AddToRoleAsync(user,"Customer");
                }
            }

        }
    }

}
}