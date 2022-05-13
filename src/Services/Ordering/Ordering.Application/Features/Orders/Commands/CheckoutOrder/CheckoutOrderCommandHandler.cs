using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler:IRequestHandler<CheckoutOrderCommand,int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;
        private readonly IMapper _mapper;

        public CheckoutOrderCommandHandler(IOrderRepository orderRepository, IEmailService emailService,
            ILogger<CheckoutOrderCommandHandler> logger, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = _mapper.Map<Order>(request);
            var newOrder = await _orderRepository.AddAsync(orderEntity);
            await SendMail(newOrder);
            return newOrder.Id;
        }

        private async Task SendMail(Order newOrder)
        {
            try
            {
                var email = new Email()
                    {Body = $"Order id {newOrder.Id} Created.", Subject = "Order Created", To = "Ashkan@mail.com"};
                await _emailService.SendMail(email);

            }
            catch (Exception e)
            {
                _logger.LogError($"Error in Sending Mail for order id {newOrder.Id}",e);
            }
        }
    }
}