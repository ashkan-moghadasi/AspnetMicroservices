using System;
using System.Threading.Tasks;
using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly ILogger<DiscountService> _logger;
        private readonly IDiscountRepository _repository;
        private readonly IMapper _mapper;

        public DiscountService(ILogger<DiscountService> logger, IDiscountRepository repository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);
            if (coupon == null)
            {
                _logger.LogError($"Could Not Found Discount For Product {request.ProductName}");
                throw new RpcException(new Status(StatusCode.NotFound, ""),
                    $"Could Not Found Discount For Product {request.ProductName} .");
            }

            _logger.LogInformation($"Discount For {request.ProductName} is {coupon.Amount}");
            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _repository.CreateDiscount(coupon);
            _logger.LogInformation($"Discount Created Successfully.ProductName {coupon.ProductName} ");
            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            await _repository.UpdateDiscount(coupon);
            _logger.LogInformation($"Discount Updated Successfully .ProductName {coupon.ProductName} ");
            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request,
            ServerCallContext context)
        {
            var result = await _repository.DeleteDiscount(request.Coupon.ProductName);
            _logger.LogInformation(result
                ? $"Discount Deleted Successfully .ProductName {request.Coupon.ProductName} "
                : $"Discount Deleted Failed .ProductName {request.Coupon.ProductName} ");

            return new DeleteDiscountResponse() {Success = result};
        }
    }
}