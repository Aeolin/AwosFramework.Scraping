using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Middleware
{
    public interface IMiddleware
    {
        public Task<bool> ExecuteAsync(MiddlewareContext context);
    }
}
