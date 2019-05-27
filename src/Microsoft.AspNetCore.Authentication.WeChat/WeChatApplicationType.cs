using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Authentication.WeChat
{
    /// <summary>
    /// 微信应用类型
    /// </summary>
    public enum WeChatApplicationType
    {
        /// <summary>
        /// 公众号类型，需要在微信公众平台注册服务号，将会直接调用公众号登录
        /// </summary>
        OfficialAccount,
        /// <summary>
        /// 网站应用，需要在微信开放平台注册，将会调用扫码登录
        /// </summary>
        WebSite,
        /// <summary>
        /// 移动应用
        /// </summary>
        App,
        /// <summary>
        /// 小程序
        /// </summary>
        MiniProgram
    }
}
