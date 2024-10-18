using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GitRecon.Models;

namespace GitRecon.Controllers
{
    public class SocialAccountFinderController
    {
        public List<AccountResult> ResultList { get; private set; } = new List<AccountResult>();

        private readonly Dictionary<string, WebsiteInfo> websites = new Dictionary<string, WebsiteInfo>
        {
            {"About.me", new WebsiteInfo {ErrorType = "status_code", Url = "https://about.me/{}"}},
            {"Chess", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.chess.com/member/{}"}},
            {"DailyMotion", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.dailymotion.com/{}"}},
            {"Docker Hub", new WebsiteInfo {ErrorType = "status_code", Url = "https://hub.docker.com/u/{}"}},
            {"Duolingo", new WebsiteInfo {ErrorType = "message", ErrorMessage = "Duolingo - Learn a language for free @duolingo", Url = "https://www.duolingo.com/profile/{}"}},
            {"Fiverr", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.fiverr.com/{}"}},
            {"Flickr", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.flickr.com/people/{}"}},
            {"GeeksforGeeks", new WebsiteInfo {ErrorType = "message", ErrorMessage = "Login GeeksforGeeks", Url = "https://auth.geeksforgeeks.org/user/{}"}},
            {"Genius (Artists)", new WebsiteInfo {ErrorType = "status_code", Url = "https://genius.com/artists/{}"}},
            {"Genius (Users)", new WebsiteInfo {ErrorType = "status_code", Url = "https://genius.com/{}"}},
            {"Giphy", new WebsiteInfo {ErrorType = "status_code", Url = "https://giphy.com/{}"}},
            {"GitHub", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.github.com/{}"}},
            {"Imgur", new WebsiteInfo {ErrorType = "status_code", Url = "https://api.imgur.com/account/v1/accounts/{}?client_id=546c25a59c58ad7"}},
            {"Minecraft", new WebsiteInfo {ErrorType = "status_code", Url = "https://api.mojang.com/users/profiles/minecraft/{}"}},
            {"npm", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.npmjs.com/~{}"}},
            {"Pastebin", new WebsiteInfo {ErrorType = "status_code", Url = "https://pastebin.com/u/{}"}},
            {"Patreon", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.patreon.com/{}"}},
            {"PyPi", new WebsiteInfo {ErrorType = "status_code", Url = "https://pypi.org/user/{}"}},
            {"Reddit", new WebsiteInfo {ErrorType = "message", ErrorMessage = "\"error\": 404}", Url = "https://www.reddit.com/user/{}/about.json"}},
            {"Replit", new WebsiteInfo {ErrorType = "status_code", Url = "https://replit.com/@{}"}},
            {"Roblox", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.roblox.com/user.aspx?username={}"}},
            {"RootMe", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.root-me.org/{}"}},
            {"Scribd", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.scribd.com/{}"}},
            {"Snapchat", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.snapchat.com/add/{}"}},
            {"SoundCloud", new WebsiteInfo {ErrorType = "status_code", Url = "https://soundcloud.com/{}"}},
            {"SourceForge", new WebsiteInfo {ErrorType = "status_code", Url = "https://sourceforge.net/u/{}"}},
            {"Spotify", new WebsiteInfo {ErrorType = "status_code", Url = "https://open.spotify.com/user/{}"}},
            {"Steam", new WebsiteInfo {ErrorType = "message", ErrorMessage = "Steam Community :: Error", Url = "https://steamcommunity.com/id/{}"}},
            {"Telegram", new WebsiteInfo {ErrorType = "message", ErrorMessage = "<meta name=\"robots\" content=\"noindex, nofollow\">", Url = "https://t.me/{}"}},
            {"Tenor", new WebsiteInfo {ErrorType = "status_code", Url = "https://tenor.com/users/{}"}},
            {"TryHackMe", new WebsiteInfo {ErrorType = "message", ErrorMessage = "<title>TryHackMe</title>", Url = "https://tryhackme.com/p/{}"}},
            {"Vimeo", new WebsiteInfo {ErrorType = "status_code", Url = "https://vimeo.com/{}"}},
            {"Wattpad", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.wattpad.com/user/{}"}},
            {"Wikipedia", new WebsiteInfo {ErrorType = "message", ErrorMessage = "(centralauth-admin-nonexistent:", Url = "https://en.wikipedia.org/wiki/Special:CentralAuth/{}?uselang=qqx"}},
            {"AllMyLinks", new WebsiteInfo {ErrorType = "status_code", Url = "https://allmylinks.com/{}"}},
            {"Buy Me a Coffee", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.buymeacoffee.com/{}"}},
            {"BuzzFeed", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.buzzfeed.com/{}"}},
            {"Cash APP", new WebsiteInfo {ErrorType = "status_code", Url = "https://cash.app/${}"}},
            {"Ebay", new WebsiteInfo {ErrorType = "message", Url = "https://www.ebay.com/usr/{}"}},
            {"Instagram", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.picuki.com/profile/{}"}},
            {"JsFiddle", new WebsiteInfo {ErrorType = "status_code", Url = "https://jsfiddle.net/user/{}/"}},
            {"Linktree", new WebsiteInfo {ErrorType = "message", ErrorMessage = "\"statusCode\":404", Url = "https://linktr.ee/{}"}},
            {"Medium", new WebsiteInfo {ErrorType = "message", ErrorMessage = "<span class=\"fs\">404</span>", Url = "https://{}.medium.com/about"}},
            {"Pinterest", new WebsiteInfo {ErrorType = "message", ErrorMessage = "<title></title>", Url = "https://pinterest.com/{}/"}},
            {"Rapid API", new WebsiteInfo {ErrorType = "status_code", Url = "https://rapidapi.com/user/{}"}},
            {"TradingView", new WebsiteInfo {ErrorType = "status_code", Url = "https://www.tradingview.com/u/{}/"}},
        };

        public async Task<List<AccountResult>> CheckWebsitesAsync(string username, Action<string> updateStatus, Action<string> addResult)
        {
            ResultList.Clear();
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(8);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

                var tasks = new List<Task>();
                var semaphore = new SemaphoreSlim(10); // Control concurrency

                foreach (var website in websites)
                {
                    updateStatus?.Invoke($"Checking {website.Key} for {username}...");
                    await semaphore.WaitAsync();
                    var task = Task.Run(async () =>
                    {
                        var result = await CheckWebsiteAsync(username, website.Key, website.Value, httpClient);
                        if (result != null)
                        {
                            addResult?.Invoke(result.ProfileUrl);
                        }
                        semaphore.Release();
                    });
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
            }
            return ResultList;
        }

        private async Task<AccountResult> CheckWebsiteAsync(string username, string websiteName, WebsiteInfo websiteInfo, HttpClient httpClient)
        {
            try
            {
                var url = websiteInfo.Url.Replace("{}", username);
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return new AccountResult { ProfileUrl = url };
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions if needed
            }
            return null;
        }
    }
}
