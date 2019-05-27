using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using AspNetCore.Authentication.WeChat;

namespace Microsoft.AspNetCore.Authentication.WeChat
{
    internal class WeChatHandler : OAuthHandler<WeChatOptions>
    {

        private readonly ISecureDataFormat<AuthenticationProperties> _secureDataFormat;


        /// <summary>
        /// Called after options/events have been initialized for the handler to finish initializing itself.
        /// </summary>
        /// <returns>A task</returns>
        protected override async Task InitializeHandlerAsync()
        {
            await base.InitializeHandlerAsync();
            if (Options.UseCachedStateDataFormat)
            {
                Options.StateDataFormat = _secureDataFormat;
            }
            //set Scopes and AuthorizationEndpoint
            //Scope 表示应用授权作用域。
            //网页上登录（非微信浏览器）需要两个Scope，一个是UserInfo，一个是Login
            foreach (var app in Options.Apps)
            {
                if (app.AppType == WeChatApplicationType.OfficialAccount)
                {
                    if (app.AuthorizationEndpoint == null)
                    {
                        app.AuthorizationEndpoint = WeChatDefaults.AuthorizationEndpoint2;
                    }

                    if (app.Scopes == null || !app.Scopes.Any())
                    {
                        app.Scopes = new List<string>() { WeChatDefaults.UserInfoScope };
                    }
                }
                if (app.AppType == WeChatApplicationType.WebSite)
                {
                    if (app.AuthorizationEndpoint == null)
                    {
                        app.AuthorizationEndpoint = WeChatDefaults.AuthorizationEndpoint;
                    }
                    if (app.Scopes == null || !app.Scopes.Any())
                    {
                        app.Scopes = new List<string>() { WeChatDefaults.UserInfoScope, WeChatDefaults.LoginScope };
                    }
                }
            }
        }

        public WeChatHandler(
             IOptionsMonitor<WeChatOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ISecureDataFormat<AuthenticationProperties> secureDataFormat)
            : base(options, logger, encoder, clock)
        {
            _secureDataFormat = secureDataFormat;
        }
        /*
         * Challenge 盘问握手认证协议
         * 这个词有点偏，好多翻译工具都查不出。
         * 这个解释才是有些靠谱 http://abbr.dict.cn/Challenge/CHAP
         */
        /// <summary>
        /// 构建请求CODE的Url地址（这是第一步，准备工作）
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            //get current current application
            WeChatApplication currentApplication= Options.Apps.First();
            if (Options.IsWeChatBrowser(Request))
            {
                currentApplication = Options.Apps.FirstOrDefault(p => p.AppType == WeChatApplicationType.OfficialAccount) ?? currentApplication;
            }
            else
            {
                currentApplication = Options.Apps.FirstOrDefault(p => p.AppType == WeChatApplicationType.WebSite) ?? currentApplication;
            }
            var scope = FormatScope(currentApplication.AppId);
            properties.Items["appid"] = currentApplication.AppId;
            var state = Options.StateDataFormat.Protect(properties);

            if (!string.IsNullOrEmpty(Options.CallbackUrl))
            {
                redirectUri = Options.CallbackUrl;
            }
 
            var parameters = new Dictionary<string, string>()
            {
                { "appid", currentApplication.AppId },
                { "redirect_uri", redirectUri },
                { "response_type", "code" },
                //{ "scope", scope },
                //{ "state", state },
            };

            var ret = QueryHelpers.AddQueryString(
                currentApplication.AuthorizationEndpoint, parameters);
            //scope 不能被UrlEncode
            ret += $"&scope={scope}&state={state}";

            return ret;
        }

        /// <summary>
        /// 处理微信授权结果（接收微信授权的回调）
        /// </summary>
        /// <returns></returns>
        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            //第一步，处理工作
            AuthenticationProperties properties = null;
            var query = Request.Query;

            //微信只会发送code和state两个参数，不会返回错误消息
            //若用户禁止授权，则重定向后不会带上code参数，仅会带上state参数
            var code = query["code"];
            var state = query["state"];

            properties = Options.StateDataFormat.Unprotect(state);
            if (properties == null)
            {
                return HandleRequestResult.Fail("The oauth state was missing or invalid.");
            }

            // OAuth2 10.12 CSRF
            if (!ValidateCorrelationId(properties))
            {
                return HandleRequestResult.Fail("Correlation failed.");
            }

            if (StringValues.IsNullOrEmpty(code)) //code为null就是
            {
                return HandleRequestResult.Fail("Code was not found.");
            }

            //第二步，通过Code获取Access Token
            var redirectUrl = !string.IsNullOrEmpty(Options.CallbackUrl) ?
                Options.CallbackUrl :
                BuildRedirectUri(Options.CallbackPath);
            var tokens = await ExchangeCodeAsync(code, redirectUrl,properties.Items["appid"]);

            if (tokens.Error != null)
            {
                return HandleRequestResult.Fail(tokens.Error);
            }

            if (string.IsNullOrEmpty(tokens.AccessToken))
            {
                return HandleRequestResult.Fail("Failed to retrieve access token.");
            }

            var identity = new ClaimsIdentity(ClaimsIssuer);

            if (Options.SaveTokens)
            {
                var authTokens = new List<AuthenticationToken>();

                authTokens.Add(new AuthenticationToken { Name = "access_token", Value = tokens.AccessToken });
                if (!string.IsNullOrEmpty(tokens.RefreshToken))
                {
                    authTokens.Add(new AuthenticationToken { Name = "refresh_token", Value = tokens.RefreshToken });
                }

                if (!string.IsNullOrEmpty(tokens.TokenType)) //微信就没有这个
                {
                    authTokens.Add(new AuthenticationToken { Name = "token_type", Value = tokens.TokenType });
                }

                if (!string.IsNullOrEmpty(tokens.ExpiresIn))
                {
                    int value;
                    if (int.TryParse(tokens.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                    {
                        // https://www.w3.org/TR/xmlschema-2/#dateTime
                        // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
                        var expiresAt = Clock.UtcNow + TimeSpan.FromSeconds(value);
                        authTokens.Add(new AuthenticationToken
                        {
                            Name = "expires_at",
                            Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                        });
                    }
                }

                properties.StoreTokens(authTokens);
            }

            var ticket = await CreateTicketAsync(identity, properties, tokens);
            if (ticket != null)
            {
                return HandleRequestResult.Success(ticket);
            }
            else
            {
                return HandleRequestResult.Fail("Failed to retrieve user information from remote server.");
            }
        }

        /// <summary>
        /// 通过Code获取Access Token(这是第二步) 
        /// </summary>
        protected  async Task<OAuthTokenResponse> ExchangeCodeAsync(string code, string redirectUri,string appId)
        {
            var parameters = new Dictionary<string, string>
            {
                {  "appid", appId },
                {  "secret",Options.Apps.First(p=>p.AppId==appId).AppSecret },
                {  "code", code},
                {  "grant_type", "authorization_code" }
            };

            var endpoint = QueryHelpers.AddQueryString(Options.TokenEndpoint, parameters);

            var response = await Backchannel.GetAsync(endpoint, Context.RequestAborted);
            if (response.IsSuccessStatusCode)
            {
                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                return OAuthTokenResponse.Success(payload);
            }
            else
            {
                return OAuthTokenResponse.Failed(new Exception("获取微信AccessToken出错。"));
            }
        }



        /// <summary>
        /// 创建身份票据(这是第三步) 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="properties"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        protected override async Task<AuthenticationTicket> CreateTicketAsync(
            ClaimsIdentity identity,
            AuthenticationProperties properties,
            OAuthTokenResponse tokens)
        {
            var openId = GetOpenId(tokens.Response);
            var unionId = GetUnionId(tokens.Response);

            var userInfo = new JObject();
            userInfo.Add("openid", openId);

            //微信获取用户信息是需要开通权限的，没有开通权限的只能用openId来标示用户
            if (!string.IsNullOrEmpty(unionId))
            {
                //获取用户信息
                var parameters = new Dictionary<string, string>
                {
                    {  "openid", openId},
                    {  "access_token", tokens.AccessToken },
                    {  "lang", "zh-CN" } //如果是多语言，这个参数该怎么获取？
                };
                var userInfoEndpoint = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, parameters);
                var userInfoResponse = await Backchannel.GetAsync(userInfoEndpoint, Context.RequestAborted);
                if (!userInfoResponse.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"未能获取到微信用户个人信息(返回状态码:{userInfoResponse.StatusCode})，请检查access_token是正确。");
                }

                userInfo = JObject.Parse(await userInfoResponse.Content.ReadAsStringAsync());
            }

            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, userInfo);
            context.RunClaimActions();
            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }

        private static string GetOpenId(JObject json)
        {
            return json.Value<string>("openid");
        }
        private static string GetUnionId(JObject json)
        {
            return json.Value<string>("unionid");
        }

        /// <summary>
        /// 构建Scope
        /// </summary>
        /// <returns></returns>
        protected  string FormatScope(string appId)
        {
            return string.Join(",",Options.Apps.First(p=>p.AppId==appId).Scopes);
        }
    }
}
