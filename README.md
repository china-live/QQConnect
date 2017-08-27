# QQConnect
asp.net core2.0 QQ登录、微信登录（未测试）

基于Microsoft.AspNetCore.Authentication.OAuth实现([aspnet/Security2.0](https://github.com/aspnet/Security/tree/rel/2.0.0))

[QQ Connect接入文档](http://wiki.connect.qq.com/%E5%87%86%E5%A4%87%E5%B7%A5%E4%BD%9C_oauth2-0)

[微信开方平台](https://open.weixin.qq.com/) 用于PC端网站(严格来说是微信外)的微信登录在这里申请 

[微信公众平台](https://mp.weixin.qq.com/) 用于在微信内（镶）调（嵌）网页的微信登录在这里申请

注意两个的区别，一个是在微信外打开网站页面，一个是在微信内打开的网页（例如你关注的公众号内（镶）调（嵌）用的页面）。虽然都是用微信登录你的网站，但流程上是有些差别。
****
## 使用方法
~~~ 
//在Startup.cs中的ConfigureServices方法中增加如下代码
services.AddAuthentication().AddQQ(qqOptions =>
{
    qqOptions.AppId = Configuration["Authentication:QQ:AppId"];
    qqOptions.AppKey = Configuration["Authentication:QQ:AppKey"];
}).AddWeChat(wechatOptions => {
    wechatOptions.AppId = Configuration["Authentication:WeChat:AppId"];
    wechatOptions.AppSecret = Configuration["Authentication:WeChat:AppSecret"];
}) ;
~~~
