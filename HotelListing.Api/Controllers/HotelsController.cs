using HotelListingApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {

        private static readonly List<Hotel> Hotels = new List<Hotel>
        {
            new Hotel { Id = 1, Name = "Grand Plaza", Address = "123 Main St", Rating = 4.5 },
            new Hotel { Id = 2, Name = "Ocean View", Address = "456 Beach Rd", Rating = 4.8 }
        };
        
        // GET: api/<HotelsController>
        [HttpGet]
        public ActionResult<IEnumerable<Hotel>> Get()
        {
            return Ok(Hotels);
        }

        // GET api/<HotelsController>/5
        [HttpGet("{id}")]
        public ActionResult<Hotel> Get(int id)
        {
            var hotel = Hotels.FirstOrDefault(h => h.Id == id);
            if (hotel == null)
            {
                return NotFound();
            }
            return Ok(hotel);
        }

        // POST api/<HotelsController>
        [HttpPost]
        public ActionResult<Hotel> Post([FromBody] Hotel newHotel)
        {
            if (Hotels.Any(h => h.Id == newHotel.Id))
            {
                return BadRequest("Hotel already exists");
            }
            Hotels.Add(newHotel);
            return CreatedAtAction(nameof(Get), new { id = newHotel.Id }, newHotel);
        }

        // PUT api/<HotelsController>/5
        [HttpPut]
        public ActionResult Put([FromBody] Hotel updatedHotel)
        {
            var existingHotel = Hotels.FirstOrDefault(h => h.Id == updatedHotel.Id);
            if (existingHotel == null)
            {
                return NotFound();
            }
            
            existingHotel.Rating = updatedHotel.Rating;
            existingHotel.Address = updatedHotel.Address;
            existingHotel.Name = updatedHotel.Name;
            
            return NoContent();
        }

        // DELETE api/<HotelsController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var hotel = Hotels.FirstOrDefault(h => h.Id == id);
            if (hotel == null)
            {
                return NotFound(new  { message = "Hotel not found" });
            }
            Hotels.Remove(hotel);
            return NoContent();
        }
    }
}
