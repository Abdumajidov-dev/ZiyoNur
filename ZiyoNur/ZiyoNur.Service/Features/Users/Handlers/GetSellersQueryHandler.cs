using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Auth;
using ZiyoNur.Service.Features.Users.Queries;

namespace ZiyoNur.Service.Features.Users.Handlers;

public class GetSellersQueryHandler : IRequestHandler<GetSellersQuery, BaseResponse<PagedResponse<UserDto>>>
{
    private readonly ISellerRepository _sellerRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSellersQueryHandler> _logger;

    public GetSellersQueryHandler(
        ISellerRepository sellerRepository,
        IMapper mapper,
        ILogger<GetSellersQueryHandler> logger)
    {
        _sellerRepository = sellerRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<PagedResponse<UserDto>>> Handle(GetSellersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Implement advanced filtering in repository
            var sellers = await _sellerRepository.GetAllAsync();

            // Apply filters
            var filteredSellers = sellers.AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                filteredSellers = filteredSellers.Where(s =>
                    s.FirstName.ToLower().Contains(searchTerm) ||
                    s.LastName.ToLower().Contains(searchTerm) ||
                    s.Phone.Contains(searchTerm));
            }

            if (request.IsActive.HasValue)
            {
                filteredSellers = filteredSellers.Where(s => s.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrEmpty(request.Role))
            {
                filteredSellers = filteredSellers.Where(s => s.Role == request.Role);
            }

            // Apply sorting
            filteredSellers = request.SortBy?.ToLower() switch
            {
                "firstname" => request.SortDescending
                    ? filteredSellers.OrderByDescending(s => s.FirstName)
                    : filteredSellers.OrderBy(s => s.FirstName),
                "lastname" => request.SortDescending
                    ? filteredSellers.OrderByDescending(s => s.LastName)
                    : filteredSellers.OrderBy(s => s.LastName),
                "phone" => request.SortDescending
                    ? filteredSellers.OrderByDescending(s => s.Phone)
                    : filteredSellers.OrderBy(s => s.Phone),
                "createdat" => request.SortDescending
                    ? filteredSellers.OrderByDescending(s => s.CreatedAt)
                    : filteredSellers.OrderBy(s => s.CreatedAt),
                _ => filteredSellers.OrderBy(s => s.FirstName)
            };

            var totalCount = filteredSellers.Count();

            // Apply pagination
            var pagedSellers = filteredSellers
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var sellerDtos = _mapper.Map<List<UserDto>>(pagedSellers);
            var pagedResponse = new PagedResponse<UserDto>(sellerDtos, request.PageIndex, request.PageSize, totalCount);

            return BaseResponse<PagedResponse<UserDto>>.Success(pagedResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sellers");
            return BaseResponse<PagedResponse<UserDto>>.Failure("Sotuvchilarni olishda xatolik yuz berdi");
        }
    }
}