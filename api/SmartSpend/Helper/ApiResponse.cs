using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SmartSpend.Helper
{
    public static class ApiResponse
    {
        public static IActionResult Success(object data = null, string message = "Request was successful")
        {
            var response = new
            {
                statusCode = 200,
                message,
                data
            };
            return new OkObjectResult(response);
        }

        public static IActionResult Created(object data = null, string message = "Resource created successfully")
        {
            var response = new
            {
                statusCode = 201,
                message,
                data
            };
            return new ObjectResult(response) { StatusCode = 201 };
        }

        public static IActionResult Conflict(dynamic message)
        {
            var response = new
            {
                statusCode = 409,
                message
            };
            return new ConflictObjectResult(response);
        }

        public static IActionResult BadRequest(dynamic message)
        {
            var response = new
            {
                statusCode = 400,
                message
            };
            return new BadRequestObjectResult(response);
        }

        public static IActionResult NotFound(string message = "The requested resource was not found")
        {
            var response = new
            {
                statusCode = 404,
                message
            };
            return new NotFoundObjectResult(response);
        }

        public static IActionResult Unauthorized(string message = "Authorization required")
        {
            var response = new
            {
                statusCode = 401,
                message
            };
            return new UnauthorizedObjectResult(response);
        }

        public static IActionResult InternalServerError(Exception ex, string message = "An unexpected error occurred")
        {
            var response = new
            {
                statusCode = 500,
                message,
                errorDetails = new
                {
                    ex.Message,
                    ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                }
            };
            return new ObjectResult(response) { StatusCode = 500 };
        }
    }
}
