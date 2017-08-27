using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Authentication.WeChat
{
    public class WeChatOptions : OAuthOptions
    {
        public WeChatOptions()
        {
            CallbackPath = new PathString("/signin-wechat");
            AuthorizationEndpoint = WeChatDefaults.AuthorizationEndpoint;
            TokenEndpoint = WeChatDefaults.TokenEndpoint;
            UserInformationEndpoint = WeChatDefaults.UserInformationEndpoint;
 
            //Scope 表示应用授权作用域。
            Scope.Add("snsapi_login"); //网页应用目前仅填写snsapi_login即可

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
    }
}
