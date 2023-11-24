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
using Hotel_Listing.api.Dtos.Country;
using Hotel_Listing.api.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Hotel_Listing.api.Exceptions;
using Hotel_Listing.api.Dtos;
using Microsoft.AspNetCore.OData.Query;

namespace Hotel_Listing.api.Controllers
{
    [Route("api/v{version:apiVersion}/countries")]
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryRepo _contry;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public CountriesController(IMapper mapper, ICountryRepo contry, ILogger<CountriesController> logger)
        {
            _mapper = mapper;
            _contry = contry;
            this._logger = logger;
        }

        // GET: api/Countries
        [HttpGet("GetAll")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            var countries = await _contry.GetAllAsync<GetCountryDto>();
            return Ok(countries);
        }
        // GET: api/Countries/?StartIndex=0&pageSize=25&PageNumber=1
        [HttpGet]
        public async Task<ActionResult<PagedResult<GetCountryDto>>> GetPagedCountries([FromQuery] QueryParameters queryParameters)
        {
            var pagedCountriesResult = await _contry.GetAllAsync<GetCountryDto>(queryParameters);
            return Ok(pagedCountriesResult);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _contry.GetDetails(id);
            return Ok(country);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountry)
        {
            if (id != updateCountry.Id)
            {
                return BadRequest("Invalid Record ID");
            }

            try
            {
                await _contry.UpdateAsync(id, updateCountry);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
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

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CountryDto>> PostCountry(CreateCountryDto createcountryDto)
        {
            var country = await _contry.AddAsync<CreateCountryDto, GetCountryDto>(createcountryDto);
            return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            await _contry.DeleteAsync(id);
            return NoContent();
        }

        private Task<bool> CountryExists(int id)
        {
            return _contry.Exist(id);
        }
    }
}
