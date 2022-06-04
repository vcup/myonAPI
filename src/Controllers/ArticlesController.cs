using Microsoft.AspNetCore.Mvc;
using myonAPI.Services;
using myonAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace myonAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _service;

    public ArticlesController(IArticleService service)
    {
        _service = service;
    }

    // GET: api/<ArticlesController>
    [HttpGet]
    public ActionResult<IEnumerable<ArticleDescriptor>> Get()
    {
        return Ok(_service.Get());
    }

    // GET api/<ArticlesController>/Title
    [HttpGet("{id:int}")]
    public ActionResult<ArticleDescriptor> Get(int id)
    {
        if (id < 0)
        {
            return BadRequest(id);
        }

        var result = _service.Get(id);
        if (result is null)
        {
            return NotFound();
        }

        return result;
    }

    // POST api/<ArticlesController>
    [HttpPost]
    public ActionResult Post(ArticleDescriptor article)
    {
        if (article.Id < 0 || _service.Contains(article))
        {
            return BadRequest(article);
        }

        return _service.Create(article) ? Ok() : BadRequest(article);
    }

    // PUT api/<ArticlesController>
    [HttpPut]
    public ActionResult Put(ArticleDescriptor article)
    {
        if (string.IsNullOrEmpty(article.Title))
        {
            return BadRequest(article);
        }
        else if (!_service.Contains(article))
        {
            return NotFound();
        }

        return _service.Update(article) ? Ok() : BadRequest(article);
    }

    // DELETE api/<ArticlesController>/Title
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        if (id < 0)
        {
            return BadRequest();
        }
        else if (!_service.Contains(id))
        {
            return NotFound();
        }

        return Ok();
    }
}