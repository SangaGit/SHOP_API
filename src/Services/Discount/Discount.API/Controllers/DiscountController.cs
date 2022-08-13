using System.Net;
using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Discount.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DiscountController : ControllerBase
{
    private readonly IDiscountRepository _repository;
    private readonly ILogger<DiscountController> _logger;

    public DiscountController(IDiscountRepository repository, ILogger<DiscountController> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Route("{productName}", Name = "GetDiscount")]
    [HttpGet]
    [ProducesResponseType(typeof(Coupon),(int)HttpStatusCode.OK)]
    public async Task<ActionResult<Coupon>> GetDiscount(string productName){
        var result = await _repository.GetDiscount(productName);
        if(result == null){
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Coupon),(int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(Coupon),(int)HttpStatusCode.NoContent)]
    public async Task<ActionResult<Coupon>> CreateDiscount([FromBody]Coupon coupon){
        var result = await _repository.CreateDiscount(coupon);
        if(result == null){
            return NoContent();
        }
        return CreatedAtRoute("GetDiscount", new {productName = coupon.ProductName}, result);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Coupon),(int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Coupon),(int)HttpStatusCode.NoContent)]
    public async Task<ActionResult<Coupon>> UpdateDiscount([FromBody]Coupon coupon){
        var result = await _repository.UpdateDiscount(coupon);
        if(!result){
            return NoContent();
        }
        return Ok(coupon);
    }

    [HttpDelete("{productName}", Name = "DeleteDiscount")]
    [ProducesResponseType(typeof(void),(int)HttpStatusCode.OK)]
    public async Task<ActionResult> DeleteDiscount(string productName){
        var result = await _repository.DeleteDiscount(productName);
        if(!result){
            return NoContent();
        }
        return Ok();
    }
}
