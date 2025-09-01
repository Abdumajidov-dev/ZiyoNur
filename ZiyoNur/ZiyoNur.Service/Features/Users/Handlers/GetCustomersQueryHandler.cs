using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Auth;
using ZiyoNur.Service.Features.Users.Queries;

namespace ZiyoNur.Service.Features.Users.Handlers;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, BaseResponse<PagedResponse<UserDto>>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCustomersQueryHandler> _logger;

    public GetCustomersQueryHandler(
        ICustomerRepository customerRepository,
        IMapper mapper,
        ILogger<GetCustomersQueryHandler> logger)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<PagedResponse<UserDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Implement advanced filtering in repository
            var customers = await _customerRepository.GetAllAsync();

            // Apply filters
            var filteredCustomers = customers.AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                filteredCustomers = filteredCustomers.Where(c =>
                    c.FirstName.ToLower().Contains(searchTerm) ||
                    c.LastName.ToLower().Contains(searchTerm) ||
                    c.Phone.Contains(searchTerm) ||
                    (c.Email != null && c.Email.ToLower().Contains(searchTerm)));
            }

            if (request.IsActive.HasValue)
            {
                filteredCustomers = filteredCustomers.Where(c => c.IsActive == request.IsActive.Value);
            }

            if (request.RegisteredFrom.HasValue)
            {
                filteredCustomers = filteredCustomers.Where(c => c.CreatedAt >= request.RegisteredFrom.Value);
            }

            if (request.RegisteredTo.HasValue)
            {
                filteredCustomers = filteredCustomers.Where(c => c.CreatedAt <= request.RegisteredTo.Value);
            }

            // Apply sorting
            filteredCustomers = request.SortBy?.ToLower() switch
            {
                "firstname" => request.SortDescending
                    ? filteredCustomers.OrderByDescending(c => c.FirstName)
                    : filteredCustomers.OrderBy(c => c.FirstName),
                "lastname" => request.SortDescending
                    ? filteredCustomers.OrderByDescending(c => c.LastName)
                    : filteredCustomers.OrderBy(c => c.LastName),
                "phone" => request.SortDescending
                    ? filteredCustomers.OrderByDescending(c => c.Phone)
                    : filteredCustomers.OrderBy(c => c.Phone),
                "createdat" => request.SortDescending
                    ? filteredCustomers.OrderByDescending(c => c.CreatedAt)
                    : filteredCustomers.OrderBy(c => c.CreatedAt),
                _ => filteredCustomers.OrderBy(c => c.FirstName)
            };

            var totalCount = filteredCustomers.Count();

            // Apply pagination
            var pagedCustomers = filteredCustomers
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var customerDtos = _mapper.Map<List<UserDto>>(pagedCustomers);
            var pagedResponse = new PagedResponse<UserDto>(customerDtos, request.PageIndex, request.PageSize, totalCount);

            return BaseResponse<PagedResponse<UserDto>>.Success(pagedResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers");
            return BaseResponse<PagedResponse<UserDto>>.Failure("Mijozlarni olishda xatolik yuz berdi");
        }
    }
}