using AutoMapper;

namespace WepApiTest.Core;

public class InitialMapper:Profile
{
	public InitialMapper()
	{
		CreateMap<Country,CountryDTO>().ReverseMap();
		CreateMap<Country,CountryCreateDTO>().ReverseMap();
		CreateMap<Hotel,HotelDTO>().ReverseMap();
		CreateMap<Hotel,CreateHotelDTO>().ReverseMap();
		CreateMap<ApiUser,UserDTO>().ReverseMap();	
		CreateMap<ApiUser,LoginDTO>().ReverseMap();	
	}
}
