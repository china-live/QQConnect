using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Authentication.QQ
{
    public class QQOptions : OAuthOptions
    {
        public QQOptions()
        {
            CallbackPath = new PathString("/signin-qq");
            AuthorizationEndpoint = QQDefaults.AuthorizationEndpoint;
            TokenEndpoint = QQDefaults.TokenEndpoint;
            UserInformationEndpoint = QQDefaults.UserInformationEndpoint;
            OpenIdEndpoint = QQDefaults.OpenIdEndpoint;

            //StateDataFormat = 
            //Scope 表示用户授权时向用户显示的可进行授权的列表。
            Scope.Add("get_user_info"); //默认只请求对get_user_info进行授权

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "openid");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "nickname"); //显示名、昵称
            ClaimActions.MapJsonKey(ClaimTypes.Gender, "gender"); //性别
            ClaimActions.MapJsonKey("urn:qq:openid", "openid");
            ClaimActions.MapJsonKey("urn:qq:figure", "figureurl_qq_1"); //头像 40*40

            ClaimActions.MapJsonKey("urn:qq:figureurl_1", "figureurl_1");
            ClaimActions.MapJsonKey("urn:qq:figureurl_2", "figureurl_2");
            ClaimActions.MapJsonKey("urn:qq:figureurl_qq_1", "figureurl_qq_1");
            ClaimActions.MapJsonKey("urn:qq:figureurl_qq_2", "figureurl_qq_2");
            ClaimActions.MapJsonKey("urn:qq:is_yellow_vip", "is_yellow_vip");
            ClaimActions.MapJsonKey("urn:qq:vip", "vip");
            ClaimActions.MapJsonKey("urn:qq:yellow_vip_level", "yellow_vip_level");
            ClaimActions.MapJsonKey("urn:qq:level", "level");
            ClaimActions.MapJsonKey("urn:qq:is_yellow_year_vip", "is_yellow_year_vip");
        }

        public string OpenIdEndpoint { get; }

        /// <summary>
        /// QQ互联 APP ID https://connect.qq.com
        /// </summary>
        public string AppId
        {
            get { return ClientId; }
            set { ClientId = value; }
        }
        /// <summary>
        /// QQ互联 APP Key https://connect.qq.com
        /// </summary>
        public string AppKey
        {
            get { return ClientSecret; }
            set { ClientSecret = value; }
        }
    }
}
