# QQConnect
asp.net core3.0 QQ登录、微信登录

基于Microsoft.AspNetCore.Authentication.OAuth实现([aspnetcore/Security](https://github.com/dotnet/aspnetcore))

[在线实例](https://www.dming.top)

QQ登录在[QQ互联](https://connect.qq.com)申请


微信登录分为两种情况

1.扫码登录，通常于在PC端，即在浏览器中选择用微信登录时，页面会重定向到微信开发平台页面（https://open.weixin.qq.com/connect/qrconnect） 显示一个二维码，用户需使用微信扫码登录，扫码后微信客户端内会显示一个授权界面，用户授权后，网站会自动跳转到你在开放平台-网站应用内设置的回调地址。

理论上这种登录方式在任何场景都可用，不管是PC端浏览器还是移动端浏览器，甚至是微信内的浏览器，包括公众号、服务号、小程序内嵌页面。
但是，在手机、微信内登录网站要先显示一个二维码，再用另一个手机去扫一下才能登录不是很扯吗。
所以微信是会屏蔽了这个的，在微信内打开时不会显示二维码而是一片空白，移动端浏览器还可用（微信也只有这种方式了，QQ在这里可调用本机安装且登录的QQ用户信息）。

扫码登录，请前往[微信开放平台](https://open.weixin.qq.com)注册认证（交300块钱），申请一个网站应用即可

2.微信内公众号、服务号、小程序内嵌页面获取用户信息，只须取得用户授权即可，在[微信公众平台](https://mp.weixin.qq.com)申请。
可以在[微信开放平台](https://open.weixin.qq.com)-管理中心-公众帐号-绑定测试号申请一个测试号进行测试（同需认证）。

这样就会导致如果需要在同一个网站同时支持这两种登录方式的话，后台必须使用不同的appId，本组件以判断请求是否来自微信内置浏览器发出进行区分

另外，微信的OpenId和用户并不是一对一的关系，所以强烈建议开发的时候保存unionId，然后通过unionId匹配登录
****
## 使用方法

### QQ
1.在NuGet中搜索Microsoft.AspNetCore.Authentication.QQ或AspNetCore.Authentication.QQ安装。也可以把本项目源码Microsoft.AspNetCore.Authentication.QQ目录拷贝到你的工程目录内，再在解决方案中附加该项目即可。  

2.打开appsettings.json文件，添加如下代码
~~~
{
    "Authentication": {
        "QQ": {
            "AppId": "你申请的QQ互联AppID",
            "AppKey": "你申请的QQ互联AppKey"
        }
    },
    //省略....
}
~~~
3.在Startup.cs中的ConfigureServices方法中增加如下代码
~~~ 
services.AddAuthentication().AddQQ(qqOptions =>
{
    qqOptions.AppId = Configuration["Authentication:QQ:AppId"];
    qqOptions.AppKey = Configuration["Authentication:QQ:AppKey"];
});
~~~
4.默认回调地址为 http://DomainName/signin-qq 和 https://DomainName/signin-wechat(微信必须要求https)

### 微信
微信NuGet上包名为AspNetCore.Authentication.Weixin和AspNetCore.Authentication.WeChat

~~~
services.AddAuthentication().AddWeChat(wechatOptions => {
                Configuration.GetSection("Authentication:WeChat").Bind(wechatOptions);
            }) ;
~~~

### QQ+微信
QQ微信一起用时，具体可参考Demo

~~~
//appsettings.json
{
    "Authentication": {
        "QQ": {
            "AppId": "你申请的QQ互联AppID",
            "AppKey": "你申请的QQ互联AppKey"
        }"WeChat": {
            "AppId": "你申请的微信应用AppID",
            "AppSecret": "你申请的微信应用AppSecret"
        }
    },
    //省略....
}
~~~

~~~
//Startup.cs
services.AddAuthentication().AddQQ(qqOptions =>
 {
    qqOptions.AppId = Configuration["Authentication:QQ:AppId"];
    qqOptions.AppKey = Configuration["Authentication:QQ:AppKey"];
}).AddWeChat(wechatOptions => {
    wechatOptions.AppId = Configuration["Authentication:WeChat:AppId"];
    wechatOptions.AppSecret = Configuration["Authentication:WeChat:AppSecret"];
}) ;
~~~


#微信State Too Long 报错
由于微信的设置，state最多128字节，但是默认生成的state会超出限制，所以需要加入缓存
~~~
 services.AddAuthentication()
                .AddWeChat(wechatOptions => {
                    wechatOptions.AppId = configuration["Authentication:WeChat:AppId"];
                    wechatOptions.AppSecret = configuration["Authentication:WeChat:AppSecret"];
                    wechatOptions.UseCachedStateDataFormat = true;
                })

//注意如果你有多个后端服务器，需要使用真实的分布式缓存
 services.AddDistributedMemoryCache();
~~~
