using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Authentication.QQ
{
    internal class QQHandler : OAuthHandler<QQOptions>
    {
        public QQHandler(IOptionsMonitor<QQOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(
            ClaimsIdentity identity,
            AuthenticationProperties properties,
            OAuthTokenResponse tokens)
        {
            //获取OpenId
            var openIdEndpoint = QueryHelpers.AddQueryString(Options.OpenIdEndpoint, "access_token", tokens.AccessToken);
            var openIdResponse = await Backchannel.GetAsync(openIdEndpoint, Context.RequestAborted);
            if (!openIdResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"未能检索QQ Connect的OpenId(返回状态码:{openIdResponse.StatusCode})，请检查access_token是正确。");
            }
            var json = JObject.Parse(企鹅的返回不拘一格传入这里转换为JSON(await openIdResponse.Content.ReadAsStringAsync()));
            var openId = GetOpenId(json);

            //获取用户信息
            var parameters = new Dictionary<string, string>
            {
                {  "openid", openId},
                {  "access_token", tokens.AccessToken },
                {  "oauth_consumer_key", Options.ClientId }
            };
            var userInformationEndpoint = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, parameters);
            var userInformationResponse = await Backchannel.GetAsync(userInformationEndpoint, Context.RequestAborted);
            if (!userInformationResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"未能检索QQ Connect的用户信息(返回状态码:{userInformationResponse.StatusCode})，请检查参数是否正确。");
            }

            var payload = JObject.Parse(await userInformationResponse.Content.ReadAsStringAsync());
            //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, openId, ClaimValueTypes.String, Options.ClaimsIssuer));
            payload.Add("openid", openId);
            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payload);
            context.RunClaimActions();
            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }
 
        /// <summary>
        /// 通过Authorization Code获取Access Token。
        /// 重写改方法，QQ这一步用的是Get请求，base中用的是Post
        /// </summary>
        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri)
        {
            var parameters = new Dictionary<string, string>
            {
                {  "grant_type", "authorization_code" },
                {  "client_id", Options.ClientId },
                {  "client_secret", Options.ClientSecret },
                {  "code", code},
                {  "redirect_uri", redirectUri}
            };

            var endpoint = QueryHelpers.AddQueryString(Options.TokenEndpoint, parameters);

            var response = await Backchannel.GetAsync(endpoint, Context.RequestAborted);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                result = "{\"" + result.Replace("=", "\":\"").Replace("&", "\",\"") + "\"}";
                var payload = JObject.Parse(result);
                return OAuthTokenResponse.Success(payload);
            }
            //else if (response.StatusCode == System.Net.HttpStatusCode.Found || response.StatusCode == System.Net.HttpStatusCode.MovedPermanently)
            //{
            //QQ Connect接口调用有错误时，会返回code和msg字段，以url参数对的形式返回，value部分会进行url编码（UTF-8)。
            //http://wiki.connect.qq.com/%E4%BD%BF%E7%94%A8authorization_code%E8%8E%B7%E5%8F%96access_token
            //如果这样的话，那应该是发生了301或302重定向操作
            //要取得出错的具体错误提示代码，只能在重定向后的Url中截取
            //    var newUrl = response.Headers.Location;

            //    var error = "OAuth token endpoint failure: ";// + await Display(response);
            //    return OAuthTokenResponse.Failed(new Exception(error));
            //}
            else
            {
                return OAuthTokenResponse.Failed(new Exception("获取QQ Connect Access Token出错。"));
            }
        }

        private static string 企鹅的返回不拘一格传入这里转换为JSON(string text)
        {
            return new System.Text.RegularExpressions.Regex("callback\\((?<json>[ -~]+)\\);").Match(text).Groups["json"].Value;
        }

        private static string GetOpenId(JObject json) {
            return json.Value<string>("openid");
        }
    }
}
