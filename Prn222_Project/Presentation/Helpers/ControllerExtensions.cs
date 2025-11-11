using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Threading.Tasks;

namespace Presentation.Helpers {
    public static class ControllerExtensions {
        public static async Task<string> RenderViewAsync<TModel>(
            this Controller controller,
            string viewName,
            TModel model,
            bool partial = false) {
            try {
                if (string.IsNullOrEmpty(viewName) || !viewName.StartsWith("~/")) {
                    return null!;
                }

                if (!viewName.EndsWith(".cshtml")) {
                    viewName += ".cshtml";
                }

                controller.ViewData.Model = model;

                using (var writer = new StringWriter()) {
                    var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

                    var viewResult = viewEngine!.GetView(executingFilePath: null, viewPath: viewName, isMainPage: !partial);

                    if (viewResult.View == null)
                        return null!;

                    var viewContext = new ViewContext(
                        controller.ControllerContext,
                        viewResult.View,
                        controller.ViewData,
                        controller.TempData,
                        writer,
                        new HtmlHelperOptions()
                    );

                    await viewResult.View.RenderAsync(viewContext);
                    return writer.GetStringBuilder().ToString();
                }
            } catch {
                return null!;
            }
        }
    }
}
