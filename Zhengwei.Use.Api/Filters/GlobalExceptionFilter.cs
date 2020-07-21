﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zhengwei.Use.Api.Filters
{
   
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<GlobalExceptionFilter> _logger;
        public GlobalExceptionFilter(IHostingEnvironment env, ILogger<GlobalExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            var json = new JsonErrorResponse
            {
                Message = context.Exception.Message
            };
            if (context.Exception.GetType() == typeof(UserOperationException))
            {
                
                context.Result = new BadRequestObjectResult(json);
            }else
            {
                json.Message = "zhengweiProject中发生了未知的内部异常！";
                if(_env.IsDevelopment())
                {
                    json.Message = context.Exception.StackTrace;
                }
                context.Result = new InternalServerErrorObjectResult(json);
            }
            _logger.LogError(context.Exception, context.Exception.Message);
            context.ExceptionHandled = true;
        }
    }

    public class InternalServerErrorObjectResult:ObjectResult
    {
        public InternalServerErrorObjectResult(object error):base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
