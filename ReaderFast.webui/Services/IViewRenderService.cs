using System.Threading.Tasks;

namespace ReaderFast.webui.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
