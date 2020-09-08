using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatjiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> PostTest(IFormCollection files)
        {
            try
            {
                var vid = files["vid"];
                foreach (var v in files.Files)
                {
                    int p = v.FileName.LastIndexOf('.');
                    string ext = v.FileName.Substring(p);
                    FileStream F = new FileStream("wwwroot/videos/" + vid + ext, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
                    await v.CopyToAsync(F);
                    F.Close();
                }
            }
            catch
            {
                return NotFound("上传失败");
            }
            return Ok();
        }

        public class Test
        {
            public Test(int i)
            {
                x1 = i;
            }
            public int x1 = 10;
            public decimal x2 = 3.1415926M;
            public string x3 = "Hello World!";
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/test
        [HttpPost("test")]
        public ActionResult<IEnumerable<Test>> Get2(Test T)
        {
            //return new string[] { "value1", "value2" };
            return Ok(T.x2);
        }

        // GET api/values/5
        [HttpGet("id")]
        public ActionResult<string> Get(int ID)
        {
            return ID.ToString();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
