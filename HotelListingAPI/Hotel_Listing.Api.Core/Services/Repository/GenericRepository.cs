using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hotel_Listing.api.Data;
using Hotel_Listing.api.Dtos;
using Hotel_Listing.api.Exceptions;
using Hotel_Listing.api.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Listing.api.Services.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;

        public GenericRepository(HotelListingDbContext context, IMapper mapper)
        {
            _context = context;
            this._mapper = mapper;
        }
        public async Task<T> AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TResult> AddAsync<TSource, TResult>(TSource source)
        {
            var entity = _mapper.Map<T>(source);

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<TResult>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var enitity = await GetAsync(id) ?? throw new NotFoundException(typeof(T).Name, id);

            _context.Set<T>().Remove(enitity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exist(int id)
        {
            var entity = await GetAsync(id);
            return entity != null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<PagedResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters)
        {
            var totalSize = await _context.Set<T>().CountAsync();
            var items = await _context.Set<T>()
                .Skip(queryParameters.StartIndex)
                .Take(queryParameters.PageSize)
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return new PagedResult<TResult>
            {
                Items = items,
                PageNumber = queryParameters.PageNumber,  
                RecordNumber = queryParameters.PageSize,
                TotalCount = totalSize
            };
        }

        public async Task<List<TResult>> GetAllAsync<TResult>()
        {
            return await _context.Set<T>()
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<T> GetAsync(int? id)
        {
            if (id is null)
            {
                return null;
            }
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<TResult> GetAsync<TResult>(int? id)
        {
            var result = await _context.Set<T>().FindAsync(id);
            if (result is null)
            {
                throw new NotFoundException(typeof(T).Name, id.HasValue ? id : "No Key Provided");
            }
            return _mapper.Map<TResult>(result); 
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync<TSource>(int id, TSource source)
        {
            var entity = await GetAsync(id) ?? throw new NotFoundException(typeof(T).Name, id);

            _mapper.Map(source, entity);
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
