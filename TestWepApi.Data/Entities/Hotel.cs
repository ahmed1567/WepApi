using System.ComponentModel.DataAnnotations.Schema;

namespace WepApiTest.Data;

public class Hotel
{
    public int  Id { get; set; } 
    public string Name { get; set; }
    public string Address { get; set; } 
    public double Rating { get; set; }

    public Country Country { get; set; }
    [ForeignKey(nameof(CountryId))]
    public int CountryId { get; set; }
}
