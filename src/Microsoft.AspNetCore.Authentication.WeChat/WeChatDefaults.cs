namespace Microsoft.AspNetCore.Authentication.WeChat
{
    public static class WeChatDefaults
    {
        public const string AuthenticationScheme = "WeChat";

        public static readonly string DisplayName = "WeChat";

        /// <summary>
        /// 第一步，获取授权临时票据（code）地址
        /// </summary>
        public static readonly string AuthorizationEndpoint = "https://open.weixin.qq.com/connect/oauth2/authorize";

        /// <summary>
        /// 第二步，用户允许授权后，通过返回的code换取access_token地址
        /// </summary>
        public static readonly string TokenEndpoint = "https://api.weixin.qq.com/sns/oauth2/access_token";

        /// <summary>
        /// 第三步，使用access_token获取用户个人信息地址
        /// </summary>
        public static readonly string UserInformationEndpoint = "https://api.weixin.qq.com/sns/userinfo";
    }
}
