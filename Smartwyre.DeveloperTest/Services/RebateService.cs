using System.Collections.Generic;
using System.Linq;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services
{
    public class RebateService : IRebateService
    {
        private readonly IRebateDataStore _rebateDataStore;
        private readonly IProductDataStore _productDataStore;
        private readonly IEnumerable<IRebateCalculator> _rebateCalculators;

        public RebateService(
            IRebateDataStore rebateDataStore,
            IProductDataStore productDataStore,
            IEnumerable<IRebateCalculator> rebateCalculators)
        {
            _rebateDataStore = rebateDataStore;
            _productDataStore = productDataStore;
            _rebateCalculators = rebateCalculators;
        }

        public CalculateRebateResult Calculate(CalculateRebateRequest request)
        {
            var rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
            var product = _productDataStore.GetProduct(request.ProductIdentifier);

            var result = new CalculateRebateResult();

            if (rebate == null || product == null)
            {
                result.Success = false;
                return result;
            }

            var calculator = _rebateCalculators.FirstOrDefault(c => c.CanCalculate(rebate.Incentive));
            if (calculator == null)
            {
                result.Success = false;
                return result;
            }

            var rebateAmount = calculator.Calculate(rebate, product, request);
            result.Success = rebateAmount > 0;

            if (result.Success)
            {
                _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
            }

            return result;
        }
    }
}
