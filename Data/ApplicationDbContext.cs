using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OsintEyeWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<OsintEyeWeb.Models.EmailAuthor> EmailAuthors { get; set; } = default!;
        public DbSet<SocialMediaService> SocialMediaServices { get; set; }

      
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

               // check if databased is seeded
            if (await context.SocialMediaServices.AnyAsync())
            {
                return; // do not seed the database
            }

            var services = new List<SocialMediaService>
        {
             new SocialMediaService { Id = 1, Name = "About.me", ErrorType = "status_code", UrlTemplate = "https://about.me/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 2, Name = "Chess", ErrorType = "status_code", UrlTemplate = "https://www.chess.com/member/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 3, Name = "DailyMotion", ErrorType = "status_code", UrlTemplate = "https://www.dailymotion.com/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 4, Name = "Docker Hub", ErrorType = "status_code", UrlTemplate = "https://hub.docker.com/u/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 5, Name = "Duolingo", ErrorType = "message", UrlTemplate = "https://www.duolingo.com/profile/{}", ErrorMessage = "Duolingo - Learn a language for free @duolingo" },
                new SocialMediaService { Id = 6, Name = "Fiverr", ErrorType = "status_code", UrlTemplate = "https://www.fiverr.com/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 7, Name = "Flickr", ErrorType = "status_code", UrlTemplate = "https://www.flickr.com/people/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 8, Name = "GeeksforGeeks", ErrorType = "message", UrlTemplate = "https://auth.geeksforgeeks.org/user/{}", ErrorMessage = "Login GeeksforGeeks" },
                new SocialMediaService { Id = 9, Name = "Genius (Artists)", ErrorType = "status_code", UrlTemplate = "https://genius.com/artists/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 10, Name = "Genius (Users)", ErrorType = "status_code", UrlTemplate = "https://genius.com/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 11, Name = "Giphy", ErrorType = "status_code", UrlTemplate = "https://giphy.com/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 12, Name = "GitHub", ErrorType = "status_code", UrlTemplate = "https://www.github.com/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 13, Name = "Imgur", ErrorType = "status_code", UrlTemplate = "https://api.imgur.com/account/v1/accounts/{}?client_id=546c25a59c58ad7", ErrorMessage = "No error" },
                new SocialMediaService { Id = 14, Name = "Minecraft", ErrorType = "status_code", UrlTemplate = "https://api.mojang.com/users/profiles/minecraft/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 15, Name = "npm", ErrorType = "status_code", UrlTemplate = "https://www.npmjs.com/~{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 16, Name = "Pastebin", ErrorType = "status_code", UrlTemplate = "https://pastebin.com/u/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 17, Name = "Patreon", ErrorType = "status_code", UrlTemplate = "https://www.patreon.com/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 18, Name = "PyPi", ErrorType = "status_code", UrlTemplate = "https://pypi.org/user/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 19, Name = "Reddit", ErrorType = "message", UrlTemplate = "https://www.reddit.com/user/{}/about.json", ErrorMessage = "\"error\": 404)" },
                new SocialMediaService { Id = 20, Name = "Replit", ErrorType = "status_code", UrlTemplate = "https://replit.com/@{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 21, Name = "Roblox", ErrorType = "status_code", UrlTemplate = "https://www.roblox.com/user.aspx?username={}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 22, Name = "RootMe", ErrorType = "status_code", UrlTemplate = "https://www.root-me.org/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 23, Name = "Scribd", ErrorType = "status_code", UrlTemplate = "https://www.scribd.com/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 24, Name = "Snapchat", ErrorType = "status_code", UrlTemplate = "https://www.snapchat.com/add/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 25, Name = "SoundCloud", ErrorType = "status_code", UrlTemplate = "https://soundcloud.com/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 26, Name = "SourceForge", ErrorType = "status_code", UrlTemplate = "https://sourceforge.net/u/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 27, Name = "Spotify", ErrorType = "status_code", UrlTemplate = "https://open.spotify.com/user/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 28, Name = "Steam", ErrorType = "message", UrlTemplate = "https://steamcommunity.com/id/{}", ErrorMessage = "Steam Community :: Error" },
                new SocialMediaService { Id = 29, Name = "Telegram", ErrorType = "message", UrlTemplate = "https://t.me/{}", ErrorMessage = "<meta name=\"robots\" content=\"noindex, nofollow\">" },
                new SocialMediaService { Id = 30, Name = "Tenor", ErrorType = "status_code", UrlTemplate = "https://tenor.com/users/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 31, Name = "TryHackMe", ErrorType = "message", UrlTemplate = "https://tryhackme.com/p/{}", ErrorMessage = "<title>TryHackMe</title>" },
                new SocialMediaService { Id = 32, Name = "Vimeo", ErrorType = "status_code", UrlTemplate = "https://vimeo.com/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 33, Name = "Wattpad", ErrorType = "status_code", UrlTemplate = "https://www.wattpad.com/user/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 34, Name = "Wikipedia", ErrorType = "message", UrlTemplate = "https://en.wikipedia.org/wiki/Special:CentralAuth/{}?uselang=qqx", ErrorMessage = "(centralauth-admin-nonexistent:" },
                new SocialMediaService { Id = 35, Name = "AllMyLinks", ErrorType = "status_code", UrlTemplate = "https://allmylinks.com/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 36, Name = "Buy Me a Coffee", ErrorType = "status_code", UrlTemplate = "https://www.buymeacoffee.com/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 37, Name = "BuzzFeed", ErrorType = "status_code", UrlTemplate = "https://www.buzzfeed.com/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 38, Name = "Cash APP", ErrorType = "status_code", UrlTemplate = "https://cash.app/${}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 39, Name = "Ebay", ErrorType = "message", UrlTemplate = "https://www.ebay.com/usr/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 40, Name = "Instagram", ErrorType = "status_code", UrlTemplate = "https://www.picuki.com/profile/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 41, Name = "JsFiddle", ErrorType = "status_code", UrlTemplate = "https://jsfiddle.net/user/{}/", ErrorMessage = "No error" },
                new SocialMediaService { Id = 42, Name = "Linktree", ErrorType = "message", UrlTemplate = "https://linktr.ee/{}", ErrorMessage = "\"statusCode\":404" },
                new SocialMediaService { Id = 43, Name = "Medium", ErrorType = "message", UrlTemplate = "https://{}.medium.com/about", ErrorMessage = "<span class=\"fs\">404</span>" },
                new SocialMediaService { Id = 44, Name = "Pinterest", ErrorType = "message", UrlTemplate = "https://pinterest.com/{}/", ErrorMessage = "<title></title>" },
                new SocialMediaService { Id = 45, Name = "Rapid API", ErrorType = "status_code", UrlTemplate = "https://rapidapi.com/user/{}", ErrorMessage = "No error" },
                new SocialMediaService { Id = 46, Name = "TradingView", ErrorType = "status_code", UrlTemplate = "https://www.tradingview.com/u/{}/", ErrorMessage = "No error" }
        };

            await context.SocialMediaServices.AddRangeAsync(services);
            await context.SaveChangesAsync();
        }
    }
}
