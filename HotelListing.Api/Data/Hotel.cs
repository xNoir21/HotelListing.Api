namespace HotelListing.Api.Data;

public class Hotel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public double Rating { get; set; }
    public decimal PerNightRate { get; set; }
    public int CountryId { get; set; }
    public Country? Country { get; set; }
    public ICollection<HotelAdmin> Admins { get; set; } = new List<HotelAdmin>();
    public ICollection<Booking> Bookings { get; set; } =  new List<Booking>();
}