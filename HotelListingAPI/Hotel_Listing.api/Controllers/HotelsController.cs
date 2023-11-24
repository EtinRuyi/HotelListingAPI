using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hotel_Listing.api.Data;
using Hotel_Listing.api.Models;
using AutoMapper;
using Hotel_Listing.api.Dtos.Hotel;
using Hotel_Listing.api.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Hotel_Listing.api.Dtos;
using Hotel_Listing.api.Dtos.Country;
using Microsoft.AspNetCore.OData.Query;
using System.Diagnostics.Metrics;

namespace Hotel_Listing.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelRepo _hotel;
        private readonly IMapper _mapper;

        public HotelsController(IHotelRepo hotel, IMapper mapper)
        {
            _hotel = hotel;
            _mapper = mapper;
        }

        // GET: api/Hotels
        [HttpGet("GetAll")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
        {
            var hotels = await _hotel.GetAllAsync<HotelDto>();
            return Ok(hotels);
        }

        // GET: api/Hotels?StartIndex=0&pageSize=25&PageNumber=1
        [HttpGet]
        public async Task<ActionResult<PagedResult<HotelDto>>> GetHotels([FromQuery] QueryParameters queryParameters)
        {
            var pagedHotelsResult = await _hotel.GetAllAsync<HotelDto>(queryParameters);
            return Ok(pagedHotelsResult);
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            var hotel = await _hotel.GetAsync<HotelDto>(id);
            return Ok(hotel);
        }

        // PUT: api/Hotels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutHotel(int id, HotelDto hotelDto)
        {
            if (id != hotelDto.Id)
            {
                return BadRequest();
            }

            try
            {
                await _hotel.UpdateAsync(id, hotelDto); 
            }
            catch (DbUpdateConcurrencyException)
            {
                if (! await HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/Hotels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDto hotelDto)
        {
            var hotel = await _hotel.AddAsync<CreateHotelDto, HotelDto>(hotelDto);
            return CreatedAtAction(nameof(GetHotel), new { id = hotel.Id }, hotel);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            await _hotel.DeleteAsync(id);
            return NoContent();
        }

        private async Task<bool> HotelExists(int id)
        {
            return await _hotel.Exist(id);
        }
    }
}
