using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Authentication.QQConnect
{
    public class QQConnectOptions : OAuthOptions
    {
        public QQConnectOptions()
        {
            CallbackPath = new PathString("/signin-qq");
            AuthorizationEndpoint = QQConnectDefaults.AuthorizationEndpoint;
            TokenEndpoint = QQConnectDefaults.TokenEndpoint;
            UserInformationEndpoint = QQConnectDefaults.UserInformationEndpoint;
            OpenIdEndpoint = QQConnectDefaults.OpenIdEndpoint;

            //StateDataFormat = 
            //Scope 表示用户授权时向用户显示的可进行授权的列表。
            Scope.Add("get_user_info"); //默认只请求对get_user_info进行授权

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "openid");
            ClaimActions.MapJsonKey(ClaimTypes.Name, "nickname");
            ClaimActions.MapJsonKey("urn:qq:figure", "figureurl_qq_1");
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
