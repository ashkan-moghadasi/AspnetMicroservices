using System;
using System.Net;
using System.Threading.Tasks;
using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Discount.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DiscountController:ControllerBase
    {
        private readonly IDiscountRepository _repository;

        public DiscountController(IDiscountRepository repository)
        {
            _repository = repository??throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("{productName}",Name = "GetDiscount")]
        [ProducesResponseType(typeof(Coupon),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<Coupon>> GetDiscount(string productName)
        {
            var coupon = await _repository.GetDiscount(productName);
            return Ok(coupon);
        }
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult<Coupon>> CreateDiscount([FromBody] Coupon coupon)
        {
            var result=await _repository.CreateDiscount(coupon);
            return CreatedAtRoute("GetDiscount", new {productName = coupon.ProductName}, coupon);
        }
        
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateDiscount([FromBody] Coupon coupon)
        {
            await _repository.UpdateDiscount(coupon);
            return NoContent();
        }
        [HttpDelete("{productName}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteDiscount(string productName)
        {
            var result=await _repository.DeleteDiscount(productName);
            if (!result) return NotFound();
            return NoContent();


        }
    }
}