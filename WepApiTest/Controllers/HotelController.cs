using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WepApiTest;

[Route("api/[controller]")]
[ApiController]
public class HotelController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HotelController> _logger;
    private readonly IMapper _mapper;

    public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHotels()
    {
        var hotels = await _unitOfWork.Hotels.GetAll();
        var results = _mapper.Map<IList<HotelDTO>>(hotels);
        return Ok(results);

    }

    [HttpGet("{id:int}", Name = "GetHotel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHotel(int id)
    {

        var Hotel = await _unitOfWork.Hotels.GetById(q => q.Id == id, new List<string> { "Country" });
        var result = _mapper.Map<HotelDTO>(Hotel);
        return Ok(result);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation($"Invalid Post Attempt in {nameof(CreateHotel)}");
            return BadRequest(ModelState);
        }

        var hotel = _mapper.Map<Hotel>(hotelDTO);

        await _unitOfWork.Hotels.Insert(hotel);
        await _unitOfWork.Save();
        return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);

    }



    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDTO)
    {
        if (!ModelState.IsValid || id < 1)
        {
            _logger.LogInformation($"Invalid Put Attempt in {nameof(UpdateHotel)}");
            return BadRequest(ModelState);
        }

        var hotel = await _unitOfWork.Hotels.GetById(q => q.Id == id);

        if (hotel == null)
        {
            _logger.LogInformation($"Invalid Put Attempt in {nameof(UpdateHotel)}");
            return BadRequest("Not Found that Hotel");

        }
        _mapper.Map(hotelDTO, hotel);
        _unitOfWork.Hotels.Update(hotel);
        await _unitOfWork.Save();
        return NoContent();
    }


    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        if (!ModelState.IsValid || id < 1)
        {
            _logger.LogInformation($"Invalid Delete Attempt in {nameof(DeleteHotel)}");
            return BadRequest(ModelState);
        }

        var hotel = await _unitOfWork.Hotels.GetById(q => q.Id == id);

        if (hotel == null)
        {
            _logger.LogInformation($"Invalid Delete Attempt in {nameof(DeleteHotel)}");
            return BadRequest("Not Found that Hotel");

        }
        await _unitOfWork.Hotels.Delete(id);
        await _unitOfWork.Save();
        return NoContent();

    }
}
