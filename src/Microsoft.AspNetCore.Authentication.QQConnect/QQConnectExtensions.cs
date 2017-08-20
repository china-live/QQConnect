using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Authentication.QQConnect
{
    public static class QQConnectExtensions
    {
        public static AuthenticationBuilder AddQQConnect(this AuthenticationBuilder builder)
            => builder.AddQQConnect(QQConnectDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddQQConnect(this AuthenticationBuilder builder, Action<QQConnectOptions> configureOptions)
            => builder.AddQQConnect(QQConnectDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddQQConnect(this AuthenticationBuilder builder, string authenticationScheme, Action<QQConnectOptions> configureOptions)
            => builder.AddQQConnect(authenticationScheme, QQConnectDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddQQConnect(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<QQConnectOptions> configureOptions)
            => builder.AddOAuth<QQConnectOptions, QQConnectHandler>(authenticationScheme, displayName, configureOptions);
    }
}
