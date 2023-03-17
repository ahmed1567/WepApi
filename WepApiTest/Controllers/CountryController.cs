using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace WepApiTest;
[Route("api/[controller]")]
[ApiController]
public class CountryController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CountryController> _logger;
    private readonly IMapper _mapper;

    public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    //[ResponseCache(CacheProfileName = "120SecondDuration")]
    //[HttpCacheExpiration(CacheLocation =CacheLocation.Public,MaxAge =60)]
    //[HttpCacheValidation(MustRevalidate =false)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCountries([FromQuery] RequestParams requestParams)
    {

        var countries = await _unitOfWork.Countries.GetPaging(requestParams);
        var results = _mapper.Map<IList<CountryDTO>>(countries);
        return Ok(results);
    }


    [HttpGet("{id:int}", Name = "GetCountry")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCountry(int id)
    {
        var country = await _unitOfWork.Countries.GetById(q => q.Id == id, new List<string> { "Hotels" });
        var result = _mapper.Map<CountryDTO>(country);
        return Ok(result);
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> CreateCountry([FromBody] CountryCreateDTO countryDTO)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation($"Invalid Post Attempt in {nameof(CreateCountry)}");
            return BadRequest(ModelState);
        }
        var country = _mapper.Map<Country>(countryDTO);

        await _unitOfWork.Countries.Insert(country);
        await _unitOfWork.Save();
        return CreatedAtRoute("GetCountry", new { id = country.Id }, country);
    }


    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDTO)
    {
        if (!ModelState.IsValid || id < 1)
        {
            _logger.LogInformation($"Invalid Put Attempt in {nameof(UpdateCountry)}");
            return BadRequest(ModelState);
        }
        var country = await _unitOfWork.Countries.GetById(q => q.Id == id);

        if (country == null)
        {
            _logger.LogInformation($"Invalid Put Attempt in {nameof(UpdateCountry)}");
            return BadRequest("Not Found that Country");

        }
        _mapper.Map(countryDTO, country);
        _unitOfWork.Countries.Update(country);
        await _unitOfWork.Save();
        return NoContent();

    }


    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        if (!ModelState.IsValid || id < 1)
        {
            _logger.LogInformation($"Invalid Delete Attempt in {nameof(DeleteCountry)}");
            return BadRequest(ModelState);
        }

        var country = await _unitOfWork.Countries.GetById(q => q.Id == id);

        if (country == null)
        {
            _logger.LogInformation($"Invalid Delete Attempt in {nameof(DeleteCountry)}");
            return BadRequest("Not Found that Country");

        }
        await _unitOfWork.Countries.Delete(id);
        await _unitOfWork.Save();
        return NoContent();


    }
}
