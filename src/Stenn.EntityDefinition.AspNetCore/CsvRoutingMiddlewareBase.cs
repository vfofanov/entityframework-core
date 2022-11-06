using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Stenn.EntityDefinition.AspNetCore
{
    public abstract class CsvMiddlewareBase
    {
        
        private readonly RequestDelegate _next;
        private readonly string _routePattern;

        protected CsvMiddlewareBase(string routePattern, RequestDelegate next)
        {
            if (routePattern == null)
            {
                throw new ArgumentNullException(nameof(routePattern));
            }

            // ensure _routePattern starts with /
            _routePattern = routePattern.StartsWith('/') ? routePattern : $"/{routePattern}";
            
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        ///     Invoke the OData Route debug middleware.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns>A task that can be awaited.</returns>
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var request = context.Request;

            if (string.Equals(request.Path.Value, _routePattern, StringComparison.OrdinalIgnoreCase))
            {
                await WriteAsCsv(context).ConfigureAwait(false);
            }
            else
            {
                await _next(context).ConfigureAwait(false);
            }
        }

        private async Task WriteAsCsv(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var csvTable = GenerateCsv(context);

            context.Response.ContentType = "text/csv";
            await context.Response.WriteAsync(csvTable).ConfigureAwait(false);
        }

        protected abstract string GenerateCsv(HttpContext context);

        
    }
}