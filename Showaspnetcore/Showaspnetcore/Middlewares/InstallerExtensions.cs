using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Showaspnetcore.Middlewares
{
    public static class InstallerExtensions
    {
        public static IApplicationBuilder UseInstaller(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InstallerMiddleware>();
        }
    }
}
