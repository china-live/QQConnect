using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.WeChat;

namespace AspNetCore.Authentication.WeChat
{
    /// <summary>
    /// We Chat Application
    /// </summary>
    public class WeChatApplication
    {
        /// <summary>
        /// App Id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// App Secret
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// App 类型
        /// </summary>
        public WeChatApplicationType AppType { get; set; }

        /// <summary>
        /// Scopes
        /// </summary>
        public List<string> Scopes { get; set; }=new List<string>();

        /// <summary>
        /// 网站微信登录有两种场景，一种是在微信客户端内打开登录，一种是在微信客户端外登录。
        /// 在微信内登录直接转到让用户授权页面，在微信外则为显示二微码让用户扫描后在微信内授权。
        /// AuthorizationEndpoint是在微信外登录地址，AuthorizationEndpoint2是微信内登录地址
        /// 公众号，<see cref="WeChatDefaults.AuthorizationEndpoint2"/>>
        /// 网站,<see cref="WeChatDefaults.AuthorizationEndpoint"/>>
        /// </summary>
        public string AuthorizationEndpoint { get; set; }

    }
}
