using System.Threading.Tasks;
using SparkPost;

namespace Posttrack.BLL.Helpers.Interfaces
{
    public interface ISparkPostTemplateProvider
    {
        string GetTemplateId(EmailType type);

        Task<RetrieveTemplateResponse> GetTemplate(EmailType type);
    }
}