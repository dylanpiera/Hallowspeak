using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hallowspeak.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Hallowspeak.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private NavigationManager NavigationManager { get; set; }
        private DiscordClientCredentials DiscordClientCredentials { get; set; }
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(NavigationManager navigationManager, DiscordClientCredentials discordClientCredentials, ILogger<LoginModel> logger)
        {
            NavigationManager = navigationManager;
            DiscordClientCredentials = discordClientCredentials;
            _logger = logger;
        }

        public string ReturnUrl { get; set; }
        public async Task<IActionResult> OnGetAsync(string code)
        {
            string returnUrl = Url.Content("~/");
            try
            {
                // Clear the existing external cookie
                await HttpContext
                    .SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch { }


            // *** !!! This is where you would validate the user !!! ***
            // In this example we just log the user in
            // (Always log the user in for this demo)
            _logger.LogDebug("Test!");
            DiscordUser discordUser = await LoginUser(code);


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, discordUser.Username),
                new Claim(ClaimTypes.Role, "Administrator"),
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                RedirectUri = this.Request.Host.Value
            };
            try
            {
                await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return LocalRedirect(returnUrl);
        }

        private async Task<DiscordUser> LoginUser(string code)
        {
            _logger.LogDebug("Test!");
            try
            {
                /*Get Access Token from authorization code by making http post request*/
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://discordapp.com/api/oauth2/token");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                string parameters = "client_id=" + DiscordClientCredentials.ClientID + "&client_secret=" + DiscordClientCredentials.ClientSecret + "&grant_type=authorization_code&code=" + code + "&redirect_uri=" + $"{Request.Scheme}://{Request.Host}{Request.Path}";
                _logger.LogDebug(parameters);
                byte[] byteArray = Encoding.UTF8.GetBytes(parameters);
                webRequest.ContentLength = byteArray.Length;

                Stream postStream = await webRequest.GetRequestStreamAsync();

                await postStream.WriteAsync(byteArray, 0, byteArray.Length);
                postStream.Close();
                //TODO: look into this close method ^

                WebResponse response = await webRequest.GetResponseAsync();
                postStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(postStream);
                string responseFromServer = await reader.ReadToEndAsync();

                string tokenInfo = responseFromServer.Split(',')[0].Split(':')[1];
                string access_token = tokenInfo.Trim().Substring(1, tokenInfo.Length - 3);
                /*End*/

                /*Do http get request to the url to get the cleint info in json*/
                HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create("https://discordapp.com/api/users/@me");
                webRequest1.Method = "Get";
                webRequest1.ContentLength = 0;
                webRequest1.Headers.Add("Authorization", "Bearer " + access_token);
                webRequest1.ContentType = "application/x-www-form-urlencoded";

                string json = "";
                using (HttpWebResponse response1 = await webRequest1.GetResponseAsync() as HttpWebResponse)
                {
                    StreamReader reader1 = new StreamReader(response1.GetResponseStream());
                    json = await reader1.ReadToEndAsync();
                }
                /*End*/

                var dictionary = (JsonConvert.DeserializeObject<Dictionary<string, object>>(json));
                DiscordUser user = new DiscordUser() { UserID = (string)dictionary["id"], Username = (string)dictionary["username"] };
                return user;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return new DiscordUser();
        }
    }
}