using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Api.Controllers;
using SneakerAPI.Core.DTOs;

    public static class ControllerExtensions
        {
            public static void SetCurrentUser(this BaseController controller, CurrentUser user)
            {
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                };
                controller.HttpContext.Items["CurrentUser"] = user;
            }
        }
            // Helper extension to set the current user in the controller
