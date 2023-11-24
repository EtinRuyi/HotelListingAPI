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
using Microsoft.AspNetCore.OData.Query;

namespace Hotel_Listing.api.Controllers
{
    [Route("api/v{version:apiVersion}/countries")]
    [ApiController]
    [ApiVersion("2.0")]
    public class CountriesV2Controller : ControllerBase
    {
        private readonly ICountryRepo _contry;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public CountriesV2Controller(IMapper mapper, ICountryRepo contry, ILogger<CountriesV2Controller> logger)
        {
            _mapper = mapper;
            _contry = contry;
            this._logger = logger;
        }

        // GET: api/Countries
        [HttpGet]
        [EnableQuery] 
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            var countries = await _contry.GetAllAsync();
            var records = _mapper.Map<List<GetCountryDto>>(countries);
            return Ok(records);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _contry.GetDetails(id);
            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountry), id);
            }
              
            var response = _mapper.Map<CountryDto>(country);

            return response;
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

            var country = await _contry.GetAsync(id);
            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountries), id);
            }
            _mapper.Map(updateCountry, country);

            try
            {
                await _contry.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
                {
                    throw new NotFoundException(nameof(GetCountries), id);
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
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto countryDto)
        {
            var country = _mapper.Map<Country>(countryDto);

            await _contry.AddAsync(country);

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _contry.GetAsync(id);
            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountries), id);
            }

            await _contry.DeleteAsync(id);

            return NoContent();
        }

        private Task<bool> CountryExists(int id)
        {
            return _contry.Exist(id);
        }
    }
}
