using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.QQ
{
    internal class QQHandler : OAuthHandler<QQOptions>
    {
        private readonly ILogger _logger;
        public QQHandler(IOptionsMonitor<QQOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _logger = logger.CreateLogger(nameof(QQHandler));
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(
            ClaimsIdentity identity,
            AuthenticationProperties properties,
            OAuthTokenResponse tokens)
        {
            var openId = await GetOpenIdAsync(tokens);
            var userInfo = await GetUserInfoAsync(tokens, openId);

            _logger.LogDebug("qq userinfo:" + userInfo);

            using var payload = JsonDocument.Parse(userInfo);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, openId, Options.ClaimsIssuer)); // 必须
            identity.AddClaim(new Claim("urn:qq:openid", openId, Options.ClaimsIssuer));  //
            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payload.RootElement);
            context.RunClaimActions();
            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }


        /// <summary>
        /// 通过Authorization Code获取Access Token。
        /// QQ这一步用的是Get请求，base中用的是Post
        /// </summary>
        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {
            // https://wiki.connect.qq.com/%E4%BD%BF%E7%94%A8authorization_code%E8%8E%B7%E5%8F%96access_token Step2：通过Authorization Code获取Access Token
            var response = await Backchannel.GetAsync(QueryHelpers.AddQueryString(Options.TokenEndpoint, new Dictionary<string, string>
            {
                {  "grant_type", "authorization_code" },
                {  "client_id", Options.ClientId },
                {  "client_secret", Options.ClientSecret },
                {  "code", context.Code},
                {  "redirect_uri", context.RedirectUri},
                {  "fmt","json" } //可选参数，因历史原因，默认是x-www-form-urlencoded格式，指定为json返回json格式
            }), Context.RequestAborted);

            if (!response.IsSuccessStatusCode)
            {
                var error = "OAuth token endpoint failure: " + await Display(response);
                return OAuthTokenResponse.Failed(new Exception(error));
            }

            var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return OAuthTokenResponse.Success(payload);
        }


        //通过AccessToken获取用户OpenId
        protected async Task<string> GetOpenIdAsync(OAuthTokenResponse tokens)
        {
            // https://wiki.connect.qq.com/%E8%8E%B7%E5%8F%96%E7%94%A8%E6%88%B7openid_oauth2-0 
            var response = await Backchannel.GetAsync(QueryHelpers.AddQueryString(Options.OpenIdEndpoint, new Dictionary<string, string>
            {
                { "access_token", tokens.AccessToken },
                { "fmt","json"}//可选参数，因历史原因，默认是jsonpb格式，指定为json返回json格式
            }), Context.RequestAborted);

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogDebug($"获取QQConnect的OpenId失败：\n{await Display(response)}");
                throw new HttpRequestException($"获取QQConnect的OpenId失败：{response.StatusCode}，请检查access_token是正确。");
            }

            var responseContext = await response.Content.ReadAsStringAsync();
            using var jsonDocument = JsonDocument.Parse(responseContext);
            return jsonDocument.RootElement.GetProperty("openid").GetString();
        }
        //获取用户信息
        protected async Task<string> GetUserInfoAsync(OAuthTokenResponse tokens, string openId)
        {
            // https://wiki.connect.qq.com/openapi%E8%B0%83%E7%94%A8%E8%AF%B4%E6%98%8E_oauth2-0
            var response = await Backchannel.GetAsync(QueryHelpers.AddQueryString(Options.UserInformationEndpoint, new Dictionary<string, string>
            {
                {  "openid", openId},
                {  "access_token", tokens.AccessToken },
                {  "oauth_consumer_key", Options.ClientId }
            }), Context.RequestAborted);

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogDebug($"获取QQConnect的用户信息失败：\n{await Display(response)}");
                throw new HttpRequestException($"获取QQConnect的用户信息失败：{response.StatusCode}，请检查参数是否正确。");
            }

            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<string> Display(HttpResponseMessage response)
        {
            var output = new StringBuilder();
            output.Append("Status: " + response.StatusCode + ";");
            output.Append("Headers: " + response.Headers.ToString() + ";");
            output.Append("Body: " + await response.Content.ReadAsStringAsync() + ";");
            return output.ToString();
        }
    }
}
