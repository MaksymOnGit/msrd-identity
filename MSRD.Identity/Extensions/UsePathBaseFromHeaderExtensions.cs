// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder.Extensions;
using MSRD.Identity.Middlewares;

namespace MSRD.Identity.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class UsePathBaseFromHeaderExtensions
    {
        /// <summary>
        /// Adds a middleware that extracts the specified path base from request path and postpend it to the request path base.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <param name="headerName">The header name from where to extract the path base to extract.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder UsePathBaseFromHeader(this IApplicationBuilder app, string headerName)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (string.IsNullOrEmpty(headerName))
            {
                return app;
            }

            return app.UseMiddleware<UsePathBaseFromHeaderMiddleware>(headerName);
        }
    }
}
