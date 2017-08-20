using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Authentication.QQConnect
{
    public static class QQConnectExtensions
    {
        public static AuthenticationBuilder AddQQ(this AuthenticationBuilder builder)
            => builder.AddQQ(QQDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddQQ(this AuthenticationBuilder builder, Action<QQConnectOptions> configureOptions)
            => builder.AddQQ(QQDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddQQ(this AuthenticationBuilder builder, string authenticationScheme, Action<QQConnectOptions> configureOptions)
            => builder.AddQQ(authenticationScheme, QQDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddQQ(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<QQConnectOptions> configureOptions)
            => builder.AddOAuth<QQConnectOptions, QQHandler>(authenticationScheme, displayName, configureOptions);
    }
}
