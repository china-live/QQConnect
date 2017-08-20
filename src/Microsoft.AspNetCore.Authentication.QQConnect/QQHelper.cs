using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.AspNetCore.Authentication.QQConnect
{
    internal static class QQHelper
    {
        ///// <summary>
        ///// 用户在QQ空间的昵称。
        ///// </summary>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //internal static string GetNickname(JObject info)
        //{
        //    if (info == null)
        //    {
        //        throw new ArgumentNullException(nameof(info));
        //    }

        //    return info.Value<string>("nickname");
        //}

        ///// <summary>
        ///// 大小为30×30像素的QQ空间头像URL。
        ///// </summary>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //internal static string GetFigure30(JObject info)
        //{
        //    if (info == null)
        //    {
        //        throw new ArgumentNullException(nameof(info));
        //    }

        //    return info.Value<string>("figureurl");
        //}
        ///// <summary>
        ///// 大小为50×50像素的QQ空间头像URL。
        ///// </summary>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //internal static string GetFigure50(JObject info)
        //{
        //    if (info == null)
        //    {
        //        throw new ArgumentNullException(nameof(info));
        //    }

        //    return info.Value<string>("figureurl_1");
        //}
        ///// <summary>
        ///// 大小为100×100像素的QQ空间头像URL。
        ///// </summary>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //internal static string GetFigure100(JObject info)
        //{
        //    if (info == null)
        //    {
        //        throw new ArgumentNullException(nameof(info));
        //    }

        //    return info.Value<string>("figureurl_2");
        //}
        ///// <summary>
        ///// 大小为40×40像素的QQ头像URL。
        ///// </summary>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //internal static string GetQQFigure40(JObject info)
        //{
        //    if (info == null)
        //    {
        //        throw new ArgumentNullException(nameof(info));
        //    }

        //    return info.Value<string>("figureurl_qq_1");
        //}
        ///// <summary>
        ///// 大小为100×100像素的QQ头像URL。需要注意，不是所有的用户都拥有QQ的100×100的头像，但40×40像素则是一定会有。
        ///// </summary>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //internal static string GetQQFigure100(JObject info)
        //{
        //    if (info == null)
        //    {
        //        throw new ArgumentNullException(nameof(info));
        //    }

        //    return info.Value<string>("figureurl_qq_2");
        //}

        internal static string GetOpenId(JObject json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            return json.Value<string>("openid");
        }
    }
}
