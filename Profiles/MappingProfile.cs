using AutoMapper;
using FoodHub.DTOs.Auth;
using FoodHub.DTOs.Cart;
using FoodHub.DTOs.Category;
using FoodHub.DTOs.MenuItem;
using FoodHub.DTOs.Order;
using FoodHub.DTOs.Restaurant;
using FoodHub.Models;
namespace FoodHub.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User

            CreateMap<ApplicationUser, UserDTO>();

            // Restaurant

            CreateMap<Restaurant, RestaurantDTO>()
                .ForMember(
                dest => dest.OwnerName,
                opt => opt.MapFrom(src =>
                    src.Owner.FirstName + " " + src.Owner.LastName));

            CreateMap<CreateRestaurantDTO, Restaurant>();

            CreateMap<UpdateRestaurantDTO, Restaurant>();

            // Category

            CreateMap<Category, CategoryDTO>();

            CreateMap<CreateCategoryDTO, Category>();

            CreateMap<UpdateCategoryDTO, Category>();

            // Menu Item

            CreateMap<MenuItem, MenuItemDTO>();

            CreateMap<CreateMenuItemDTO, MenuItem>();

            CreateMap<UpdateMenuItemDTO, MenuItem>();
            // Cart Item

            CreateMap<CartItem, CartItemDTO>()
                .ForMember(
                    dest => dest.MenuItemName,
                    opt => opt.MapFrom(src => src.MenuItem.Name))
                .ForMember(
                    dest => dest.Price,
                    opt => opt.MapFrom(src => src.MenuItem.Price))
                .ForMember(
                    dest => dest.SubTotal,
                    opt => opt.MapFrom(src =>
                        src.MenuItem.Price * src.Quantity));

            // Cart

            CreateMap<Cart, CartDTO>()
                .ForMember(
                    dest => dest.Items,
                    opt => opt.MapFrom(src => src.CartItems));

            // Order Item

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(
                    dest => dest.MenuItemName,
                    opt => opt.MapFrom(src => src.MenuItem.Name)
                );

            // Order

            CreateMap<Order, OrderDTO>()
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(
                    dest => dest.Items,
                    opt => opt.MapFrom(src => src.OrderItems));
        }
    }
}
