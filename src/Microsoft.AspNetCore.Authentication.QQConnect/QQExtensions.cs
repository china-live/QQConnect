using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Authentication.QQConnect
{
    public static class QQConnectExtensions
    {
        public static AuthenticationBuilder AddQQConnect(this AuthenticationBuilder builder)
            => builder.AddQQConnect(QQDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddQQConnect(this AuthenticationBuilder builder, Action<QQConnectOptions> configureOptions)
            => builder.AddQQConnect(QQDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddQQConnect(this AuthenticationBuilder builder, string authenticationScheme, Action<QQConnectOptions> configureOptions)
            => builder.AddQQConnect(authenticationScheme, QQDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddQQConnect(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<QQConnectOptions> configureOptions)
            => builder.AddOAuth<QQConnectOptions, QQHandler>(authenticationScheme, displayName, configureOptions);
    }
}
