using EmploymentAgency.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace EmploymentAgency.Controllers;

[ApiController]
public class CrudController<T> : ControllerBase
    where T : class, IIDentifiable
{
    private readonly Service<T> _service;

    public CrudController(Service<T> service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> Post(T entity)
    {
        await _service.CreateAsync(entity);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        return Ok(await _service.ReadAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<T>> Get(int id)
    {
        T? entity = await _service.ReadAsync(id);
        if (entity is null)
        {
            return NotFound(id);
        }
        return Ok(entity);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] T entity)
    {
        bool found = await _service.UpdateAsync(id, entity);
        if (!found)
        {
            return NotFound(id);
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        bool found = await _service.DeleteAsync(id);
        if (!found)
        {
            return NotFound(id);
        }
        return NoContent();
    }

    public override NotFoundObjectResult NotFound([ActionResultObjectValue] object? id)
    {
        return base.NotFound($"Сущность с id = {id} не найдена.");
    }
}

[Route("api/[controller]")]
public class AddressController : CrudController<Address>
{
    public AddressController(Service<Address> service)
        : base(service)
    {
    }
}

[Route("api/[controller]")]
public class ApplicationController : CrudController<Application>
{
    public ApplicationController(Service<Application> service)
        : base(service)
    {
    }
}

[Route("api/[controller]")]
public class ChangeLogController : CrudController<ChangeLog>
{
    public ChangeLogController(Service<ChangeLog> service)
        : base(service)
    {
    }
}

[Route("api/[controller]")]
public class DistrictController : CrudController<District>
{
    public DistrictController(Service<District> service)
        : base(service)
    {
    }
}

[Route("api/[controller]")]
public class EmployerController : CrudController<Employer>
{
    public EmployerController(Service<Employer> service)
        : base(service)
    {
    }
}

[Route("api/[controller]")]
public class EmploymentTypeController : CrudController<EmploymentType>
{
    public EmploymentTypeController(Service<EmploymentType> service)
        : base(service)
    {
    }
}

[Route("api/[controller]")]
public class PositionController : CrudController<Position>
{
    public PositionController(Service<Position> service)
        : base(service)
    {
    }
}

[Route("api/[controller]")]
public class PropertyController : CrudController<Property>
{
    public PropertyController(Service<Property> service)
        : base(service)
    {
    }
}

[Route("api/[controller]")]
public class SeekerController : CrudController<Seeker>
{
    public SeekerController(Service<Seeker> service)
        : base(service)
    {
    }
}

[Route("api/[controller]")]
public class StatusController : CrudController<Status>
{
    public StatusController(Service<Status> service)
        : base(service)
    {
    }
}

[Route("api/[controller]")]
public class StreetController : CrudController<Street>
{
    public StreetController(Service<Street> service)
        : base(service)
    {
    }
}

[Route("api/[controller]")]
public class VacancyController : CrudController<Vacancy>
{
    public VacancyController(Service<Vacancy> service)
        : base(service)
    {
    }
}
