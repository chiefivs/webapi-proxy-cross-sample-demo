using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NLog;
using StandardProducts.Filters;
using StandardProducts.Models;
using StandardProducts.Services;

namespace StandardProducts.Controllers {

    public interface IBillingInvoiceController {
        /// <summary>
        /// <see cref="BillingInvoiceApiController.GetBillingInvoicesAdvancedSearchPost"/>
        /// </summary>
        Task<BillingInvoicesResult> GetBillingInvoicesAdvancedSearchPostAsync(
            BillingInvoicesAdvancedSearchRequest request);

        /// <summary>
        /// <see cref="BillingInvoiceApiController.GetBillingInvoiceFilePost"/>
        /// </summary>
        Task<BillingInvoiceFileResult> GetBillingInvoiceFilePostAsync(GetBillingInvoiceFileRequest request);

        /// <summary>
        /// <see cref="BillingInvoiceApiController.GetBillingInvoicesAdvancedSearchPost"/>
        /// </summary>
        Task<SearchableColumnsResult> GetBillingInvoicesSearchableColumnsPostAsync();
    }

    public class BillingInvoiceControllerImpl : IBillingInvoiceController {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly IBillingInvoiceService _billingInvoiceService;
        private readonly IKumsFilter _kumsFilter;
        private readonly IMapper _mapper;

        public BillingInvoiceControllerImpl(IBillingInvoiceService billingInvoiceService,
            IKumsFilter kumsFilter,
            IMapper mapper) {
            _billingInvoiceService = billingInvoiceService;
            _kumsFilter = kumsFilter;
            _mapper = mapper;
        }

        public async Task<BillingInvoicesResult> GetBillingInvoicesAdvancedSearchPostAsync(
            BillingInvoicesAdvancedSearchRequest request) {
            _kumsFilter.ValidateListOfKums(request.KumsIds);

            var serviceResult =
                await _billingInvoiceService.GetBillingInvoicesDetailsAdvancedSearchAsync(request.KumsIds,
                 request.MaxResultCount,
                 request.SkipCount,
                 request.Sorting,
                 request.AdvancedSearch);
            Logger.Debug($"Found {serviceResult.BillingInvoicesDetails.Count} records.");
            return serviceResult;
        }

        public async Task<BillingInvoiceFileResult>
            GetBillingInvoiceFilePostAsync(GetBillingInvoiceFileRequest request) {
            var invoiceContent = await _billingInvoiceService.GetInvoiceContent(request.InvoiceNumber);
            Logger.Debug($"Invoice {request.InvoiceNumber} content length = {invoiceContent?.Length ?? 0}.");
            return new BillingInvoiceFileResult {
                Content = invoiceContent != null ? Convert.ToBase64String(invoiceContent) : null
            };
        }

        public async Task<SearchableColumnsResult> GetBillingInvoicesSearchableColumnsPostAsync() {
            var advancedSearch = ( await _billingInvoiceService.GetBillingInvoicesSearchableColumnsAsync() ).ToArray();
            Logger.Debug($"Found {advancedSearch.Length} fields for advanced search.");
            return new SearchableColumnsResult {
                Items = advancedSearch.Select(_mapper.Map<Column>).ToList()
            };
        }
    }

}
