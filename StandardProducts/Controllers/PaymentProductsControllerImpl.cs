using System;
using System.Threading.Tasks;
using StandardProducts.Models;
using StandardProducts.Services;

namespace StandardProducts.Controllers {

    public interface IPaymentProductsController {
        /// <summary>
        /// <see cref="PaymentProductsApiController.PaymentProductDetailsPost(PaymentProductDetailsRequest)"/>
        /// </summary>
        Task<PaymentProductDetailsResult> PaymentProductDetailsPostAsync(
            PaymentProductDetailsRequest paymentDetailsRequest);
    }

    public class PaymentProductsControllerImpl : IPaymentProductsController {
        private readonly IPaymentProductService _paymentProductService;

        public PaymentProductsControllerImpl(IPaymentProductService paymentProductService) {
            _paymentProductService =
                paymentProductService ?? throw new ArgumentNullException(nameof(paymentProductService));
        }

        public async Task<PaymentProductDetailsResult> PaymentProductDetailsPostAsync(
            PaymentProductDetailsRequest paymentDetailsRequest) {
            return await _paymentProductService.GetPaymentDetailsAsync(paymentDetailsRequest.KumsIds,
                                                                       paymentDetailsRequest.SearchString,
                                                                       paymentDetailsRequest.SearchProfileName,
                                                                       paymentDetailsRequest.SkipCount,
                                                                       paymentDetailsRequest.MaxResultCount);
        }
    }

}