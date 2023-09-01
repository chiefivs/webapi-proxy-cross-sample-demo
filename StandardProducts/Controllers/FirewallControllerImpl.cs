using System;
using System.Threading.Tasks;
using StandardProducts.Models;
using StandardProducts.Services;

namespace StandardProducts.Controllers {

    public interface IFirewallController {
        /// <summary>
        /// <see cref="FirewallApiController.GetVirtualFirewallListPost"/>
        /// </summary>
        /// <returns></returns>
        Task<VirtualFirewallResult> GetVirtualFirewallListPostAsync(VirtualFirewallListRequest input);

        /// <summary>
        /// <see cref="FirewallApiController.ValidateWebAcNumberPost"/>
        /// </summary>
        /// <returns></returns>
        Task<ValidateWebAcNumberResult> ValidateWebAcNumberPostAsync(ValidateWebAcNumberRequest input);
    }

    public class FirewallControllerImpl : IFirewallController {
        private readonly IFirewallService _firewallService;

        public FirewallControllerImpl(IFirewallService firewallService) {
            _firewallService = firewallService ?? throw new ArgumentNullException(nameof(firewallService));
        }

        /// <inheritdoc />
        public async Task<VirtualFirewallResult> GetVirtualFirewallListPostAsync(
            VirtualFirewallListRequest input) {
            return await _firewallService.GetVirtualFirewallListAsync(input.KumsIds,
                                                                      input.SearchString,
                                                                      input.SearchProfileName,
                                                                      input.SkipCount,
                                                                      input.MaxResultCount);
        }

        /// <inheritdoc />
        public async Task<ValidateWebAcNumberResult> ValidateWebAcNumberPostAsync(ValidateWebAcNumberRequest input) {

            var firewallResponse = await _firewallService.GetVirtualFirewallByWebAcAsync(input.WebAcNumber);

            var result = new ValidateWebAcNumberResult {
                IsWebAcNumber = firewallResponse != null,
                WebAcNumber = input.WebAcNumber,
                CustomerNumber = firewallResponse?.Kums,
                ProductName = firewallResponse?.ProductGroup
            };

            return result;
        }
    }

}
