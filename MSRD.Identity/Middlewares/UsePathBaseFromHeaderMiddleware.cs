// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MSRD.Identity.Middlewares
{
    /// <summary>
    /// Represents a middleware that extracts the specified path base from request path and postpend it to the request path base.
    /// </summary>
    public class UsePathBaseFromHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _headerName;

        /// <summary>
        /// Creates a new instance of <see cref="UsePathBaseFromHeaderMiddleware"/>.
        /// </summary>
        /// <param name="next">The delegate representing the next middleware in the request pipeline.</param>
        /// <param name="headerName">The header name from where to extract the path base to extract.</param>
        public UsePathBaseFromHeaderMiddleware(RequestDelegate next, string headerName)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (string.IsNullOrEmpty(headerName))
            {
                throw new ArgumentException($"{nameof(headerName)} cannot be null or empty.");
            }

            _next = next;
            _headerName = headerName;
        }

        /// <summary>
        /// Executes the middleware.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
        /// <returns>A task that represents the execution of this middleware.</returns>
        public Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Request.Headers.TryGetValue(_headerName, out var pathBase) &&
                context.Request.Path.StartsWithSegments(pathBase.Last(), out var matchedPath, out var remainingPath))
            {
                return InvokeCore(context, matchedPath, remainingPath);
            }
            return _next(context);
        }

        private async Task InvokeCore(HttpContext context, PathString matchedPath, PathString remainingPath)
        {
            var originalPath = context.Request.Path;
            var originalPathBase = context.Request.PathBase;
            context.Request.Path = remainingPath;
            context.Request.PathBase = originalPathBase.Add(matchedPath);

            try
            {
                await _next(context);
            }
            finally
            {
                context.Request.Path = originalPath;
                context.Request.PathBase = originalPathBase;
            }
        }
    }
}
