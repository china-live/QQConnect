using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using AspNetCore.Authentication.WeChat;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Authentication.WeChat
{
    public class WeChatOptions : OAuthOptions
    {

        public WeChatOptions()
        {
            CallbackPath = new PathString("/signin-wechat");

            TokenEndpoint = WeChatDefaults.TokenEndpoint;
            UserInformationEndpoint = WeChatDefaults.UserInformationEndpoint;

            //Scope.Add("snsapi_login");

            //除了openid外，其余的都可能为空，因为微信获取用户信息是有单独权限的
            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "openid");

            ClaimActions.MapJsonKey(ClaimTypes.Name, "nickname");
            ClaimActions.MapJsonKey(ClaimTypes.Gender, "sex",ClaimValueTypes.Integer);
            ClaimActions.MapJsonKey(ClaimTypes.Country, "country");//ClaimTypes.Locality
            ClaimActions.MapJsonKey("urn:wechat:province", "province");//ClaimTypes.StateOrProvince
            ClaimActions.MapJsonKey("urn:wechat:city", "city");//ClaimTypes.StreetAddress
            ClaimActions.MapJsonKey(ClaimTypes.Uri, "headimgurl");
            //ClaimActions.MapCustomJson("urn:wechat:privilege", user =>  string.Join(",",user.SelectToken("privilege")?.Select(s => (string)s).ToArray() ?? new string[0]));
            ClaimActions.MapJsonKey("urn:wechat:unionid", "unionid");

            IsWeChatBrowser=(r) => r.IsWeChatBroswer();
        }


        [Obsolete("应该获取当前应用的ClientId", true)]
        public new string ClientId => base.ClientId;

        [Obsolete("应该获取当前应用的ClientSecret", true)]
        public new string ClientSecret => base.ClientSecret;
        [Obsolete("应该获取当前应用的AuthorizationEndpoint", true)]
        public new string AuthorizationEndpoint => base.AuthorizationEndpoint;

        [Obsolete("应该获取当前应用的Scope", true)]
        public new ICollection<string> Scope => base.Scope;


        /// <summary>
        /// 回调URL
        /// </summary>
        public string CallbackUrl { get; set; }


        /// <summary>
        /// 是否是微信内置浏览器
        /// </summary>
        public Func<HttpRequest, bool> IsWeChatBrowser { get; set; }

        public bool UseCachedStateDataFormat { get; set; } = false;

        /// <summary>
        /// App配置
        /// </summary>
        public List<WeChatApplication> Apps { get; set; } = new List<WeChatApplication>();


        /// <summary>
        /// Check that the options are valid.  Should throw an exception if things are not ok.
        /// </summary>
        public override void Validate()
        {
            if (Apps == null || !Apps.Any())
            {
                throw new ArgumentException("App 至少有一个");
            }
        }

    }
}
