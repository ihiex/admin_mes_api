using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NPOI.XWPF.UserModel;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons;
using SunnyMES.Commons.Core.App;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Json;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Options;
namespace SunnyMES.AspNetCore.Mvc.Filter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class CommonAuthorizeAttribute : ActionFilterAttribute
{
    private readonly bool isEnabled = Configs.GetSection("AppSetting:EnabledActionLog").Value == "1";
    private DateTime startTime = DateTime.MinValue;
    /// <inheritdoc />
    public  override async void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        startTime = DateTime.Now;

        var para = context.HttpContext.Request.QueryString.Value?.Replace("\\r\\n", "\r\n").Replace("\\u0022", "\u0022");
        var controllerName = context.HttpContext.GetRouteValue("controller");
        var actionName = context.HttpContext.GetRouteValue("action");

        var request = context.HttpContext.Request;
        
        string bodyContent = null;
        if (request.Method.ToLower().Equals("post") && request.Body.Length > 0)
        {
            request.EnableBuffering();
            context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8);
            bodyContent = await reader.ReadToEndAsync();
        }

        if (isEnabled)
        {
            var stationName = context.HttpContext.Request.Headers["station-name"];
            var lineName = context.HttpContext.Request.Headers["line-name"];
            var staionId = context.HttpContext.Request.Headers["station-id"];
            var lineId = context.HttpContext.Request.Headers["line-id"];

            Log4NetHelper.Debug($"{staionId} - {stationName}, {lineId} - {lineName}");
            Log4NetHelper.Debug($"exec : {controllerName} - {actionName}, param ：{para}, body : {bodyContent ?? ""}");
        }                
    }

    /// <inheritdoc />
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        base.OnActionExecuted(context);
        var result = context.Result.ToJson().Replace("\\r\\n", "\r\n").Replace("\\u0022", "\u0022");
        var controllerName = context.HttpContext.GetRouteValue("controller");
        var actionName = context.HttpContext.GetRouteValue("action");
        if (isEnabled)
        {
            Log4NetHelper.Debug($"执行{controllerName} - {actionName},耗时为:{(DateTime.Now - startTime).Milliseconds}s, 结果为：" + result);
        }
    }

    /// <inheritdoc />
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (next == null)
            throw new ArgumentNullException(nameof(next));
        this.OnActionExecuting(context);
        if (context.Result != null)
            return;
        this.OnActionExecuted(await next());
    }

    /// <inheritdoc />
    public override async void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is JsonResult)
        {
            //只有是JsonResult类型的才特殊处理
            //JsonResult jsonResult = (JsonResult)context.Result;
            //context.Result = new JsonResult(new
            //{
            //    Success = true,
            //    Message = "OK",
            //    Data = jsonResult.Value
            //});
        }

        if (context.Result is CommonResult)
        {
            Console.WriteLine("aa");
        }

    }

    /// <inheritdoc />
    public override void OnResultExecuted(ResultExecutedContext context)
    {
        base.OnResultExecuted(context);
        //Console.WriteLine("");
    }

    /// <inheritdoc />
    public override async Task OnResultExecutionAsync(
        ResultExecutingContext context,
        ResultExecutionDelegate next)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (next == null)
            throw new ArgumentNullException(nameof(next));
        this.OnResultExecuting(context);
        if (context.Cancel)
            return;
        this.OnResultExecuted(await next());
    }
}