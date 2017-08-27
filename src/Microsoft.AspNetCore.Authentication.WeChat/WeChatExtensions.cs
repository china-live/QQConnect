using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Authentication.WeChat
{
    public static class WeChatExtensions
    {
        public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder)
            => builder.AddWeChat(WeChatDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder, Action<WeChatOptions> configureOptions)
            => builder.AddWeChat(WeChatDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder, string authenticationScheme, Action<WeChatOptions> configureOptions)
            => builder.AddWeChat(authenticationScheme, WeChatDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddWeChat(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WeChatOptions> configureOptions)
            => builder.AddOAuth<WeChatOptions, WeChatHandler>(authenticationScheme, displayName, configureOptions);
    }
}
