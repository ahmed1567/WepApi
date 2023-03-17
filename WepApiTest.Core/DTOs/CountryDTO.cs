namespace WepApiTest.Core;

public class CountryDTO : CountryCreateDTO
{
    public int Id { get; set; }
    public IList<HotelDTO> Hotels { get; set; }

}

public class CountryCreateDTO
{
    [Required]
    [StringLength(maximumLength: 50, ErrorMessage = "Country Name Is Too Long")]
    public string Name { get; set; }

    [Required]
    [StringLength(maximumLength: 5, ErrorMessage = " Short Country Name Is Too Long")]
    public string ShortName { get; set; }
}
public class UpdateCountryDTO:CountryCreateDTO
{
    public IList<CreateHotelDTO> Hotels { get; set; }

}






