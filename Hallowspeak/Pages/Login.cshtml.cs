using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hallowspeak.Data.Models;
using Hallowspeak.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Hallowspeak.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private DatabaseHelper DatabaseHelper { get; set; }
        private DiscordClientCredentials DiscordClientCredentials { get; set; }
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(DatabaseHelper databaseHelper, DiscordClientCredentials discordClientCredentials, ILogger<LoginModel> logger)
        {
            DatabaseHelper = databaseHelper;
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

            /* Prepares data for Cookie */
            DiscordUser discordUser = await LoginUser(code);

            /* Fetches AuthLevel from the DB if it exists. */
            #region UserLevel
            if (DatabaseHelper.Enabled)
            {
                MySqlConnection conn = DatabaseHelper.GetConnection();
                try
                {
                    await conn.OpenAsync();

                    MySqlCommand cmd = new MySqlCommand("SELECT `authlevel` FROM `Users` WHERE `uuid` = @uuid", conn);
                    cmd.Parameters.AddWithValue("@uuid", discordUser.UserID);

                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int authlevel = 0;

                        while (await reader.ReadAsync())
                        {
                            authlevel = reader.GetInt32(0);
                        }

                        discordUser.AuthLevel = (AuthLevel) authlevel;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
                finally
                {
                    await conn.CloseAsync();
                }
            }
            else
            {
                discordUser.AuthLevel = AuthLevel.Administrator;
            }
            #endregion

            /* Prepares the necessary claims for AspNetCore Cookie-based Identity */
            #region AspNetCore Authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, discordUser.Username),
                new Claim(ClaimTypes.Role, discordUser.AuthLevel.ToString()),
                new Claim(ClaimTypes.NameIdentifier, discordUser.UserID)
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
            #endregion

            return LocalRedirect(returnUrl);
        }

        /// <summary>
        /// Oauth Discord Login
        /// </summary>
        /// <param name="code">oAuth Return Code from Discord</param>
        /// <returns>Data from the logged in User</returns>
        private async Task<DiscordUser> LoginUser(string code)
        {
            _logger.LogDebug("Test!");
            try
            {
                /*Get Access Token from authorization code by making http post request*/
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://discordapp.com/api/oauth2/token");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                string parameters = "client_id=" + DiscordClientCredentials.ClientID + "&client_secret=" + DiscordClientCredentials.ClientSecret + "&grant_type=authorization_code&code=" + code + "&redirect_uri=" + $"https://{Request.Host}{Request.Path}";
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