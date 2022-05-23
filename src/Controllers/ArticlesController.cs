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
    public ActionResult<IEnumerable<ArticleInfo>> Get()
    {
        return Ok(_service.Get());
    }

    // GET api/<ArticlesController>/Title
    [HttpGet("{title}")]
    public ActionResult<ArticleInfo> Get(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return BadRequest(title);
        }

        var result = _service.Get(title);
        if (result is null)
        {
            return NotFound();
        }

        return result;
    }

    // POST api/<ArticlesController>
    [HttpPost]
    public ActionResult Post(ArticleInfo article)
    {
        if (string.IsNullOrEmpty(article.Title))
        {
            return BadRequest(article);
        }
        else if (!_service.Contains(article))
        {
            return NotFound();
        }

        return _service.Create(article) ? Ok() : BadRequest(article);
    }

    // PUT api/<ArticlesController>
    [HttpPut]
    public ActionResult Put(ArticleInfo article)
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
    [HttpDelete("{title}")]
    public ActionResult Delete(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return BadRequest();
        }
        else if (!_service.Contains(title))
        {
            return NotFound();
        }

        return Ok();
    }
}