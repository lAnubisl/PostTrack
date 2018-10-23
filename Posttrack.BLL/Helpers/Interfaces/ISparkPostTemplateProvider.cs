using SparkPost;
using System.Threading.Tasks;

namespace Posttrack.BLL.Helpers.Interfaces
{
    public interface ISparkPostTemplateProvider
    {
        string GetTemplateId(EmailTypes type);
        Task<RetrieveTemplateResponse> GetTemplate(EmailTypes type);
    }
}