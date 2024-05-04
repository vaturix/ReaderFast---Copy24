using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System.IO;
using System.Threading.Tasks;

namespace ReaderFast.webui.Services
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;

        public ViewRenderService(
            IHttpContextAccessor httpContextAccessor,
            ICompositeViewEngine viewEngine,
            ITempDataProvider tempDataProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
        }

        public async Task<string> RenderToStringAsync(string viewPath, object model)
        {
            var httpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = _httpContextAccessor.HttpContext.RequestServices };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(actionContext, viewPath, false);

                if (viewResult.View == null)
                {
                    throw new System.Exception($"View '{viewPath}' not found.");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}
