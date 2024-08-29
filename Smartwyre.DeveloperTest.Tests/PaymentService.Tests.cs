using Moq;
using Xunit;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using System.Collections.Generic;
using System.Linq;

namespace Smartwyre.DeveloperTest.Tests
{
    public class PaymentServiceTests
    {
        [Fact]
        public void Calculate_ShouldReturnSuccess_ForValidFixedCashAmountRebate()
        {
            var rebate = new Rebate
            {
                Identifier = "rebate1",
                Incentive = IncentiveType.FixedCashAmount,
                Amount = 100m
            };
            var product = new Product
            {
                Identifier = "product1",
                SupportedIncentives = SupportedIncentiveType.FixedCashAmount
            };

            var rebateDataStoreMock = new Mock<IRebateDataStore>();
            rebateDataStoreMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(rebate);
            rebateDataStoreMock.Setup(r => r.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()));

            var productDataStoreMock = new Mock<IProductDataStore>();
            productDataStoreMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(product);

            var fixedCashAmountCalculator = new FixedCashAmountRebateCalculator();
            var rebateCalculators = new List<IRebateCalculator> { fixedCashAmountCalculator };

            var service = new RebateService(rebateDataStoreMock.Object, productDataStoreMock.Object, rebateCalculators);

            var request = new CalculateRebateRequest
            {
                RebateIdentifier = "rebate1",
                ProductIdentifier = "product1",
                Volume = 10
            };

            var result = service.Calculate(request);

            Assert.True(result.Success);
            rebateDataStoreMock.Verify(r => r.StoreCalculationResult(rebate, 100m), Times.Once);
        }

        [Fact]
        public void Calculate_ShouldReturnSuccess_ForValidFixedRateRebate()
        {
            var rebate = new Rebate
            {
                Identifier = "rebate2",
                Incentive = IncentiveType.FixedRateRebate,
                Percentage = 0.1m // 10%
            };
            var product = new Product
            {
                Identifier = "product2",
                Price = 200m,
                SupportedIncentives = SupportedIncentiveType.FixedRateRebate
            };

            var rebateDataStoreMock = new Mock<IRebateDataStore>();
            rebateDataStoreMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns(rebate);
            rebateDataStoreMock.Setup(r => r.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()));

            var productDataStoreMock = new Mock<IProductDataStore>();
            productDataStoreMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(product);

            var fixedRateRebateCalculator = new FixedRateRebateCalculator();
            var rebateCalculators = new List<IRebateCalculator> { fixedRateRebateCalculator };

            var service = new RebateService(rebateDataStoreMock.Object, productDataStoreMock.Object, rebateCalculators);

            var request = new CalculateRebateRequest
            {
                RebateIdentifier = "rebate2",
                ProductIdentifier = "product2",
                Volume = 3
            };

            var result = service.Calculate(request);

            var expectedRebateAmount = 200m * 0.1m * 3; // 600
            Assert.True(result.Success);
            rebateDataStoreMock.Verify(r => r.StoreCalculationResult(rebate, expectedRebateAmount), Times.Once);
        }

        [Fact]
        public void Calculate_ShouldReturnFailure_IfRebateDataIsMissing()
        {
            var rebate = new Rebate
            {
                Identifier = "rebate3",
                Incentive = IncentiveType.AmountPerUom,
                Amount = 50m
            };
            var product = new Product
            {
                Identifier = "product3",
                SupportedIncentives = SupportedIncentiveType.AmountPerUom
            };

            var rebateDataStoreMock = new Mock<IRebateDataStore>();
            rebateDataStoreMock.Setup(r => r.GetRebate(It.IsAny<string>())).Returns<Rebate>(null); // Simulate missing rebate

            var productDataStoreMock = new Mock<IProductDataStore>();
            productDataStoreMock.Setup(p => p.GetProduct(It.IsAny<string>())).Returns(product);

            var amountPerUomCalculator = new AmountPerUomRebateCalculator();
            var rebateCalculators = new List<IRebateCalculator> { amountPerUomCalculator };

            var service = new RebateService(rebateDataStoreMock.Object, productDataStoreMock.Object, rebateCalculators);

            var request = new CalculateRebateRequest
            {
                RebateIdentifier = "rebate3",
                ProductIdentifier = "product3",
                Volume = 10
            };

            var result = service.Calculate(request);

            Assert.False(result.Success);
            rebateDataStoreMock.Verify(r => r.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), Times.Never);
        }
    }
}
