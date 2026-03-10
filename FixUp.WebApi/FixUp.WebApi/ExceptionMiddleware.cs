//using System.Net;
//using System.Text.Json;

//namespace FixUp.WebApi.Middleware
//{
//    public class ExceptionMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public ExceptionMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            try
//            {
//                await _next(context);
//            }
//            catch (Exception ex)
//            {
//                // כאן אנחנו עוצרים הכל ושולחים תשובה משלנו
//                await HandleExceptionAsync(context, ex);
//            }
//        }

//        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
//        {
//            // פתרון סופי לבעיית ה-CORS שגרמה לגלגל להסתובב
//            context.Response.Headers.Append("Access-Control-Allow-Origin", "*");

//            context.Response.ContentType = "application/json";
//            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

//            // יצירת אובייקט פשוט מאוד - רק מה שחשוב לך לראות
//            var response = new
//            {
//                error = "Validation Error",
//                message = exception.Message // כאן תופיע ההודעה שלך מה-Service!
//            };

//            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
//            var result = JsonSerializer.Serialize(response, options);

//            return context.Response.WriteAsync(result);
//        }
//    }
//}