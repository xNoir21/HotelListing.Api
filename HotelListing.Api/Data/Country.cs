namespace HotelListingApi.Data;

public class Country
{
    public int CountryId { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}