# QQConnect
asp.net core2.0 QQ登录

基于Microsoft.AspNetCore.Authentication.OAuth实现([aspnet/Security2.0](https://github.com/aspnet/Security/tree/rel/2.0.0))

[QQ Connect接入文档](http://wiki.connect.qq.com/%E5%87%86%E5%A4%87%E5%B7%A5%E4%BD%9C_oauth2-0)

****
## 使用方法
~~~ 
//在Startup.cs中的ConfigureServices方法中增加如下代码
services.AddAuthentication().AddQQ(qqconnectOptions =>
{
    qqconnectOptions.AppId = Configuration["Authentication:QQ:AppId"];
    qqconnectOptions.AppKey = Configuration["Authentication:QQ:AppKey"];
});
~~~