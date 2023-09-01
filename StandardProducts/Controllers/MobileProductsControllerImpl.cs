using StandardProducts.Models;
using StandardProducts.Services;
using System;
using System.Threading.Tasks;

namespace StandardProducts.Controllers {

    public interface IMobileProductsController {
        /// <summary>
        /// <see cref="MobileProductsApiController.MobileProductDetailsPost(MobileProductDetailsRequest)"/>
        /// </summary>
        Task<MobileProductDetailsResult>
            MobileProductDetailsPostAsync(MobileProductDetailsRequest mobileDetailsRequest);

        /// <summary>
        /// <see cref="MobileProductsApiController.GetDeviceAsAServiceListPost"/>
        /// </summary>
        Task<DeviceAsAServiceResult> GetDeviceAsAServiceListPostAsync(DeviceAsAServiceRequest input);
    }

    public class MobileProductControllerImpl : IMobileProductsController {
        private readonly IMobileProductService _mobileProductService;

        public MobileProductControllerImpl(IMobileProductService mobileProductService) {
            _mobileProductService =
                mobileProductService ?? throw new ArgumentNullException(nameof(mobileProductService));
        }

        /// <inheritdoc />
        public async Task<MobileProductDetailsResult> MobileProductDetailsPostAsync(
            MobileProductDetailsRequest mobileDetailsRequest) {
            return await _mobileProductService.GetMobileDetailsAsync(mobileDetailsRequest);
        }

        /// <inheritdoc />
        public async Task<DeviceAsAServiceResult> GetDeviceAsAServiceListPostAsync(DeviceAsAServiceRequest input) {
            return await _mobileProductService.GetDeviceAsAServiceListPostAsync(input.KumsIds,
                                                                                input.MaxResultCount,
                                                                                input.SkipCount,
                                                                                input.SearchProfileName,
                                                                                input.SearchString);
        }
    }

}