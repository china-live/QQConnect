namespace Microsoft.AspNetCore.Authentication.WeChat
{
    public static class WeChatDefaults
    {
        public const string AuthenticationScheme = "WeChat";

        public static readonly string DisplayName = "WeChat";

        /// <summary>
        /// 第一步，获取授权临时票据（code）地址，适用于微信客户端外的网页登录
        /// </summary>
        public static readonly string AuthorizationEndpoint = "https://open.weixin.qq.com/connect/qrconnect";

        /// <summary>
        /// 第一步，获取授权临时票据（code）地址，适用于微信客户端内的网页登录（在微信内部访问登录）
        /// </summary>
        public static readonly string AuthorizationEndpoint2 = "https://open.weixin.qq.com/connect/oauth2/authorize";

        /// <summary>
        /// 第二步，用户允许授权后，通过返回的code换取access_token地址
        /// </summary>
        public static readonly string TokenEndpoint = "https://api.weixin.qq.com/sns/oauth2/access_token";

        /// <summary>
        /// 第三步，使用access_token获取用户个人信息地址
        /// </summary>
        public static readonly string UserInformationEndpoint = "https://api.weixin.qq.com/sns/userinfo";

        public static readonly string UserInfoScope = "snsapi_userinfo";
        public static readonly string LoginScope = "snsapi_login";

        public static readonly string AppIdKey = "appid";
    }
}
