using System.Threading.Tasks;
using StandardProducts.Models;

namespace StandardProducts.Controllers {

    public interface IHealthCheckController {
        Task<HealthCheckResult> HealthCheckGetAsync();
    }

    public class HealthCheckControllerImpl : IHealthCheckController {
        public Task<HealthCheckResult> HealthCheckGetAsync() {
            return Task.Factory.StartNew(() => new HealthCheckResult {
                Result = "OK"
            });
        }
    }

}