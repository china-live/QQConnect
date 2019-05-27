using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Authentication.WeChat
{
    public class WeChatOptions : OAuthOptions
    {
        public static string UserInfoScope = "snsapi_userinfo";
        public static string LoginScope = "snsapi_login";

        public WeChatOptions()
        {
            CallbackPath = new PathString("/signin-wechat");
            AuthorizationEndpoint = WeChatDefaults.AuthorizationEndpoint;
            AuthorizationEndpoint2 = WeChatDefaults.AuthorizationEndpoint2;
            TokenEndpoint = WeChatDefaults.TokenEndpoint;
            UserInformationEndpoint = WeChatDefaults.UserInformationEndpoint;

            //Scope 表示应用授权作用域。
            //网页上登录（非微信浏览器）需要两个Scope，一个是UserInfo，一个是Login
            Scope.Add(UserInfoScope);
            Scope.Add(LoginScope);

            //微信内嵌浏览器Login只需要UserInfo
            Scope2 = new List<string>();
            Scope2.Add(UserInfoScope);

            //除了openid外，其余的都可能为空，因为微信获取用户信息是有单独权限的
            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "openid");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "nickname");
            ClaimActions.MapJsonKey(ClaimTypes.Gender, "sex",ClaimValueTypes.Integer);
            ClaimActions.MapJsonKey(ClaimTypes.Country, "country");//ClaimTypes.Locality
            ClaimActions.MapJsonKey("urn:wechat:province", "province");//ClaimTypes.StateOrProvince
            ClaimActions.MapJsonKey("urn:wechat:city", "city");//ClaimTypes.StreetAddress
            ClaimActions.MapJsonKey(ClaimTypes.Uri, "headimgurl");
            ClaimActions.MapCustomJson("urn:wechat:privilege", user =>  string.Join(",",user.SelectToken("privilege")?.Select(s => (string)s).ToArray() ?? new string[0]));
            ClaimActions.MapJsonKey("urn:wechat:unionid", "unionid");

            IsWeChatBrowser=(r) => r.IsWeChatBroswer();
        }


        /// <summary>
        /// 应用唯一标识，在微信开放平台提交应用审核通过后获得
        /// </summary>
        public string AppId
        {
            get { return ClientId; }
            set { ClientId = value; }
        }
        /// <summary>
        /// 应用密钥AppSecret，在微信开放平台提交应用审核通过后获得
        /// </summary>
        public string AppSecret
        {
            get { return ClientSecret; }
            set { ClientSecret = value; }
        }

        /// <summary>
        /// 回调URL
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// 网站微信登录有两种场景，一种是在微信客户端内打开登录，一种是在微信客户端外登录。
        /// 在微信内登录直接转到让用户授权页面，在微信外则为显示二微码让用户扫描后在微信内授权。
        /// AuthorizationEndpoint是在微信外登录地址，AuthorizationEndpoint2是微信内登录地址
        /// </summary>
        public string AuthorizationEndpoint2 { get; set; }

        /// <summary>
        /// 微信内登录地址 的Scope
        /// </summary>
        public ICollection<string> Scope2 { get; set; }

        /// <summary>
        /// 是否是微信内置浏览器
        /// </summary>
        public Func<HttpRequest, bool> IsWeChatBrowser { get; set; }

        public bool UseCachedStateDataFormat { get; set; } = false;

    }
}
