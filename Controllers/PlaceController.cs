using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using reservationApi.Repositories;
using reservationApi.Models;


namespace reservationApi.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceRepository placeRepo;

        public PlaceController(IPlaceRepository placeRepo2)
        {
            placeRepo = placeRepo2;
        }

        [HttpPost("addPlace")]
        public async Task<ActionResult<List<Place>>> addPlace([FromBody]Place place)
        {
            var res = placeRepo.addPlace(place);

            if (res != null) return Ok(res);

            //return BadRequest();
            return StatusCode(StatusCodes.Status208AlreadyReported, new { message = "متاسفانه عملیات با خطا مواجه شده است" });
        }

        [HttpPut("editPlace")]
        public async Task<ActionResult<Place>> editPlace([FromBody]PlaceEdit place)
        {
            var res = placeRepo.editPlace(place);

            if (res != null) return Ok(res);

            return StatusCode(StatusCodes.Status208AlreadyReported, new { message = "متاسفانه عملیات با خطا مواجه شده است" });
        }

        [HttpDelete("deletePlace")]
        public async Task<ActionResult> deletePlace([FromHeader]int id)
        {
            var res = placeRepo.deletePlace(id);

            if (res == "success") return StatusCode(StatusCodes.Status200OK, new { message = "عملیات با موفقیت انجام شد" });

            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "متاسفانه عملیات با خطا مواجه شده است" });
        }
        [HttpGet("searchByTypeAndName")]
        public async Task<ActionResult<List<Place>>> searchByNameAndType([FromQuery] string name, string type, int page)
        {
            var res = placeRepo.searchAll(name, type, page);

            if (res != null) return Ok(res);

            return StatusCode(StatusCodes.Status400BadRequest, new { message = "مشکل در برقراری ارتباط با سرور" });
        }

        //[HttpGet("searchByName")]
        //public async Task<ActionResult<List<Place>>> searchByName([FromQuery]string param,int page)
        //{
        //    var res = placeRepo.searchByName(param, page);

        //    return Ok(res);
        //}

        //[HttpGet("searchByType")]
        //public async Task<ActionResult<List<Place>>> searchByType([FromQuery] string param, int page)
        //{
        //    var res = placeRepo.searchByType(param, page);

        //    return Ok(res);
        //}

        [HttpPost("reservePlace")]
        public async Task<ActionResult> reserve(Reserve reserve)
        {
            var res = placeRepo.reservePlace(reserve);

            return StatusCode(StatusCodes.Status200OK, new { message= res });
        }

        
    }
}
